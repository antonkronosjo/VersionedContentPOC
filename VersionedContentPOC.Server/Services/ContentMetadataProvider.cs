using System.Net.Mime;
using System.Reflection;
using System.Runtime.CompilerServices;
using VersionedContentPOC.Attributes;
using VersionedContentPOC.Data.Enums;
using VersionedContentPOC.Data.Models;
using VersionedContentPOC.Server.Requests;

namespace VersionedContentPOC.Server.Services;

public static class ContentMetadataProvider
{
    public static CreateContentRequest GetCreationSchema(Type contentType, Language language)
    {
        return new CreateContentRequest
        {
            ContentTypeName = contentType.Name,
            Language = language,
            PropertiesSchema = GetPropertySchema(contentType)
        };  
    }

    public static IDictionary<string, ContentPropertyValueDto> GetPropertySchema(Type contentType)
    {
        if (!typeof(Content).IsAssignableFrom(contentType))
            throw new InvalidOperationException($"Type '{contentType.FullName}' does not inherit from {nameof(Content)}.");

        return contentType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => IsRequired(p) || HasContentMetaData(p))
            .ToDictionary(
                p => p.Name,
                p => new ContentPropertyValueDto
                {
                    PropertyTypeFullName = p.PropertyType.FullName!,
                    IsRequired = IsRequired(p)
                }
            );
    }

    /// <summary>
    /// Validates schema so that types actually is the same as intended to avoid remote code execution
    /// </summary>
    public static void ValidateSchema(Type type, IDictionary<string, ContentPropertyValueDto> schema)
    {
        var originalSchema = GetPropertySchema(type);

        // Reject unknown properties
        if (schema.Keys.Except(originalSchema.Keys).Any())
            throw new InvalidOperationException("Schema contains unknown properties.");


        if (!schema.All(x => String.Equals(originalSchema[x.Key].PropertyTypeFullName, x.Value.PropertyTypeFullName, StringComparison.Ordinal)))
            throw new InvalidOperationException("Schema validation not successfull!");
    }

    private static bool HasContentMetaData(PropertyInfo property)
    {
        return property.IsDefined(typeof(ContentPropertyMetaDataAttribute), inherit: true);
    }

    public static bool IsRequired(PropertyInfo property)
    {
        var requiredAttr = property.GetCustomAttribute<ContentPropertyMetaDataAttribute>();
        return requiredAttr?.Required ?? false
               || property.CustomAttributes.Any(a => a.AttributeType == typeof(RequiredMemberAttribute));
    }
}

public class ContentPropertyValueDto
{
    public string PropertyTypeFullName { get; set; } = null!;
    public bool IsRequired { get; set; }
    public object? Value { get; set; }
}