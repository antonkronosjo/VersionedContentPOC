using System.ComponentModel.DataAnnotations;
using VersionedContentPOC.CMS.Data.Models;
using VersionedContentPOC.CMS.Data.Enums;
using VersionedContentPOC.CMS.Attributes;

namespace VersionedContentPOC.Server.Data.Models
{
    [ContentType]
    public class EventContent : Content
    {
        public EventContent(Language language) : base(language)
        {
            
        }

        [ContentPropertyMetadata(editable: true)]
        [Required]
        public required string Heading { get; set; }

        [ContentPropertyMetadata(editable: true, inputType: InputType.DateTimePicker)]
        [Required]
        [MainLanguageOnly]
        public required DateTime StartDate { get; set; }

        [ContentPropertyMetadata(editable: true, inputType: InputType.DateTimePicker)]
        [Required]
        [MainLanguageOnly]
        public required DateTime EndDate { get; set; }

        [ContentPropertyMetadata(editable: true, inputType: InputType.TextArea)]
        [Required]
        public required string Description { get; set; }
    }
}
