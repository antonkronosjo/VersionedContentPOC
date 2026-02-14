using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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
