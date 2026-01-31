using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VersionedContentPOC.CMS.Data.Converters
{
    internal class NullableUtcDateTimeConverter : ValueConverter<DateTime?, DateTime?>
    {
        public NullableUtcDateTimeConverter()
            : base(
                v => v.HasValue
                    ? v.Value.ToUniversalTime()
                    : v,
                v => v.HasValue
                    ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc)
                    : v
          )
        {

        }
    }
}
