using VersionedContentPOC.CMS.Data.Enums;

namespace VersionedContentPOC.CMS.Requests
{
    public class GetRootSummariesRequest
    {
        public string? ContentType { get; set; }
        public bool? Published { get; set; }
        public Language? Language { get; set; }
    }
}
