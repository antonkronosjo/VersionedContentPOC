using VersionedContentPOC.Data.Enums;
using VersionedContentPOC.Server.Services;

namespace VersionedContentPOC.Server.Requests;

public class CreateContentRequest
{
    public required string ContentTypeName { get; set; }
    public required Language Language { get; set; }
    public required IDictionary<string, ContentPropertyValueDto> PropertiesSchema { get; set; }
}
