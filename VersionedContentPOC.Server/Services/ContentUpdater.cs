using System.Reflection;
using System.Text.Json;
using VersionedContentPOC.Attributes;
using VersionedContentPOC.Server.Attributes;

namespace VersionedContentPOC.Server.Services;

public static class ContentUpdater
{
    public static void ApplyUpdates<T>(T content, IDictionary<string, ContentPropertyValueDto> updates) where T : class
    {
        //ValidateSchema(content.GetType(), updates);

        var instanceProperties = content.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite && p.CanRead)
            .ToDictionary(p => p.Name, p => p);

        foreach (var (propertyName, dto) in updates)
        {
            if (!instanceProperties.TryGetValue(propertyName, out var prop))
                throw new Exception($"Could not get PropertyInfo for property \"{propertyName}\" from class {content.GetType().Name}");

            var attr = prop.GetCustomAttribute<ContentPropertyMetadataAttribute>(inherit: false);
            if (attr?.Editable != true)
                throw new UnauthorizedAccessException($"Property \"{propertyName}\" is not editable");

            SetValue(content, prop, dto);
        }
    }

    [ShouldBeRefactored("As for now no schema validation is done this is crucial since people can otherwhise inject other properties here.")]
    //private static void ValidateSchema(Type type, IDictionary<string, ContentPropertyValueDto> schema)
    //{
    //    ContentTypeRegistry.Guards.IsRegiesteredContentType(type);
    //    var originalSchema = ContentMetadataProvider.GetPropertySchema(type);

    //    if (schema.Keys.Except(originalSchema.Keys).Any())
    //        throw new InvalidOperationException("Schema contains unknown properties.");
    //}

    private static void SetValue(object content, PropertyInfo prop, ContentPropertyValueDto dto)
    {
        var value = dto.Value;
        var resolvedValue = ResolveValue(prop.PropertyType.FullName!, value);
        prop.SetValue(content, resolvedValue);
    }

    private static object? ResolveValue(string typeName, object? rawValue)
    {
        if (rawValue == null)
            return null;

        var type = Type.GetType(typeName, throwOnError: false);
        if (type == null)
            throw new InvalidOperationException("Type could not be resolved");

        if (rawValue is JsonElement je)
            return JsonSerializer.Deserialize(je.GetRawText(), type);

        throw new InvalidOperationException($"Unexpected value type: {rawValue.GetType()}");
    }
}
