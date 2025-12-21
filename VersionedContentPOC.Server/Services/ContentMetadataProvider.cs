using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using VersionedContentPOC.Attributes;
using VersionedContentPOC.Data.Enums;
using VersionedContentPOC.Data.Models;
using VersionedContentPOC.Server.Requests;

namespace VersionedContentPOC.Server.Services;

public static class ContentMetadataProvider
{
    public static CreateContentRequest GetCreationSchema(Type contentType, Language language)
    {
        if (!typeof(Content).IsAssignableFrom(contentType))
            throw new InvalidOperationException($"Type '{contentType.FullName}' does not inherit from {nameof(Content)}.");


        return new CreateContentRequest
        {
            ContentTypeName = contentType.Name,
            Language = language,
            Properties = contentType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => IsRequired(p) || HasContentMetaData(p))
                .ToDictionary(
                    p => p.Name,
                    p => new ContentPropertyValueDto
                    {
                        PropertyTypeFullName = p.PropertyType.FullName!,
                        IsRequired = IsRequired(p)
                    }
                )
        };  
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

//public class ContentProperty
