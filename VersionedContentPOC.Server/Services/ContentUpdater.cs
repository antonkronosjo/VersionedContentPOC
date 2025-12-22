using System.Reflection;
using System.Text.Json;
using VersionedContentPOC.Attributes;

namespace VersionedContentPOC.Server.Services;

public static class ContentUpdater
{
    public static void ApplyUpdates<T>(T content, IDictionary<string, ContentPropertyValueDto> updates) where T : class
    {

        
        ValidateSchema(content.GetType(), updates);

        var instanceProperties = content.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite && p.CanRead)
            .ToDictionary(p => p.Name, p => p);

        foreach (var (propertyName, dto) in updates)
        {
            if (!instanceProperties.TryGetValue(propertyName, out var prop))
                throw new KeyNotFoundException($"Content of type {content.GetType().Name} does not contain a property named \"{propertyName}\"");

            var attr = prop.GetCustomAttribute<ContentPropertyMetadataAttribute>(inherit: false);
            if (attr == null || !attr.Editable)
                throw new UnauthorizedAccessException($"Property \"{propertyName}\" is not editable");

            SetValue(content, prop, dto);
        }
    }

    /// <summary>
    /// Validates schema so that types actually is the same as intended to avoid remote code execution
    /// </summary>
    private static void ValidateSchema(Type type, IDictionary<string, ContentPropertyValueDto> schema)
    {
        ContentTypeRegistry.Guards.IsRegiesteredContentType(type);
        var originalSchema = ContentMetadataProvider.GetPropertySchema(type);

        // Reject unknown properties
        if (schema.Keys.Except(originalSchema.Keys).Any())
            throw new InvalidOperationException("Schema contains unknown properties.");


        if (!schema.All(x => String.Equals(originalSchema[x.Key].PropertyTypeFullName, x.Value.PropertyTypeFullName, StringComparison.Ordinal)))
            throw new InvalidOperationException("Schema validation not successfull!");
    }

    private static void SetValue(object content, PropertyInfo prop, ContentPropertyValueDto dto)
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
