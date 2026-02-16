using System.ComponentModel.DataAnnotations;
using VersionedContentPOC.CMS.Data.Enums;
using VersionedContentPOC.CMS.Data.Interfaces;
namespace VersionedContentPOC.CMS.Data.Models;

public abstract class LocalizableVersion : ContentVersion, IPublishable, ILocalizable
{
    public LocalizableVersion(Language language) : base()
    {
        VersionCreated = DateTime.UtcNow;
        Language = language;
        Status = PublishStatus.Draft;
    }

    [Required]
    public PublishStatus Status { get; set; }

    [Required]
    public Language Language { get; set; }

    public DateTime? StartPublish { get; set; }
    public DateTime? StopPublish { get; set; }

}