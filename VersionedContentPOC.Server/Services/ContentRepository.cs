using Microsoft.EntityFrameworkCore;
using VersionedContentPOC.Data;
using VersionedContentPOC.Data.Enums;
using VersionedContentPOC.Data.Models;
using VersionedContentPOC.Server.Attributes;

namespace VersionedContentPOC.Server.Services;

public interface IContentRepository
{
    T? Get<T>(Guid contentId, Language language) where T : Content;
    T Create<T>(T content) where T : Content;
    T Update<T>(Guid contentId, T contentVersion, bool forceUpdate = false) where T : Content;
    T Update<T>(T content, IDictionary<string, ContentPropertyValueDto> updates, bool forceUpdate = false) where T : Content;
    void Delete(Guid contentId);
    IQueryable<T> QueryActiveVersions<T>(Language languageBranch) where T : Content;
    IEnumerable<T> Versions<T>(Guid contentId, Language language) where T : Content;
    void SetAsActiveVersion(Guid versionId);
}

public class ContentRepository : IContentRepository
{
    VersionedContentPOCContext _context;

    public ContentRepository(VersionedContentPOCContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Returns currently active version of content for language. Returns null if entity not found
    /// </summary>
    public T? Get<T>(Guid contentId, Language language) where T : Content 
    {
        return QueryActiveVersions<T>(language).FirstOrDefault(x => x.ContentId == contentId);
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

            if (!forceUpdate && languageBranch.ActiveVersionId != updatedVersion.VersionId)
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
    public IEnumerable<T> Versions<T>(Guid contentId, Language language) where T : Content
    {
        return _context.Content.OfType<T>()
            .Where(x => x.ContentId == contentId && x.Language == language)
            .Include(x => x.ContentRoot)
            .Include(x => x.LanguageBranch);
    }

    public void SetAsActiveVersion(Guid versionId)
    {
        var content = _context.Content
            .Where(x => x.VersionId == versionId)
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
            .Where(x => x.Language == language)
            .Include(x => x.LanguageBranch)
            .Where(x => x.LanguageBranch.ActiveVersionId == x.VersionId);
    }
}