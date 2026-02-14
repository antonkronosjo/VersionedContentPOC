using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using VersionedContentPOC.CMS.Attributes;
using VersionedContentPOC.CMS.Data.Enums;
using VersionedContentPOC.CMS.Data.Models;

namespace VersionedContentPOC.CMS.Extensions
{
    public static class IQueryableExtensions
    {
        /// <summary>
        /// Applies where statement if condition is true
        /// </summary>
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
        {
            return condition ? query.Where(predicate) : query;
        }

        /// <summary>
        /// Returns current representation of content. If content is published it returns published version, else it returns last created version
        /// </summary>
        [ShouldBeRefactored("DBR: Resolves wrong version if content is published but have new unpublished version")]
        public static IQueryable<T> ResolveContentVersions<T>(this IQueryable<T> query) where T : Content
        {
            return query.Where(x =>
                x.VersionId ==
                query
                    .Where(y => y.ContentId == x.ContentId)
                    .OrderByDescending(y =>
                        y.Status == PublishStatus.Published ? 1 : 0)   // Prefer published
                    .ThenByDescending(y => y.VersionCreated)           // Otherwise latest
                    .Select(y => y.VersionId)
                    .First()
            );
        }
    }
}
