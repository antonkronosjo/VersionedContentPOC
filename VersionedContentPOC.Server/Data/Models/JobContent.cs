using System.ComponentModel.DataAnnotations;
using VersionedContentPOC.CMS.Data.Models;
using VersionedContentPOC.CMS.Data.Enums;
using VersionedContentPOC.CMS.Attributes;

namespace VersionedContentPOC.Server.Data.Models
{
    [ContentType]
    public class JobContent : Content
    {
        public JobContent(Language language) : base(language)
        {
            
        }

        [ContentPropertyMetadata(editable: true)]
        [Required]
        public required string Heading { get; set; }

        [ContentPropertyMetadata(editable: true)]
        [Required]
        public required string Lead { get; set; }

        [ContentPropertyMetadata(editable: true)]
        [Required]
        public required string Department { get; set; }

        [ContentPropertyMetadata(editable: true)]
        [Required]
        public string TextBody { get; set; }

        [ContentPropertyMetadata(editable: true)]
        [Required]
        public string WorkDescription { get; set; }

        [ContentPropertyMetadata(editable: true)]
        [Required]
        public string Requirements { get; set; }

        [ContentPropertyMetadata(editable: true, inputType: InputType.DateTimePicker)]
        [Required]
        [MainLanguageOnly]
        public required DateTime ApplicationEndDate { get; set; }
    }
}
