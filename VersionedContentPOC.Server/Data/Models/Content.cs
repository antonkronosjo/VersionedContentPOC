using VersionedContentPOC.Data.Enums;
using System.Text.Json.Serialization;
using VersionedContentPOC.Server.Data.Models;

namespace VersionedContentPOC.Data.Models;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "contentType")]
[JsonDerivedType(typeof(NewsContent), nameof(NewsContent))]
public abstract class Content
{
    protected Content()
    {
            
    }
    public Content(Guid versionId, Language language)
    {
        VersionId = versionId;
        VersionCreated = DateTime.UtcNow;
    }
    public Guid VersionId { get; set; }
    public Guid ContentId { get; set; }
    public ContentRoot ContentRoot { get; set; }
    public Language Language { get; set; }
    [JsonIgnore]
    public virtual LanguageBranch LanguageBranch { get; set; }
    public DateTime VersionCreated { get; set; }
}