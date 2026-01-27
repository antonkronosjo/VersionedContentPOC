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
    bool Exists(Guid contentId);
    ContentRoot Get(Guid contentId);
    Type GetContentRootType(Guid contentId);
    void SetPublishState(Guid contentId, DateTime? startPublish, DateTime? stopPublish);
    T Create<T>(T content) where T : Content;
    T Update<T>(Guid contentId, T contentVersion, bool forceUpdate = false) where T : Content;
    T Update<T>(T content, IDictionary<string, ContentPropertyValueDto> updates, bool forceUpdate = false) where T : Content;
    void Delete(Guid contentId);
    IQueryable<T> Query<T>(Language languageBranch) where T : Content;
    IQueryable<ContentRoot> QueryRoots();
    void SetAsActiveVersion(Guid versionId);
    List<Language> GetTranslatedLanguages(Guid contentId);
}

public class ContentRepository : IContentRepository
{
    VersionedContentPOCContext _context;
    IContentVersionRepository _contentVersionRepository;

    public ContentRepository(VersionedContentPOCContext context, IContentVersionRepository contentVersionRepository)
    {
        _context = context;
        _contentVersionRepository = contentVersionRepository;
    }

    /// <summary>
    /// Returns currently active version of content for language. Returns null if entity not found or not active
    /// </summary>
    public T? Get<T>(Guid contentId, Language language) where T : Content 
    {
        return Query<T>(language).FirstOrDefault(x => x.ContentId == contentId);
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
            var contentRoot = new ContentRoot(Guid.NewGuid(), initialVersion.Language);
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
        return _contentVersionRepository.AddVersion(contentId, updatedVersion, forceUpdate);
    }

    /// <summary>
    /// Updates content with new version based on key/values.
    /// </summary>
    public T Update<T>(T content, IDictionary<string, ContentPropertyValueDto> updates, bool forceUpdate = false) where T : Content
    {
        //var entity = _context.Entities.Find(id);
        _context.Entry(content).State = EntityState.Detached; //Need to detach state before applying updates
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
        _contentVersionRepository.SetAsActiveVersion(versionId);
    }

    /// <summary>
    /// Returns a base query used when querying content
    /// </summary>
    public IQueryable<T> Query<T>(Language language) where T : Content
    {
        return _context.Content.OfType<T>()
            .Where(x => x.Language == language)
            .Include(x => x.LanguageBranch)
            .Where(x => x.LanguageBranch.ActiveVersionId == x.VersionId)
            .Include(x => x.ContentRoot);
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