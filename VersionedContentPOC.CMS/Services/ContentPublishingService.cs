using Microsoft.EntityFrameworkCore;
using VersionedContentPOC.CMS.Attributes;
using VersionedContentPOC.CMS.Data;
using VersionedContentPOC.CMS.Data.Enums;
using VersionedContentPOC.CMS.Data.Models;
using VersionedContentPOC.CMS.Extensions;

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

        using var transaction = _context.Database.BeginTransaction();
        var currentlyPublishedVersion = _context.Content.FirstOrDefault(x => x.ContentId == content.ContentId && x.Status == PublishStatus.Published);
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
        UpdateMainLanguageOnlyValues(content);
        _context.SaveChanges();
        transaction.Commit();
    }

    [ShouldBeRefactored("This does not take in consideration if content has been unpublished on main language or similar")]
    public void Unpublish(Content content)
    {
        using var transaction = _context.Database.BeginTransaction();
        content.StopPublish = DateTime.UtcNow;
        content.Status = PublishStatus.Unpublished;
        _context.SaveChanges();

        var root =_context.ContentRoots.Include(x => x.Versions).Single(x => x.ContentId == content.ContentId);
        var publishedOrLastCreatedVersion = root.Versions.GetCurrentlyPublishedOrLastCreated();
        if (publishedOrLastCreatedVersion == null)
            throw new Exception("No published or last created version exist!");

        UpdateMainLanguageOnlyValues(publishedOrLastCreatedVersion);
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

    private void UpdateMainLanguageOnlyValues(Content mainLanguageVersion)
    {
        var root = _context.ContentRoots
            .Include(x => x.Versions)
            .Single(x => x.ContentId == mainLanguageVersion.ContentId);

        if (root.MainLanguage == mainLanguageVersion.Language)
        {
            foreach (var version in root.Versions.Where(x => x.Language != root.MainLanguage))
            {
                CopyMainLanguageValues(version, mainLanguageVersion);
                _context.Update(version);
            }
        }
    }

    private static void CopyMainLanguageValues<T>(T target, T source) where T : Content
    {
        if (target.GetType() != source.GetType()) throw new ArgumentException("Types differ!");

        var properties = source
            .GetType()
            .GetContentProperties()
            .FilterByAttribute<MainLanguageOnlyAttribute>(x => x?.IsActive == true);

        foreach (var p in properties)
            p.SetValue(target, p.GetValue(source));
    }
}
