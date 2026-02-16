using Microsoft.EntityFrameworkCore;
using VersionedContentPOC.CMS.Data;
using VersionedContentPOC.CMS.Data.Enums;
using VersionedContentPOC.CMS.Data.Models;
using VersionedContentPOC.CMS.Attributes;
using VersionedContentPOC.CMS.Extensions;

namespace VersionedContentPOC.CMS.Services;

public interface IContentRepository
{
    T? Get<T>(int contentId, Language language) where T : LocalizableVersion;
    bool Exists(int contentId);
    ContentRoot Get(int contentId);
    Type GetContentRootType(int contentId);
    T Create<T, T2>(T content, T2? sharedProperties = null) where T : LocalizableVersion where T2 : InvariantVersion;
    T Update<T>(int contentId, T contentVersion, bool forceNewVersion = false) where T : LocalizableVersion;
    T Update<T>(T content, IDictionary<string, ContentPropertyValueDto> updates, bool forceNewVersion = false) where T : LocalizableVersion;
    void Delete(int contentId);
    IQueryable<T> Query<T>(Language languageBranch) where T : LocalizableVersion;
    IQueryable<ContentRoot> QueryRoots();
    List<Language> GetTranslatedLanguages(int contentId);
}

internal class ContentRepository : IContentRepository
{
    CMSContext _context;
    IContentVersionRepository _contentVersionRepository;

    public ContentRepository(CMSContext context, IContentVersionRepository contentVersionRepository)
    {
        _context = context;
        _contentVersionRepository = contentVersionRepository;
    }

    /// <summary>
    /// Returns currently active version of content for language. Returns null if entity not found or not active
    /// </summary>
    public T? Get<T>(int contentId, Language language) where T : LocalizableVersion 
    {
        return Query<T>(language).FirstOrDefault(x => x.ContentId == contentId);
    }

    /// <summary>
    /// Returns true if content with contentid exists
    /// </summary>
    public bool Exists(int contentId)
    {
        return _context.ContentRoots.Any(x => x.ContentId == contentId);
    }

    /// <summary>
    /// Returns root content object
    /// </summary>
    public ContentRoot Get(int contentId)
    {
        return _context.ContentRoots
            .Where(x => x.ContentId == contentId)
            .Single();
    }

    [ShouldBeRefactored("To get type of ContentRoot should be done in a more eligant way")]
    public Type GetContentRootType(int contentId)
    {
        return _context.Content.First(x => x.ContentId == contentId).GetType();
    }

    /// <summary>
    /// Creates new content with underlying root object, language branch and version handling
    /// </summary>
    public T Create<T, T2>(T initialVersion, T2? sharedProperties = null)
        where T : LocalizableVersion 
        where T2 : InvariantVersion
    {
        using var transaction = _context.Database.BeginTransaction();

        var contentRoot = new ContentRoot();
        _context.Add(contentRoot);
        _context.SaveChanges();

        initialVersion.ContentId = contentRoot.ContentId;
        _context.Add(initialVersion);

        if (sharedProperties != null)
        {
            sharedProperties.ContentId = contentRoot.ContentId;
            _context.Add(sharedProperties);
        }

        _context.SaveChanges();

        transaction.Commit();
        return initialVersion;

    }

    /// <summary>
    /// Delets content and all versions of it
    /// </summary>
    public void Delete(int contentId)
    {
        var content = _context.ContentRoots.Where(x => x.ContentId == contentId);
        _context.Remove(content);
        _context.SaveChanges();
    }

    /// <summary>
    /// Updates content with new version. NOTE: Will throw exception if content.VersionId does not match currently active content version
    /// </summary>
    [ShouldBeRefactored("Refactor this so that it makes sense regarding force update")]
    public T Update<T>(int contentId, T updatedVersion, bool forceNewVersion = false) where T : LocalizableVersion
    {
        bool addNewVersion = forceNewVersion
            || updatedVersion.Status != PublishStatus.Draft
            || updatedVersion.VersionId == 0; //Not sure if this can happen
            
        if (addNewVersion)
        {
            return _contentVersionRepository.AddVersion(contentId, updatedVersion);
        }
            

        _context.Update(updatedVersion);
        _context.SaveChanges();

        return updatedVersion;
    }

    /// <summary>
    /// Updates content with new version based on key/values.
    /// </summary>
    public T Update<T>(T content, IDictionary<string, ContentPropertyValueDto> updates, bool forceNewVersion = false) where T : LocalizableVersion
    {
        //var entity = _context.Entities.Find(id);
        //_context.Entry(content).State = EntityState.Detached; //Need to detach state before applying updates
        ContentUpdater.ApplyUpdates(content, updates);
        return Update<T>(content.ContentId, content);
    }

    /// <summary>
    /// Returns a base query used when querying content
    /// </summary>
    [ShouldBeRefactored("DBR: Look over published version filter")]
    public IQueryable<T> Query<T>(Language language) where T : LocalizableVersion
    {
        return _context.Content.OfType<T>()
            .Where(x => x.Language == language)
            .ResolveContentVersions()
            .Include(x => x.ContentRoot)
            .ThenInclude(x => x.SharedContentProperties);
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
    public List<Language> GetTranslatedLanguages(int contentId)
    {
        return _context.ContentRoots
            .Include(x => x.Versions)
            .Single(x => x.ContentId == contentId)
            .Versions
            .GroupBy(x => x.Language)
            .Select(x => x.Key)
            .ToList();
    }
}