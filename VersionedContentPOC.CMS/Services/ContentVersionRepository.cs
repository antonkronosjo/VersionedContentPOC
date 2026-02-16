using Microsoft.EntityFrameworkCore;
using VersionedContentPOC.CMS.Data;
using VersionedContentPOC.CMS.Data.Enums;
using VersionedContentPOC.CMS.Data.Interfaces;
using VersionedContentPOC.CMS.Data.Models;
namespace VersionedContentPOC.CMS.Services;

public interface IContentVersionRepository
{
    T AddVersion<T>(int contentId, T version) where T : ContentVersion;
    IQueryable<T> QueryVersions<T>() where T : ContentVersion;
    T GetVersion<T>(int versionId) where T : ContentVersion;
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
    public T AddVersion<T>(int contentId, T version) where T: ContentVersion
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

    public IQueryable<T> QueryVersions<T>() where T : ContentVersion
    {
        return _context.Set<T>()
            .Include(x => x.ContentRoot);
    }

    public T GetVersion<T>(int versionId) where T : ContentVersion
    {
        return QueryVersions<T>()
            .Single(x => x.VersionId == versionId);
    }
}