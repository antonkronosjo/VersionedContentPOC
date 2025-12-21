using VersionedContentPOC.Attributes;
using VersionedContentPOC.Data.Enums;
using VersionedContentPOC.Data.Models;

namespace VersionedContentPOC.Server.Data.Models
{
    [ContentType]
    public class NewsContent : Content
    {
        public NewsContent(Guid versionId, Language language) : base(versionId, language)
        {
            
        }

        [ContentPropertyMetaData(editable: true)]
        public required string Heading { get; set; }

        [ContentPropertyMetaData(editable: true)]
        public string? Lead { get; set; }

        [ContentPropertyMetaData(editable: true)]
        public required string Text { get; set; }
    }
}
