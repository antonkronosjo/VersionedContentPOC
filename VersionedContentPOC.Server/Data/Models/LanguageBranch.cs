using VersionedContentPOC.Data.Enums;
using VersionedContentPOC.Extensions;
using System.Text.Json.Serialization;

namespace VersionedContentPOC.Data.Models;

public class LanguageBranch
{
    public LanguageBranch()
    {
        
    }
    public LanguageBranch(Guid contentId, Language language)
    {
        ContentId = contentId;
        Language = language;
    }

    public Guid ContentId { get; set; }
    public Language Language { get; set; }
    public Guid? ActiveVersionId { get; set; }

    [JsonIgnore]
    public Content? ActiveVersion { get; set; }

    [JsonIgnore]
    public ICollection<Content> Versions { get; set; } = new List<Content>();

    public void AddVersion<T>(T content, bool setAsActive = true) where T : Content
    {
        Versions.AddIfNotAny(content, x => x.VersionId == content.VersionId);

        if (!setAsActive)
            return;
        
        ActiveVersionId = content.VersionId;
        ActiveVersion = content;
        content.ContentId = ContentId;
    }
}
