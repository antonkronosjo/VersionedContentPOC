using VersionedContentPOC.CMS.Attributes;
using VersionedContentPOC.CMS.Data.Enums;
using VersionedContentPOC.CMS.Data.Models;
using VersionedContentPOC.CMS.Services;

namespace VersionedContentPOC.CMS.Mappers
{
    public static class ContentRootSummaryMapper
    {
        [ShouldBeRefactored("ContentTypeName should be stored in the database somehow. Don't know how")]
        [ShouldBeRefactored("DBR: Look over stop/start publish")]
        public static IEnumerable<ContentRootSummary> ToSummary(this IEnumerable<ContentRoot> contentRoots, IContentRepository contentRepository)
        {
            return contentRoots.Select(x => new ContentRootSummary()
            {
                ContentId = x.ContentId,
                ContentTypeName = contentRepository
                    .GetContentRootType(x.ContentId)
                    .Name,
                Created = x.Created,
                StartPublish = null,//x.StartPublish,
                StopPublish = null,//x.StopPublish,
                LanguageVersions = x.Versions
                    .GroupBy(x => x.Language)
                    .Select(x => x.Key)
                    .ToList(),
                LastUpdated = x.Versions
                    .OrderByDescending(x => x.VersionCreated)
                    .FirstOrDefault()
                    ?.VersionCreated
            });
        }
    }

    public class ContentRootSummary
    {
        public required int ContentId { get; set; }
        public required string ContentTypeName { get; set; }
        public required DateTime? StartPublish {  get; set; }
        public required DateTime? StopPublish { get; set; }
        public required List<Language> LanguageVersions { get; set; }
        public required DateTime? Created { get; set; }
        public required DateTime? LastUpdated { get; set; }
    }
}
