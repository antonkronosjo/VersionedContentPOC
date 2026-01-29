using System.ComponentModel.DataAnnotations;
using VersionedContentPOC.Attributes;
using VersionedContentPOC.Data.Enums;
using VersionedContentPOC.Data.Models;
using VersionedContentPOC.Server.Data.Enums;

namespace VersionedContentPOC.Server.Data.Models
{
    [ContentType]
    public class NewsContent : Content
    {
        public NewsContent(Language language) : base(language)
        {
            
        }

        [ContentPropertyMetadata(editable: true)]
        [Required]
        public required string Heading { get; set; }

        [ContentPropertyMetadata(editable: true, inputType: InputType.TextArea)]
        public string? Lead { get; set; }

        [ContentPropertyMetadata(editable: true, inputType: InputType.TextArea)]
        [Required]
        public required string Text { get; set; }
    }
}
