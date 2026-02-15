using Microsoft.EntityFrameworkCore;
using VersionedContentPOC.CMS.Data;
using VersionedContentPOC.CMS.Data.Enums;
using VersionedContentPOC.CMS.Data.Interfaces;
using VersionedContentPOC.CMS.Data.Models;
namespace VersionedContentPOC.CMS.Services;

public interface IContentVersionRepository
{
    T AddVersion<T>(int contentId, T version) where T : Content, IVersionable;
    T GetVersion<T>(int contentId, int versionId, Language language) where T : Content;
    IQueryable<T> QueryVersions<T>(Language language) where T : Content;
    T GetVersion<T>(int versionId) where T : Content;


}

internal class ContentVersionRepository : IContentVersionRepository
{
    CMSContext _context;

    public ContentVersionRepository(CMSContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Adds new version to content + updates non cultural specific properties on other language branches
    /// </summary>
    public T AddVersion<T>(int contentId, T version) where T: Content, IVersionable
    {
        version.ContentId = contentId;
        version.VersionId = 0;
        version.VersionCreated = DateTime.UtcNow;

        if (version is IPublishable publishable)
        {
            publishable.StartPublish = null;
            publishable.StopPublish = null;
            publishable.Status = PublishStatus.Draft;
        }

        _context.Add(version);
        _context.SaveChanges();

        return version;
    }

    /// <summary>
    /// Queries all version 
    /// </summary>
    public IQueryable<T> QueryVersions<T>(Language language) where T : Content
    {
        return _context.Content.OfType<T>()
            .Where(x => x.Language == language)
            .Include(x => x.ContentRoot);
    }

    /// <summary>
    /// Returns version of content for language. Returns null if not found,
    /// </summary>
    public T GetVersion<T>(int contentId, int versionId, Language language) where T : Content
    {
        return QueryVersions<T>(language)
            .Single(x => x.ContentId == contentId && x.VersionId == versionId);
    }

    public T GetVersion<T>(int versionId) where T : Content
    {
        return _context.Content.OfType<T>().Single(x => x.VersionId == versionId);
    }
}