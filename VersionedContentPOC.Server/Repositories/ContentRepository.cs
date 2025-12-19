using VersionedContentPOC.Data;
using VersionedContentPOC.Data.Enums;
using VersionedContentPOC.Data.Extensions;
using VersionedContentPOC.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace VersionedContentPOC.Repositories;

public interface IContentRepository
{
    T? Get<T>(Guid contentId, Language language) where T : Content;
    T Create<T>(T content) where T : Content;
    T Update<T>(Guid contentId, T contentVersion) where T : Content;
    T Update<T>(Guid contentId, Language language, Dictionary<string, string?> updates) where T : Content;
    void Delete(Guid contentId);
    IQueryable<T> QueryActiveVersions<T>(Language languageBranch) where T : Content;
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
    /// Creates new content with underlying root object, language branch and version handeling
    /// </summary>
    public T Create<T>(T initialVersion) where T : Content
    {
        using var transaction = _context.Database.BeginTransaction();
        try
        {
            var contentRoot = new ContentRoot(Guid.NewGuid());
            _context.Add(contentRoot);

            var languageBranch = contentRoot.AddNewLanguageBranch(initialVersion.Language);
            _context.Add(languageBranch);
            _context.SaveChanges();

            initialVersion.ContentId = contentRoot.ContentId;
            languageBranch.AddVersion(initialVersion);
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
        var content = _context.Content.Single(x => x.ContentId == contentId);
        _context.Remove(content);
        _context.SaveChanges();
    }

    /// <summary>
    /// Updates content with new version
    /// </summary>
    public T Update<T>(Guid contentId, T updatedVersion) where T : Content
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
    /// Updates content with new version based on key/values 
    /// </summary>
    public T Update<T>(Guid contentId, Language language, Dictionary<string, string?> updates) where T : Content
    {
        var activeVersion = Get<T>(contentId, language);
        if (activeVersion == null)
            throw new InvalidOperationException($"Content with id {contentId} does not exist for language {language}");

        var updatedVersion = activeVersion.ApplyUpdates(updates);
        updatedVersion.VersionId = Guid.NewGuid();

        return Update(contentId, updatedVersion);
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
