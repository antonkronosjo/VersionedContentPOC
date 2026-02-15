using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersionedContentPOC.CMS.Data.Enums;

namespace VersionedContentPOC.CMS.Data.Interfaces
{
    public interface IPublishable : IContentVersion
    {
        PublishStatus Status { get; set; }
        DateTime? StartPublish { get; set; }
        DateTime? StopPublish { get; set; }
    }
}
