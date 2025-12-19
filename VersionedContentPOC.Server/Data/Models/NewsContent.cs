using VersionedContentPOC.Attributes;
using VersionedContentPOC.Data.Enums;
using System.Text.Json.Serialization;

namespace VersionedContentPOC.Data.Models
{
    [JsonDerivedType(typeof(NewsContent), nameof(NewsContent))]
    [ContentType(ContentType.News)]
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
