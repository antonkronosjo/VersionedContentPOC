using VersionedContentPOC.CMS.Data.Enums;

namespace VersionedContentPOC.CMS.Data.Interfaces;

public interface IPublishable : IContentVersion
{
    PublishStatus Status { get; set; }
    DateTime? StartPublish { get; set; }
    DateTime? StopPublish { get; set; }
}
