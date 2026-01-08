using VersionedContentPOC.Attributes;
using VersionedContentPOC.Data.Enums;
using VersionedContentPOC.Data.Models;

namespace VersionedContentPOC.Server.Data.Models
{
    [ContentType]
    public class EventContent : Content
    {
        public EventContent(Guid versionId, Language language) : base(versionId, language)
        {
            
        }

        [ContentPropertyMetadata(editable: true)]
        public required string Heading { get; set; }

        [ContentPropertyMetadata(editable: true)]
        public required DateTime StartDate { get; set; }

        [ContentPropertyMetadata(editable: true)]
        public required DateTime EndDate { get; set; }
    }
}
