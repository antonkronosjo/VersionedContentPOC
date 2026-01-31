using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using VersionedContentPOC.Data.Enums;
using VersionedContentPOC.Data.Models;
using VersionedContentPOC.Server.Attributes;
using VersionedContentPOC.Server.Controllers.Requests;
using VersionedContentPOC.Server.Data.Enums;
using VersionedContentPOC.Server.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace VersionedContentPOC.Server.Extensions
{
    public static class ContentFilterExtensions
    {
        public static IQueryable<T> FilterByContentQueryRequest<T>(this IQueryable<T> query, ContentQueryRequest contentFilterRequest) where T : Content
        {
            return query
                .FilterByLanguages(contentFilterRequest.Languages)
                .FilterByContentTypeNames(contentFilterRequest.ContentTypeNames)
                .FilterByPublishState(contentFilterRequest.PublishState);
        }

        public static IQueryable<T> FilterByLanguage<T>(this IQueryable<T> query, Language? language) where T : Content
        {
            if (language == null)
                return query;

            return query.Where(x => x.Language == language);
        }

        public static IQueryable<T> FilterByLanguages<T>(this IQueryable<T> query, List<Language>? languages) where T : Content
        {
            if (languages == null || !languages.Any())
                return query;

            return query.Where(x => languages.Contains(x.Language));
        }

        [ShouldBeRefactored("Since i use TPC discriminator filtering will not work")]
        public static IQueryable<T> FilterByContentTypeNames<T>(this IQueryable<T> query, List<string>? contentTypeNames) where T : Content
        {
            if (contentTypeNames == null || !contentTypeNames.Any())
                return query;

            return query.Where(c => contentTypeNames.Contains(EF.Property<string>(c, "Discriminator")));
        }

        public static IQueryable<T> FilterByPublishState<T>(this IQueryable<T> query, PublishState? publishState) where T : Content
        {
            if (publishState == null)
                return query;

            var utcNow = DateTime.UtcNow;

            switch (publishState)
            {
                case PublishState.Draft:
                    return query.Where(x => x.ContentRoot.StartPublish == null
                        && x.ContentRoot.StopPublish == null);
                case PublishState.Published:
                    return query.Where(x => x.ContentRoot.StartPublish < utcNow
                        && (x.ContentRoot.StopPublish == null || x.ContentRoot.StopPublish > utcNow));
                case PublishState.Unpublished:
                    return query.Where(x => x.ContentRoot.StopPublish < utcNow);
                default:
                    throw new NotImplementedException($"No filter added for publish state {publishState}");
            }
        }


        [ShouldBeRefactored("Have not implemented this yet")]
        public static IOrderedQueryable<T> OrderByContentQueryRequest<T>(this IQueryable<T> query, ContentQueryRequest contentFilterRequest) where T : Content
        {
            return query.OrderBy(x => x.ContentRoot.StartPublish);
            //    if (contentFilterRequest.Sort != null && contentFilterRequest.Sort.Any())
            //    {
            //        IOrderedQueryable<T>? sortedQuery = null;
            //        foreach (var sort in contentFilterRequest.Sort)
            //        {
            //            sortedQuery = sortedQuery == null
            //                ? (sort.Descending
            //                    ? query.OrderByDescending(e => EF.Property<object>(e, sort.Field))
            //                    : query.OrderBy(e => EF.Property<object>(e, sort.Field)))
            //                : (sort.Descending
            //                    ? sortedQuery.ThenByDescending(e => EF.Property<object>(e, sort.Field))
            //                    : sortedQuery.ThenBy(e => EF.Property<object>(e, sort.Field)));
            //        }

            //        if (sortedQuery != null)
            //            return sortedQuery;
            //    }
            //    return query;

            //    //// Paging
            //    //if (contentFilterRequest.Skip.HasValue)
            //    //    query = query.Skip(query.Skip.Value);

            //    //if (contentFilterRequest.Take.HasValue)
            //    //    query = query.Take(query.Take.Value);
            //}

            //private static SortByField<T>(T content, string field)
            //{
            //    sortedQuery = sortedQuery == null
            //    ? (sort.Descending
            //        ? query.OrderByDescending(e => EF.Property<object>(e, sort.Field))
            //        : query.OrderBy(e => EF.Property<object>(e, sort.Field)))
            //    : (sort.Descending
            //        ? sortedQuery.ThenByDescending(e => EF.Property<object>(e, sort.Field))
            //        : sortedQuery.ThenBy(e => EF.Property<object>(e, sort.Field)));
        }

        [ShouldBeRefactored("NOT IMPLEMENTED YET")]
        public static IQueryable<T> PaginateByContentQueryRequest<T>(this IQueryable<T> query, ContentQueryRequest request)
        {
            return query;
        }
    }
}


