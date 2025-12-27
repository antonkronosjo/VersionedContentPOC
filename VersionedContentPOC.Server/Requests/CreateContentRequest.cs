using VersionedContentPOC.Data.Enums;
using VersionedContentPOC.Server.Services;

namespace VersionedContentPOC.Server.Requests;

public class CreateContentRequest
{
    public required CreateContentRequestMetadata Metadata { get; set; }
    public required Dictionary<string, ContentPropertyValueDto> PropertiesSchema { get; set; }
}

public class CreateContentRequestMetadata
{
    public required string ContentTypeName { get; set; }
    public required Language Language { get; set; }
}
