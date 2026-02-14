using VersionedContentPOC.CMS.Attributes;
using VersionedContentPOC.CMS.Data;
using VersionedContentPOC.CMS.Data.Enums;
using VersionedContentPOC.CMS.Data.Models;

namespace VersionedContentPOC.CMS.Services;

public interface IContentPublishingService
{
    void Publish(int versionId);
    void Publish(Content content);
    void Unpublish(int versionId);
    void Unpublish(Content content);
}
internal class ContentPublishingService : IContentPublishingService
{
    CMSContext _context;

    public ContentPublishingService(CMSContext context)
    {
        _context = context;
    }

    public void Publish(int versionId)
    {
        var content = _context.Content.Single(x => x.VersionId == versionId);
        Publish(content);
    }

    public void Publish(Content content) 
    {
        if (content.Status == PublishStatus.Published)
            throw new Exception("Content already published!");

        var currentlyPublishedVersion = _context.Content.FirstOrDefault(x =>
            x.ContentId == content.ContentId
            && x.Status == PublishStatus.Published
        );

        if (currentlyPublishedVersion != null)
        {
            currentlyPublishedVersion.Status = PublishStatus.Unpublished;
            currentlyPublishedVersion.StopPublish = DateTime.UtcNow;
            _context.Update(currentlyPublishedVersion);
        }

        content.StartPublish = DateTime.UtcNow;
        content.StopPublish = null;
        content.Status = PublishStatus.Published;
        _context.Update(content);
        
        _context.SaveChanges();
    }

    [ShouldBeRefactored("This does not take in consideration if content has been unpublished on main language or similar")]
    public void Unpublish(Content content)
    {
        content.StopPublish = DateTime.UtcNow;
        content.Status = PublishStatus.Unpublished;
        _context.SaveChanges();
    }

    public void Unpublish(int versionId)
    {
        var content = _context.Content.Single(x => x.VersionId == versionId);
        Unpublish(content);
    }
}
