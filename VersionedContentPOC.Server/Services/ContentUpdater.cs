using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using VersionedContentPOC.Attributes;
using VersionedContentPOC.Data.Models;

namespace VersionedContentPOC.Server.Services;

public static class ContentUpdater
{
    public static void ApplyUpdates<T>(T content, Dictionary<string, ContentPropertyValueDto> updates) where T : class
    {
        if (!typeof(Content).IsAssignableFrom(content.GetType()))
            throw new InvalidOperationException($"Type '{content.GetType().Name}' does not inherit from {nameof(Content)}.");

        var instanceProperties = typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite && p.CanRead)
            .ToDictionary(p => p.Name, p => p);

        foreach (var (propertyName, dto) in updates)
        {
            if (!instanceProperties.TryGetValue(propertyName, out var prop))
                throw new KeyNotFoundException($"Content of type {content.GetType().Name} does not contain a property named \"{propertyName}\"");

            var attr = prop.GetCustomAttribute<ContentPropertyMetaDataAttribute>(inherit: false);
            if (attr == null || !attr.Editable)
                throw new UnauthorizedAccessException($"Property \"{propertyName}\" is not editable");

            SetValue(content, prop, dto);
        }
    }

    public static void SetValue(object content, PropertyInfo prop, ContentPropertyValueDto dto)
    {
        var value = dto.Value;
        var resolvedValue = ResolveValue(dto.PropertyTypeFullName, value);
        prop.SetValue(content, resolvedValue);
    }

    private static object? ResolveValue(string typeName, object? rawValue)
    {
        if (rawValue == null)
            return null;

        var type = Type.GetType(typeName, throwOnError: true);
        if (type == null)
            throw new InvalidOperationException("Type could not be resolved");

        if (rawValue is JsonElement je)
            return JsonSerializer.Deserialize(je.GetRawText(), type);

        throw new InvalidOperationException($"Unexpected value type: {rawValue.GetType()}");
    }
}
