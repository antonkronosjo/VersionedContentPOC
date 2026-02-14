using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersionedContentPOC.CMS.Data.Models;

namespace VersionedContentPOC.CMS.Attributes
{
    public class SharedContentPropertiesAttribute : Attribute
    {
        public Type ContentType { get; set; }
        public SharedContentPropertiesAttribute(Type type)
        {
            ContentType = type;
        }
    }
}
