using Microsoft.EntityFrameworkCore;
using VersionedContentPOC.Data.Enums;
using VersionedContentPOC.Data.Models;

namespace VersionedContentPOC.Server.Extensions
{
    public static class ContentQueryExtensions
    {
        //public static IQueryable<T> WhereActive<T>(this IQueryable<T> query) where T : Content
        //{
        //    return query
        //        .Include(x => x.LanguageBranch)
        //        .Where(x => x.LanguageBranch.ActiveVersionId == x.VersionId);
        //}

        //public static IQueryable<T> WhereLanguage<T>(this IQueryable<T> query, Language language) where T : Content
        //{
        //    return query.Where(x => x.Language == language);
        //}

        //public static IQueryable<T> WhereVersion<T>(this IQueryable<T> query, Guid versionId) where T : Content
        //{
        //    return query.Where(x => x.VersionId == versionId);
        //}
    }
}
