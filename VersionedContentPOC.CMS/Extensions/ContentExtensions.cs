using VersionedContentPOC.CMS.Attributes;
using VersionedContentPOC.CMS.Data.Enums;
using VersionedContentPOC.CMS.Data.Models;

namespace VersionedContentPOC.CMS.Extensions
{
    internal static class ContentExtensions
    {
        [ShouldBeRefactored("This maybe should return latest published version if no version is published before fallbacking to last created version? IDK")]
        public static Content? GetCurrentlyPublishedOrLastCreated(this IEnumerable<Content> contentList)
        {
            var publishedContent = contentList.Where(x => x.Status == PublishStatus.Published).SingleOrDefault();
            if (publishedContent != null)
                return publishedContent;

            return contentList.OrderByDescending(x => x.VersionCreated).FirstOrDefault();
        }
    }
}
