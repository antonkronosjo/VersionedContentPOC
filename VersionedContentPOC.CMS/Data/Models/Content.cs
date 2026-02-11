using System.ComponentModel.DataAnnotations;
using VersionedContentPOC.CMS.Attributes;
using VersionedContentPOC.CMS.Data.Enums;

namespace VersionedContentPOC.CMS.Data.Models;

public abstract class Content
{
    protected Content()
    {
            
    }

    public Content(Language language)
    {
        VersionCreated = DateTime.UtcNow;
        Language = language;
        Status = PublishStatus.Draft;
    }

    [Required]
    public int VersionId { get; set; }

    [Required]
    public int ContentId { get; set; }

    [Required]
    public PublishStatus Status { get; set; }

    [Required]
    public Language Language { get; set; }

    public DateTime? StartPublish { get; set; }
    public DateTime? StopPublish { get; set; }

    [ShouldBeRefactored("Should this be marked as nullable??? In practice instances can exist with this set to null")]
    public ContentRoot ContentRoot { get; set; }

    public DateTime VersionCreated { get; set; }
}