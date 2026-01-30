using System.Text.Json.Serialization;
using VersionedContentPOC.CMS.Data.Enums;

namespace VersionedContentPOC.CMS.Data.Models;

public class LanguageBranch
{
    public LanguageBranch(int contentId, Language language)
    {
        ContentId = contentId;
        Language = language;
    }

    public int ContentId { get; set; }
    public Language Language { get; set; }
    public int? ActiveVersionId { get; set; }

    [JsonIgnore]
    public Content? ActiveVersion { get; set; }

    [JsonIgnore]
    public ICollection<Content> Versions { get; set; } = new List<Content>();

    public void AddVersion<T>(T content, bool setAsActive = true) where T : Content
    {
        content.ContentId = ContentId;

        if (!Versions.Any(x => x.VersionId == content.VersionId)) //Should throw exception if this is true
            Versions.Add(content);

        if (setAsActive)
            SetActiveVersion(content);
    }

    public void SetActiveVersion<T>(T content) where T : Content
    {
        ActiveVersionId = content.VersionId;
        ActiveVersion = content;
    }
}
