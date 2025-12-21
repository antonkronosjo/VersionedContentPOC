using VersionedContentPOC.Data.Enums;
using VersionedContentPOC.Server.Services;

namespace VersionedContentPOC.Server.Requests;

public class UpdateContentRequest
{
    public required UpdateContentRequestMetadata Metadata { get; set; }
    public required IDictionary<string, ContentPropertyValueDto> PropertiesSchema { get; set; }
}

public class UpdateContentRequestMetadata
{
    public required Guid ContentId { get; set; }
    public required Guid CurrentVersionId { get; set; }
    public required Language Language { get; set; }
}
    


