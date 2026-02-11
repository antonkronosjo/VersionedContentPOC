using Microsoft.EntityFrameworkCore;
using VersionedContentPOC.CMS.Attributes;
using VersionedContentPOC.CMS.Data;
using VersionedContentPOC.CMS.Data.Enums;
using VersionedContentPOC.CMS.Data.Models;
using VersionedContentPOC.CMS.Extensions;

namespace VersionedContentPOC.CMS.Services;

public interface IContentVersionRepository
{
    T AddVersion<T>(int contentId, T version) where T : Content;
    T? GetVersion<T>(int contentId, int versionId, Language language) where T : Content;
    IQueryable<T> QueryVersions<T>(Language language) where T : Content;
    
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
    [ShouldBeRefactored("DBR: Need to look over this")]
    public T AddVersion<T>(int contentId, T version) where T : Content
    {
        using var transaction = _context.Database.BeginTransaction();

        var root = _context.ContentRoots
            .Include(x => x.Versions)
            .Single(r => r.ContentId == contentId);

        var activeMainLanguageVersion = root.Versions.ResolveContentETC();

        //Content has no MainLanguageOnly-properties => Just add new version
        if (NoMainLanguageOnlyProperties(version))
        {
            InternalAddVersion(contentId, version);
            transaction.Commit();
            return version;
        }

        //Content has no MainLanguageOnly-properties changed => Just add new version
        if (HasMainLanguageChanges(activeMainLanguageVersion, version) == false)
        {
            InternalAddVersion(contentId, version);
            transaction.Commit();
            return version;
        }

        //If main language is updated, update all existing versions of other languages
        if (root.MainLanguage == version.Language)
        {
            UpdateMainLanguageOnlyValues(root, version);
        }
        //If not main language is updated => Map MainLanguageOnly-properties to the new version from current Active
        else
        {
            CopyMainLanguageValues(version, activeMainLanguageVersion);
        }

        InternalAddVersion(contentId, version);
        transaction.Commit();
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
    public T? GetVersion<T>(int contentId, int versionId, Language language) where T : Content
    {
        return QueryVersions<T>(language)
            .FirstOrDefault(x => x.ContentId == contentId && x.VersionId == versionId);
    }

    [ShouldBeRefactored("DBR: Need to look over this")]
    private void UpdateMainLanguageOnlyValues<T>(ContentRoot contentRoot, T publishedMainLanguageVersion) where T : Content
    {
        foreach (var version in contentRoot.Versions.Where(x => x.Language != contentRoot.MainLanguage))
        {
            CopyMainLanguageValues(version, publishedMainLanguageVersion);
            _context.Update(version);
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

    private static bool NoMainLanguageOnlyProperties(Content content)
    {
        return content.GetType()
            .GetContentProperties()
            .FilterByAttribute<MainLanguageOnlyAttribute>(x => x?.IsActive == true)
            .Any() == false;
    }

    private static bool HasMainLanguageChanges<T>(T versionA, T versionB) where T : Content
    {
        if (versionA.GetType() != versionB.GetType()) throw new ArgumentException("Types differ!");

        var properties = versionA
            .GetType()
            .GetContentProperties()
            .FilterByAttribute<MainLanguageOnlyAttribute>(x => x?.IsActive == true);

        return properties.Any(p => !Equals(p.GetValue(versionA), p.GetValue(versionB)));
    }

    private T InternalAddVersion<T>(int contentId, T version) where T : Content
    {
        version.ContentId = contentId;
        version.VersionId = 0;
        version.VersionCreated = DateTime.UtcNow;
        _context.Add(version);
        _context.SaveChanges();
        return version;
    }
}