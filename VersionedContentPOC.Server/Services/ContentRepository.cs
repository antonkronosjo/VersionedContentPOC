using Microsoft.EntityFrameworkCore;
using VersionedContentPOC.Data;
using VersionedContentPOC.Data.Enums;
using VersionedContentPOC.Data.Models;
using VersionedContentPOC.Server.Attributes;
using VersionedContentPOC.Server.Extensions;

namespace VersionedContentPOC.Server.Services;

public interface IContentRepository
{
    T? Get<T>(Guid contentId, Language language) where T : Content;
    T? GetVersion<T>(Guid contentId, Guid versionId, Language language) where T : Content;
    bool Exists(Guid contentId);
    ContentRoot Get(Guid contentId);
    Type GetContentRootType(Guid contentId);
    void SetPublishState(Guid contentId, DateTime? startPublish, DateTime? stopPublish);
    T Create<T>(T content) where T : Content;
    T Update<T>(Guid contentId, T contentVersion, bool forceUpdate = false) where T : Content;
    T Update<T>(T content, IDictionary<string, ContentPropertyValueDto> updates, bool forceUpdate = false) where T : Content;
    void Delete(Guid contentId);
    IQueryable<T> QueryActiveVersions<T>(Language languageBranch) where T : Content;
    IQueryable<T> Versions<T>(Guid contentId, Language language) where T : Content;
    IQueryable<ContentRoot> QueryRoots();
    void SetAsActiveVersion(Guid versionId);
    List<Language> GetTranslatedLanguages(Guid contentId);
}

public class ContentRepository : IContentRepository
{
    VersionedContentPOCContext _context;

