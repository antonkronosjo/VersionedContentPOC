using System.ComponentModel.DataAnnotations;
using VersionedContentPOC.CMS.Data.Enums;
using VersionedContentPOC.CMS.Services;

namespace VersionedContentPOC.CMS.Requests;

public class CreateContentRequest
{
    [Required]
    public required CreateContentRequestMetadata Metadata { get; set; }

    [Required]
    public required Dictionary<string, ContentPropertyValueDto> PropertiesSchema { get; set; } = new();
}

public class CreateContentRequestMetadata
{
    [Required]
    public required string ContentTypeName { get; set; }

    [Required]
    public required Language Language { get; set; }
}
