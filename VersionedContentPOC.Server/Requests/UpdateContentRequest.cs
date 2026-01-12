using System.ComponentModel.DataAnnotations;
using VersionedContentPOC.Data.Enums;
using VersionedContentPOC.Server.Services;

namespace VersionedContentPOC.Server.Requests;

public class UpdateContentRequest
{
    [Required]
    public virtual required UpdateContentRequestMetadata Metadata { get; set; }

    [Required]
    public required IDictionary<string, ContentPropertyValueDto> PropertiesSchema { get; set; }
}

public class UpdateContentRequestMetadata
{
    public required Guid ContentId { get; set; }
    public required Guid? CurrentVersionId { get; set; }
    public required Language Language { get; set; }
    public required DateTime? StartPublish { get; set; }
    public required DateTime? StopPublish { get; set; }
    public required List<Language> LanguageTranslations { get; set; }
    public bool ForceUpdate { get; set; }
}
    