    public ContentRepository(VersionedContentPOCContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Returns currently active version of content for language. Returns null if entity not found or not active
    /// </summary>
    public T? Get<T>(Guid contentId, Language language) where T : Content 
    {
        return QueryActiveVersions<T>(language).FirstOrDefault(x => x.ContentId == contentId);
    }

    /// <summary>
    /// Returns version of content for language. Returns null if not found,
    /// </summary>
    public T? GetVersion<T>(Guid contentId, Guid versionId, Language language) where T : Content
    {
        return QueryVersions<T>(language)
            .FirstOrDefault(x => x.ContentId == contentId && x.VersionId == versionId);
    }

    /// <summary>
    /// Returns true if content with contentid exists
    /// </summary>
    public bool Exists(Guid contentId)
    {
        return _context.ContentRoots.Any(x => x.ContentId == contentId);
    }

    /// <summary>
    /// Returns root content object
    /// </summary>
    public ContentRoot Get(Guid contentId)
    {
        return _context.ContentRoots
            .Where(x => x.ContentId == contentId)
            .Include(x => x.LanguageBranches)
            .Single();
    }

    public void SetPublishState(Guid contentId, DateTime? startPublish, DateTime? stopPublish)
    {
        var contentRoot = _context.ContentRoots.Single(x => x.ContentId == contentId);
        contentRoot.StartPublish = startPublish;
        contentRoot.StopPublish = stopPublish;
        _context.SaveChanges();
    }

    [ShouldBeRefactored("To get type of ContentRoot should be done in a more eligant way")]
    public Type GetContentRootType(Guid contentId)
    {
        return _context.Content.First(x => x.ContentId == contentId).GetType();
    }

    /// <summary>
    /// Creates new content with underlying root object, language branch and version handling
    /// </summary>
    public T Create<T>(T initialVersion) where T : Content
    {
        using var transaction = _context.Database.BeginTransaction();
        try
        {
            var contentRoot = new ContentRoot(Guid.NewGuid());
            var languageBranch = contentRoot.AddNewLanguageBranch(initialVersion.Language);
            _context.Add(contentRoot);
            _context.SaveChanges();

            languageBranch.AddVersion(initialVersion, setAsActive: true);
            _context.Add(initialVersion);
            _context.Update(languageBranch);
            _context.SaveChanges();

            transaction.Commit();
            return initialVersion;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    /// <summary>
    /// Delets content and all versions of it
    /// </summary>
    public void Delete(Guid contentId)
    {
        var content = _context.ContentRoots.Where(x => x.ContentId == contentId);
        _context.Remove(content);
        _context.SaveChanges();
    }

    /// <summary>
    /// Updates content with new version. NOTE: Will throw exception if content.VersionId does not match currently active content version
    /// </summary>
    [ShouldBeRefactored("Refactor this so that it makes sense regarding force update")]
    public T Update<T>(Guid contentId, T updatedVersion, bool forceUpdate = false) where T : Content
    {
        using var transaction = _context.Database.BeginTransaction();
        try
        {
            var root = _context.ContentRoots
                .Include(r => r.LanguageBranches)
                    .ThenInclude(m => m.Versions)
                .Single(r => r.ContentId == contentId);

            var newLanguageBranch = root.AddNewLanguageBranchIfNotExist(updatedVersion.Language);
            if (newLanguageBranch != null)
            {
                _context.Add(newLanguageBranch);
                _context.SaveChanges();
            }

            var languageBranch = newLanguageBranch ?? root.LanguageBranches.Single(x => x.Language == updatedVersion.Language);

            if (!forceUpdate && languageBranch.ActiveVersionId != null && languageBranch.ActiveVersionId != updatedVersion.VersionId)
                throw new Exception("Content.VersionId does not match the current one being active. Use forceUpdate=true to save");
            else
            {
                updatedVersion.VersionId = Guid.NewGuid();
                updatedVersion.VersionCreated = DateTime.UtcNow;
            }
                

            languageBranch.AddVersion(updatedVersion);

            _context.Update(languageBranch);
            _context.Add(updatedVersion);
            _context.SaveChanges();
            transaction.Commit();

            return updatedVersion;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    /// <summary>
    /// Updates content with new version based on key/values.
    /// </summary>
    public T Update<T>(T content, IDictionary<string, ContentPropertyValueDto> updates, bool forceUpdate = false) where T : Content
    {
        ContentUpdater.ApplyUpdates(content, updates);
        return Update<T>(content.ContentId, content);
    }

    /// <summary>
    /// Returns all versions of given content
    /// </summary>
    public IQueryable<T> Versions<T>(Guid contentId, Language language) where T : Content
    {
        return _context.Content.OfType<T>()
            .Where(x => x.ContentId == contentId && x.Language == language)
            .Include(x => x.ContentRoot)
            .Include(x => x.LanguageBranch);
    }

    public void SetAsActiveVersion(Guid versionId)
    {
        var content = _context.Content
            .WhereVersion(versionId)
            .Include(x => x.LanguageBranch)
            .Single();

        content.LanguageBranch.SetActiveVersion(content);
        _context.Update(content.LanguageBranch);
        _context.SaveChanges();
    }

    /// <summary>
    /// Returns a base query used when querying active versions of content
    /// </summary>
    public IQueryable<T> QueryActiveVersions<T>(Language language) where T : Content
    {
        return _context.Content.OfType<T>()
            .WhereLanguage(language)
            .WhereActive();
    }

    /// <summary>
    /// Returns a base query used when querying all versions of content
    /// </summary>
    public IQueryable<T> QueryVersions<T>(Language language) where T : Content
    {
        return _context.Content.OfType<T>()
            .Include(x => x.LanguageBranch)
            .WhereLanguage(language);
    }

    /// <summary>
    /// Returns a base query used when querying content roots
    /// </summary>
    public IQueryable<ContentRoot> QueryRoots() 
    {
        return _context.ContentRoots;
    }

    /// <summary>
    /// Returns all languages content has been translated to
    /// </summary>
    [ShouldBeRefactored("This method should be moved to another class, maybe IContentMetadataService or similar?")]
    public List<Language> GetTranslatedLanguages(Guid contentId)
    {
        return _context.ContentRoots
            .Include(x => x.LanguageBranches)
            .Single(x => x.ContentId == contentId)
            .LanguageBranches
            .Select(x => x.Language)
            .ToList();
    }
}