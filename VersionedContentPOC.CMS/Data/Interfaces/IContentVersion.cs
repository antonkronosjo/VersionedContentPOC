using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersionedContentPOC.CMS.Data.Models;

namespace VersionedContentPOC.CMS.Data.Interfaces
{
    public interface IContentVersion
    {
        int ContentId { get; set; }
        ContentRoot ContentRoot { get; set; }
    }
}
