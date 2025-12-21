using System.Net.Mime;
using System.Reflection;
using System.Runtime.CompilerServices;
using VersionedContentPOC.Attributes;
using VersionedContentPOC.Data.Enums;
using VersionedContentPOC.Data.Models;
using VersionedContentPOC.Server.Attributes;
using VersionedContentPOC.Server.Requests;

namespace VersionedContentPOC.Server.Services;

public static class ContentMetadataProvider
{
    public static CreateContentRequest GetCreationSchema(Type contentType, Language language)
    {
        return new CreateContentRequest
        {
            Metadata = new CreateContentRequestMetadata
            {
                ContentTypeName = contentType.Name,
                Language = language,
            },
            PropertiesSchema = GetPropertySchema(contentType)
        };  
    }

    public static UpdateContentRequest GetUpdateSchema(Content content)
    {
        return new UpdateContentRequest
        {
            Metadata = new UpdateContentRequestMetadata {
                ContentId = content.ContentId,
                CurrentVersionId = content.VersionId,
                Language = content.Language,
            },
            PropertiesSchema = GetPropertySchema(content.GetType())
                .Where(x => x.Value.IsRequired == false)
                .ToDictionary()
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

    private static bool HasContentMetaData(PropertyInfo property)
    {
        return property.IsDefined(typeof(ContentPropertyMetaDataAttribute), inherit: true);
    }

    [ShouldBeRefactored("the RequiredMemberAttribute check does not seem to work")]
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