using System.ComponentModel.DataAnnotations;
using VersionedContentPOC.CMS.Data.Enums;
using VersionedContentPOC.CMS.Services;

namespace VersionedContentPOC.CMS.Requests;

public class UpdateContentRequest
{
    [Required]
    public virtual required UpdateContentRequestMetadata Metadata { get; set; }

    [Required]
    public required Dictionary<string, ContentPropertyValueDto> PropertiesSchema { get; set; }
    public required Dictionary<string, ContentPropertyValueDto> SharedPropertiesSchema { get; set; } = new();
}

public class UpdateContentRequestMetadata
{
    [Required]
    public required int ContentId { get; set; }

    [Required]
    public required int VersionId { get; set; }

    [Required]
    public required Language Language { get; set; }
    public required DateTime? Created { get; set; }

    [Required]
    public required PublishStatus Status { get; set; }
    public required DateTime? StartPublish { get; set; }
    public required DateTime? StopPublish { get; set; }
    public required List<Language> LanguageTranslations { get; set; }
    public bool ForceNewVersion { get; set; }

    [Required]
    public required string ContentTypeName { get; set; }
}
    


