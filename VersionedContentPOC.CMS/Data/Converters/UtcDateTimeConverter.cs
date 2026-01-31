using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VersionedContentPOC.CMS.Data.Converters
{
    internal class UtcDateTimeConverter : ValueConverter<DateTime, DateTime>
    {
        public UtcDateTimeConverter()
            : base(
            v => v.ToUniversalTime(),                     // Convert to UTC when saving
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc) // Ensure UTC when reading
          )
        {

        }
    }
}
