using System.Text.Json.Serialization;
namespace VersionedContentPOC.CMS.Data.Models;

public class ContentRoot
{
    public ContentRoot()
    {
        Created = DateTime.UtcNow;
    }

    public int ContentId { get; set; }
    public DateTime Created { get; set; }

    [JsonIgnore]
    public ICollection<InvariantVersion> SharedContentProperties { get; set; }

    [JsonIgnore]
    public ICollection<LocalizableVersion> Versions { get; set; } = new List<LocalizableVersion>();
}