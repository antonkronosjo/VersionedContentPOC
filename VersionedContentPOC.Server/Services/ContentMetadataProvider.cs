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

    [ShouldBeRefactored("Need to pre-populate values from existing content")]
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

    public static Dictionary<string, ContentPropertyValueDto> GetPropertySchema(Type contentType)
    {
        ContentTypeRegistry.Guards.IsRegiesteredContentType(contentType);

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
        return property.IsDefined(typeof(ContentPropertyMetadataAttribute), inherit: true);
    }

    [ShouldBeRefactored("the RequiredMemberAttribute check does not seem to work")]
    public static bool IsRequired(PropertyInfo property)
    {
        var requiredAttr = property.GetCustomAttribute<ContentPropertyMetadataAttribute>();
        return requiredAttr?.Required ?? false
               || property.CustomAttributes.Any(a => a.AttributeType == typeof(RequiredMemberAttribute));
    }
}

public class ContentPropertyValueDto
{
    [ShouldBeRefactored("This property should not be exposed by the API, we need to re-create it when applying creation")]
    public string PropertyTypeFullName { get; set; } = null!;
    public bool IsRequired { get; set; }
    public string? Value { get; set; }
}