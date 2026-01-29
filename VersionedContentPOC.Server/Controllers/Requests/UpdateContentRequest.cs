using System.ComponentModel.DataAnnotations;
using VersionedContentPOC.Data.Enums;
using VersionedContentPOC.Server.Services;

namespace VersionedContentPOC.Server.Controllers.Requests;

public class UpdateContentRequest
{
    [Required]
    public virtual required UpdateContentRequestMetadata Metadata { get; set; }

    [Required]
    public required IDictionary<string, ContentPropertyValueDto> PropertiesSchema { get; set; }
}

public class UpdateContentRequestMetadata
{
    public required int ContentId { get; set; }
    public required int? VersionId { get; set; }
    public required int? ActiveVersionId { get; set; }
    public required Language Language { get; set; }
    public required DateTime? Created { get; set; }
    public required DateTime? StartPublish { get; set; }
    public required DateTime? StopPublish { get; set; }
    public required List<Language> LanguageTranslations { get; set; }
    public bool ForceUpdate { get; set; }
    public required string ContentTypeName { get; set; }
}
    


