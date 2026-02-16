using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using VersionedContentPOC.CMS.Data.Interfaces;
namespace VersionedContentPOC.CMS.Data.Models;

public abstract class ContentVersion : IContentVersion, IVersionable
{
    public ContentVersion()
    {
        VersionCreated = DateTime.UtcNow;
    }

    [Required]
    public int VersionId { get; set; }
    public DateTime VersionCreated { get; set; }

    [Required]
    public int ContentId { get; set; }
    
    public ContentRoot ContentRoot { get; set; }
}
