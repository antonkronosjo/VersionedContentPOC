using VersionedContentPOC.CMS.Attributes;
using VersionedContentPOC.CMS.Data;
using VersionedContentPOC.CMS.Data.Enums;
using VersionedContentPOC.CMS.Data.Interfaces;

namespace VersionedContentPOC.CMS.Services;

public interface IContentPublishingService
{
    void Publish(IPublishable content);
    void Unpublish(IPublishable content);
}
internal class ContentPublishingService : IContentPublishingService
{
    CMSContext _context;

    public ContentPublishingService(CMSContext context)
    {
        _context = context;
    }

    [ShouldBeRefactored("Does not commit publish/unpublish in one transaction")]
    public void Publish(IPublishable content) 
    {
        if (content.Status == PublishStatus.Published)
            throw new Exception("Content already published!");

        var currentlyPublishedVersion = _context.Content.FirstOrDefault(x =>
            x.ContentId == content.ContentId
            && x.Status == PublishStatus.Published
        );

        var isOnSameLanguage = (currentlyPublishedVersion as ILocalizable)?.Language == (content as ILocalizable)?.Language;

        if (currentlyPublishedVersion != null && isOnSameLanguage)
            Unpublish(currentlyPublishedVersion);

        content.StartPublish = DateTime.UtcNow;
        content.StopPublish = null;
        content.Status = PublishStatus.Published;
        _context.Update(content);
        
        _context.SaveChanges();
    }

    public void Unpublish(IPublishable content)
    {
        content.StopPublish = DateTime.UtcNow;
        content.Status = PublishStatus.Unpublished;
        _context.SaveChanges();
    }
}
