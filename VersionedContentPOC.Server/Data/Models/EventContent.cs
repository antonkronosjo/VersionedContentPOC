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

        public required string Heading { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
    }
}
