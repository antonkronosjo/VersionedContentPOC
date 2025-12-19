using VersionedContentPOC.Data.Enums;
using System.Net.Mime;
using System.Text.Json.Serialization;

namespace VersionedContentPOC.Data.Models;

public class ContentRoot
{
    public ContentRoot()
    {
        
    }

    public ContentRoot(Guid contentId)
    {
        ContentId = contentId;
        Created = DateTime.UtcNow;
    }

    public Guid ContentId { get; set; }
    public DateTime Created { get; set; }
    public DateTime? StartPublish { get; set; }
    public DateTime? StopPublish { get; set; }

    [JsonIgnore]
    public ICollection<LanguageBranch> LanguageBranches { get; set; } = new List<LanguageBranch>();

    /// <summary>
    /// Add content for given language. Returns language branch if it does not exist before.
    /// </summary>
    public LanguageBranch? AddNewLanguageBranchIfNotExist(Language language)
    {
        var languageBranch = LanguageBranches.FirstOrDefault(x => x.Language == language);
        if (languageBranch != null)
            return null;

        return AddNewLanguageBranch(language);
    }

    public LanguageBranch AddNewLanguageBranch(Language language) {
        var languageBranch = new LanguageBranch(this.ContentId, language);
        LanguageBranches.Add(languageBranch);
        return languageBranch;
    }
}