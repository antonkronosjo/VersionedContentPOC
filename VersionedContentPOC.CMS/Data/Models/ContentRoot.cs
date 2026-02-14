using System.Text.Json.Serialization;
using VersionedContentPOC.CMS.Data.Enums;

namespace VersionedContentPOC.CMS.Data.Models;

public class ContentRoot
{
    public ContentRoot(Language mainLanguage)
    {
        Created = DateTime.UtcNow;
        MainLanguage = mainLanguage;
    }

    public int ContentId { get; set; }
    public DateTime Created { get; set; }
    public Language MainLanguage { get; set; }

    public SharedContentProperties? SharedContentProperties { get; set; }

    [JsonIgnore]
    public ICollection<Content> Versions { get; set; } = new List<Content>();
}