using System.ComponentModel.DataAnnotations;
using VersionedContentPOC.CMS.Data.Models;
using VersionedContentPOC.CMS.Data.Enums;
using VersionedContentPOC.CMS.Attributes;

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

        [ContentPropertyMetadata(editable: true, inputType: InputType.ContentPicker)]
        public ContentReference RelatedContent { get; set; }
    }

    [SharedContentProperties(typeof(NewsContent))]
    public class NewsContentSharedProperties : SharedContentProperties
    {
        [ContentPropertyMetadata(editable: true, inputType: InputType.ContentPicker)]
        public ContentReference RelatedContent { get; set; }
    }
}
