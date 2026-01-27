using Microsoft.EntityFrameworkCore;
using System.Reflection;
using VersionedContentPOC.Attributes;
using VersionedContentPOC.Data;
using VersionedContentPOC.Data.Models;
using VersionedContentPOC.Server.Attributes;

namespace VersionedContentPOC.Server.Services;

public interface IContentVersionRepository
{
    T AddVersion<T>(Guid contentId, T version, bool forceUpdate = false) where T : Content;
    void SetAsActiveVersion(Guid versionId);
}

public class ContentVersionRepository : IContentVersionRepository
{
    VersionedContentPOCContext _context;

    public ContentVersionRepository(VersionedContentPOCContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Adds new version to content + updates non cultural specific properties on other language branches
    /// </summary>
    public T AddVersion<T>(Guid contentId, T version, bool forceUpdate = false) where T : Content
    {
        using var transaction = _context.Database.BeginTransaction();
        try
        {
            //_context.Entry(version).State = EntityState.Detached;
            var root = _context.ContentRoots
                .Include(r => r.LanguageBranches)
                    .ThenInclude(m => m.Versions)
                .Include(x => x.LanguageBranches)
                    .ThenInclude(x => x.ActiveVersion)
                .Single(r => r.ContentId == contentId);

            var activeMainLanguageVersion = root.LanguageBranches
                .Where(x => x.Language == root.MainLanguage)
                .Single().ActiveVersion;

            if (activeMainLanguageVersion == null)
                throw new Exception("Something went wrong");

            //Content has no MainLanguageOnly-properties => Just add new version
            if (NoMainLanguageOnlyProperties(version))
            {
                InternalAddVersion(contentId, version, forceUpdate);
                transaction.Commit();
                return version;
            }
                
            //Content has no MainLanguageOnly-properties changed => Just add new version
            if (activeMainLanguageVersion.HasAnyMainLanguagePropertyDifference(version) == false)
            {
                InternalAddVersion(contentId, version, forceUpdate);
                transaction.Commit();
                return version;
            }
                
            //If main language is updated, update all existing versions of other languages
            if (root.MainLanguage == version.Language)
            {
                UpdateMainLanguageOnlyValues(root, version);
                InternalAddVersion(contentId, version, forceUpdate);
                transaction.Commit();
                return version;
            }

            //If not main language is updated => Map MainLanguageOnly-properties to the new version from current Active
            else
            {
                ContentVersionExtensions.MapMainLanguageOnlyValues(version, activeMainLanguageVersion);
                InternalAddVersion(contentId, version, forceUpdate);
                transaction.Commit();
                return version;
            }
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public void SetAsActiveVersion(Guid versionId)
    {
        using var transaction = _context.Database.BeginTransaction();
        try
        {
            var version = _context.Content
                .Where(x => x.VersionId == versionId)
                .Include(x => x.ContentRoot)
                .Single();

            //Content has no MainLanguageOnly-properties => No need to change values in other versions => Set as Active
            if (NoMainLanguageOnlyProperties(version))
            {
                version.LanguageBranch.SetActiveVersion(version);
                _context.Update(version.LanguageBranch);
                _context.SaveChanges();
                transaction.Commit();
                return;
            }

            var root = _context.ContentRoots
                .Include(r => r.LanguageBranches)
                    .ThenInclude(m => m.Versions)
                .Include(x => x.LanguageBranches)
                    .ThenInclude(x => x.ActiveVersion)
                .Single(r => r.ContentId == version.ContentId);

            var activeMainLanguageVersion = root.LanguageBranches
                .Where(x => x.Language == root.MainLanguage)
                .Single().ActiveVersion;

            if (activeMainLanguageVersion == null)
                throw new Exception("Something went wrong");

            //Content has no changes on MainLanguageOnly-properties => Just add new version
            if (activeMainLanguageVersion.HasAnyMainLanguagePropertyDifference(version) == false)
            {
                version.LanguageBranch.SetActiveVersion(version);
                _context.Update(version.LanguageBranch);
                _context.SaveChanges();
                transaction.Commit();
                return;
            }

            //Is not main language change => Update non-cultural-specific properties on the chosen version
            if (version.ContentRoot.MainLanguage != version.Language)
            {
                ContentVersionExtensions.MapMainLanguageOnlyValues(version, activeMainLanguageVersion);
                _context.Update(version);
                version.LanguageBranch.SetActiveVersion(version);
                _context.Update(version.LanguageBranch);
                _context.SaveChanges();
                transaction.Commit();
                return;
            }
            //
            else
            {
                UpdateMainLanguageOnlyValues(root, version);
                version.LanguageBranch.SetActiveVersion(version);
                _context.SaveChanges();
                transaction.Commit();
            }
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            throw;
        }
    }

    private void UpdateMainLanguageOnlyValues<T>(ContentRoot contentRoot, T activeMainLanguageVersion) where T : Content
    {
        foreach (var languageBranch in contentRoot.LanguageBranches.Where(x => x.Language != contentRoot.MainLanguage))
        {
            var activeVersion = languageBranch.ActiveVersion;
            if (activeVersion == null)
                throw new Exception("Something went wrong");

            foreach (var languageVersion in languageBranch.Versions)
            {
                ContentVersionExtensions.MapMainLanguageOnlyValues(languageVersion, activeMainLanguageVersion);
                _context.Update(languageVersion);
            }
        }
    }

    private static bool NoMainLanguageOnlyProperties(Content content)
    {
        return content.GetContentProperties().FilterByMainLanguageOnly().Any() == false;
    }

    public T InternalAddVersion<T>(Guid contentId, T version, bool forceUpdate = false) where T : Content
    {
        var root = _context.ContentRoots
               .Include(r => r.LanguageBranches)
                   .ThenInclude(m => m.Versions)
               .Single(r => r.ContentId == contentId);

        var newLanguageBranch = root.AddNewLanguageBranchIfNotExist(version.Language);
        if (newLanguageBranch != null)
        {
            _context.Add(newLanguageBranch);
            _context.SaveChanges();
        }

        var languageBranch = newLanguageBranch ?? root.LanguageBranches.Single(x => x.Language == version.Language);

        if (!forceUpdate && languageBranch.ActiveVersionId != null && languageBranch.ActiveVersionId != version.VersionId)
            throw new Exception("Content.VersionId does not match the current one being active. Use forceUpdate=true to save");
        else
        {
            version.VersionId = Guid.NewGuid();
            version.VersionCreated = DateTime.UtcNow;
        }


        languageBranch.AddVersion(version);

        _context.Update(languageBranch);
        _context.Add(version);
        _context.SaveChanges();
        return version;
    }
}

public static class ContentVersionExtensions
{
    public static IEnumerable<PropertyInfo> GetContentProperties(this Content content)
    {
        return content.GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(x => x.CanRead && x.IsDefined(typeof(ContentPropertyMetadataAttribute), true));
    }

    public static IEnumerable<PropertyInfo> FilterByMainLanguageOnly(this IEnumerable<PropertyInfo> propertyInfos)
    {
        return propertyInfos
                .Where((x) => x
                    .GetCustomAttribute<MainLanguageOnlyAttribute>(inherit: true)
                        ?.IsActive == true);
    }

    public static bool HasAnyMainLanguagePropertyDifference<T>(this T versionA, T versionB) where T : Content
    {
        if (versionA == null) throw new ArgumentNullException(nameof(versionA));
        if (versionB == null) throw new ArgumentNullException(nameof(versionB));
        if (versionA.GetType() != versionB.GetType()) throw new ArgumentException("Types differ!");

        var properties = versionA
            .GetContentProperties()
            .FilterByMainLanguageOnly();

        foreach (var property in properties)
        {
            var valueA = property.GetValue(versionA);
            var valueB = property.GetValue(versionB);

            if (!Equals(valueA, valueB))
            {
                return true;
            }
        }

        return false;
    }

    public static void MapMainLanguageOnlyValues<T>(T targetContent, T mainContent) where T : Content
    {
        if (targetContent.GetType() != mainContent.GetType()) throw new ArgumentException("Types differ!");

        var properties = mainContent
            .GetContentProperties()
            .FilterByMainLanguageOnly();

        foreach (var property in properties)
        {
            var value = property.GetValue(mainContent);
            property.SetValue(targetContent, value);
        }
    }
}
