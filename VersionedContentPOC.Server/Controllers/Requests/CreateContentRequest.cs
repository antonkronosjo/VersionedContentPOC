using System.ComponentModel.DataAnnotations;
using VersionedContentPOC.Data.Enums;
using VersionedContentPOC.Server.Services;

namespace VersionedContentPOC.Server.Controllers.Requests;

public class CreateContentRequest
{
    [Required]
    public required CreateContentRequestMetadata Metadata { get; set; }

    [Required]
    public required Dictionary<string, ContentPropertyValueDto> PropertiesSchema { get; set; } = new();
}

public class CreateContentRequestMetadata
{
    public required string ContentTypeName { get; set; }
    public required Language Language { get; set; }
}
