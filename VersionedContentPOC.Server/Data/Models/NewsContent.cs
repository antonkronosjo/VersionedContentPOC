using VersionedContentPOC.Attributes;
using VersionedContentPOC.Data.Enums;
using VersionedContentPOC.Data.Models;
using VersionedContentPOC.Server.Data.Enums;

namespace VersionedContentPOC.Server.Data.Models
{
    [ContentType]
    public class NewsContent : Content
    {
        public NewsContent(Guid versionId, Language language) : base(versionId, language)
        {
            
        }

        [ContentPropertyMetadata(editable: true)]
        public required string Heading { get; set; }

        [ContentPropertyMetadata(editable: true, inputType: InputType.TextArea)]
        public string? Lead { get; set; }

        [ContentPropertyMetadata(editable: true, inputType: InputType.TextArea)]
        public required string Text { get; set; }
    }
}
