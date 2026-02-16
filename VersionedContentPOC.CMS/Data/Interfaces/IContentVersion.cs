using VersionedContentPOC.CMS.Data.Models;

namespace VersionedContentPOC.CMS.Data.Interfaces;

public interface IContentVersion : IVersionable
{
    int ContentId { get; set; }
    ContentRoot ContentRoot { get; set; }
}
