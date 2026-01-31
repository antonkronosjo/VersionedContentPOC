using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersionedContentPOC.CMS.Data.Models;

namespace VersionedContentPOC.CMS.Data.Converters
{
    internal class NullableContentReferenceConverter : ValueConverter<ContentReference?, int?>
    {
        public NullableContentReferenceConverter() : base
            (
                x => x.HasValue
                    ? x.Value.ContentId 
                    : null,
                x => x.HasValue
                    ? new ContentReference(x.Value)
                    : null
            )
        {

        }
    }
}
