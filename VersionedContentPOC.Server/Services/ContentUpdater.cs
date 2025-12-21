using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json;
using VersionedContentPOC.Attributes;

namespace VersionedContentPOC.Server.Services;

public static class ContentUpdater
{
    public static T ApplyUpdates<T>(this T content, Dictionary<string, ContentPropertyValueDto> updates) where T : class
    {
        var instanceProperties = typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite && p.CanRead)
            .ToDictionary(p => p.Name, p => p);

        foreach (var (propertyName, dto) in updates)
        {
            if (!instanceProperties.TryGetValue(propertyName, out var prop))
                throw new KeyNotFoundException($"Content of type {typeof(T).Name} does not contain a property named \"{propertyName}\"");

            var attr = prop.GetCustomAttribute<ContentPropertyMetaDataAttribute>(inherit: false);
            if (attr == null || !attr.Editable)
                throw new UnauthorizedAccessException($"Property \"{propertyName}\" is not editable");

            SetValue(content, prop, dto);
        }

        return content;
    }

    public static void SetValue<T>(T content, PropertyInfo prop, ContentPropertyValueDto dto) where T : class
    {
        var value = dto.Value;

        if (value != null)
        {
            var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

            if (targetType.IsEnum)
            {
                var enumValue = Enum.Parse(targetType, value.ToString()!, ignoreCase: true);
                prop.SetValue(content, enumValue);
            }
            else if (!targetType.IsPrimitive && !targetType.IsAssignableFrom(value.GetType()))
            {
                var obj = JsonSerializer.Deserialize(value.ToString()!, targetType);
                prop.SetValue(content, obj);
            }
            else
            {
                prop.SetValue(content, Convert.ChangeType(value, targetType));
            }
        }
        else if (ContentMetadataProvider.IsRequired(prop))
        {
            throw new ValidationException($"Property \"{prop.Name}\" is required.");
        }
    }
}
