using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using VersionedContentPOC.Data.Enums;
using VersionedContentPOC.Server.Attributes;
using VersionedContentPOC.Server.Data.Models;

namespace VersionedContentPOC.Data.Models;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "contentType", UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor)]
[JsonDerivedType(typeof(NewsContent), nameof(NewsContent))]
[JsonDerivedType(typeof(EventContent), nameof(EventContent))]
public abstract class Content
{
    protected Content()
    {
            
    }

    public Content(Language language)
    {
        VersionCreated = DateTime.UtcNow;
        Language = language;
    }

    [Required]
    public int VersionId { get; set; }

    [Required]
    public int ContentId { get; set; }

    [ShouldBeRefactored("Should this be marked as nullable??? In practice instances can exist with this set to null")]
    public ContentRoot ContentRoot { get; set; }

    [Required]
    public Language Language { get; set; }
    public virtual LanguageBranch LanguageBranch { get; set; }
    public DateTime VersionCreated { get; set; }

    /// <summary>
    /// True/false if active/not active version. null if unknown (because LanguageBranch not included)
    /// </summary>
    public bool? IsActiveVersion => LanguageBranch?.ActiveVersionId == VersionId;
}