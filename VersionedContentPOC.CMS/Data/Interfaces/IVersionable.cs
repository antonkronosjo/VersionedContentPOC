using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VersionedContentPOC.CMS.Data.Interfaces
{
    public interface IVersionable
    {
        int ContentId { get; set; }
        int VersionId { get; set; }
        DateTime VersionCreated { get; set; }
    }
}
