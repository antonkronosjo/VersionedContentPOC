using System.ComponentModel.DataAnnotations;
using VersionedContentPOC.Attributes;
using VersionedContentPOC.Data.Enums;
using VersionedContentPOC.Data.Models;
using VersionedContentPOC.Server.Data.Enums;

namespace VersionedContentPOC.Server.Data.Models
{
    [ContentType]
    public class EventContent : Content
    {
        public EventContent(Guid versionId, Language language) : base(versionId, language)
        {
            
        }

        [ContentPropertyMetadata(editable: true)]
        [Required]
        public required string Heading { get; set; }

        [ContentPropertyMetadata(editable: true, inputType: InputType.DateTimePicker)]
        [Required]
        public required DateTime StartDate { get; set; }

        [ContentPropertyMetadata(editable: true, inputType: InputType.DateTimePicker)]
        [Required]
        public required DateTime EndDate { get; set; }

        [ContentPropertyMetadata(editable: true, inputType: InputType.TextArea)]
        [Required]
        public required string Description { get; set; }
    }
}
