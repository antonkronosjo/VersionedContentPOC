using VersionedContentPOC.Data.Enums;
using VersionedContentPOC.Server.Services;

namespace VersionedContentPOC.Server.Requests;

public record UpdateContentRequest(
    Guid ContentId,
    Language Language,
    Dictionary<string, ContentPropertyValueDto> Updates
);
