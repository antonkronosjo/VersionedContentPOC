using VersionedContentPOC.CMS.Attributes;
using VersionedContentPOC.CMS.Data.Enums;
using VersionedContentPOC.CMS.Data.Models;

namespace VersionedContentPOC.CMS.Extensions
{
    internal static class ContentExtensions
    {
        public static bool IsPublished(this Content content)
        {
            var utcNow = DateTime.UtcNow;
            return content.StartPublish < utcNow && (content.StopPublish == null || content.StopPublish < utcNow);
        }

        [ShouldBeRefactored("This maybe should return latest published version if no version is published? IDK")]
        public static Content ResolveContentETC(this IEnumerable<Content> contentList)
        {
            var publishedContent = contentList.Where(x => x.Status == PublishStatus.Published).SingleOrDefault();
            if (publishedContent != null)
                return publishedContent;

            return contentList.OrderByDescending(x => x.VersionCreated).First();
        }
    }
}
