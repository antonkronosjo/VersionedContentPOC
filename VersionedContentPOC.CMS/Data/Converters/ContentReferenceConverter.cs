using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersionedContentPOC.CMS.Data.Models;

namespace VersionedContentPOC.CMS.Data.Converters
{
    internal class ContentReferenceConverter : ValueConverter<ContentReference, int>
    {
        public ContentReferenceConverter() : base
            (
                x => x.ContentId,
                x => new ContentReference(x)
            )
        {
            
        }
    }
}
