using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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
