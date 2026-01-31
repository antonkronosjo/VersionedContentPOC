using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VersionedContentPOC.CMS.Data.Models;

namespace VersionedContentPOC.CMS.Data.Converters;


internal class PropertyConverterRegistry
{
    private readonly Dictionary<Type, ValueConverter> _converters = new();

    public PropertyConverterRegistry()
    {
        Register<DateTime>(new UtcDateTimeConverter());
        Register<DateTime?>(new NullableUtcDateTimeConverter());
        Register<ContentReference>(new ContentReferenceConverter());
        Register<ContentReference?>(new NullableContentReferenceConverter());
    }

    private void Register<T>(ValueConverter converter)
    {
        _converters[typeof(T)] = converter;
    }

    private ValueConverter? GetConverter(Type propertyType)
    {
        return _converters.TryGetValue(propertyType, out var converter) ? converter : null;
    }

    public void AddRegisteredConverters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.ClrType.GetProperties())
            {
                var converter = GetConverter(property.PropertyType);
                if (converter != null)
                {
                    modelBuilder
                        .Entity(entityType.Name)
                        .Property(property.Name)
                        .HasConversion(converter);
                }
            }
        }
    }
}
