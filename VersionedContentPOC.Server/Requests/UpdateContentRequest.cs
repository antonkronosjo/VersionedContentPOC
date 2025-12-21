using VersionedContentPOC.Data.Enums;
using VersionedContentPOC.Server.Services;

namespace VersionedContentPOC.Server.Requests;

public class UpdateContentRequest
{
    public required Guid ContentId { get; set; }
    public required Language Language { get; set; }
    public required Dictionary<string, ContentPropertyValueDto> PropertiesSchema { get; set; }
}
    


