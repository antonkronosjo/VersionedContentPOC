using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersionedContentPOC.CMS.Data;
using VersionedContentPOC.CMS.Data.Enums;
using VersionedContentPOC.CMS.Data.Models;

namespace VersionedContentPOC.CMS.Services;

public interface IContentPublishingService
{
    void Publish(int versionId);
    void Unpublish(int versionId);
    void Publish(Content content);
    void Unpublish(Content content);
}
internal class ContentPublishingService : IContentPublishingService
{
    CMSContext _context;
    IContentVersionRepository _contentVersionRepository;

    public ContentPublishingService(CMSContext context, IContentVersionRepository contentVersionRepository)
    {
        _context = context;
        _contentVersionRepository = contentVersionRepository;
    }

    public void Publish(Content content) 
    {
        if (content.Status == PublishStatus.Published)
            throw new Exception("Content already published!");

        var alreadyPublishedContent = _context.Content.FirstOrDefault(x => x.ContentId == content.ContentId && x.Status == PublishStatus.Published);
        if (alreadyPublishedContent != null)
        {
            alreadyPublishedContent.Status = PublishStatus.Unpublished;
            alreadyPublishedContent.StopPublish = DateTime.UtcNow;
            _context.Update(alreadyPublishedContent);
        }

        content.StartPublish = DateTime.UtcNow;
        content.StopPublish = null;
        content.Status = PublishStatus.Published;
        _context.Update(content);
        _context.SaveChanges();
    }

    public void Publish(int versionId)
    {
        var content = _context.Content.Single(x => x.VersionId == versionId);
        Publish(content);
    }

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

    public void DelayedPublish(Content contentDraft)
    {
        throw new NotImplementedException();
    }
}
