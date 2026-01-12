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

    [ShouldBeRefactored("Would be good to not need to inject translated languages")]
    public static UpdateContentRequest GetUpdateSchema(Content content, List<Language> contentLanguages)
    {
        return new UpdateContentRequest
        {
            Metadata = new UpdateContentRequestMetadata {
                ContentId = content.ContentId,
                CurrentVersionId = content.VersionId,
                Language = content.Language,
                StartPublish = content.ContentRoot?.StartPublish,
                StopPublish = content.ContentRoot?.StopPublish,
                LanguageTranslations = contentLanguages
            },
            PropertiesSchema = GetPropertySchema(content.GetType(), content)
                .Where(x => x.Value.IsRequired == false)
                .ToDictionary()
        };
    }

    //[ShouldBeRefactored("Would be good to not need to inject translated languages")]
    //public static UpdateContentRequest GetUpdateSchemaForTranslation(ContentRoot contentRoot, Language language)
    //{
    //    return new UpdateContentRequest
    //    {
    //        Metadata = new UpdateContentRequestMetadata
    //        {
    //            ContentId = contentRoot.ContentId,
    //            CurrentVersionId = null,
    //            Language = language,
    //            StartPublish = contentRoot.StartPublish,
    //            StopPublish = contentRoot.StopPublish,
    //            LanguageTranslations = contentRoot
    //                .LanguageBranches
    //                .Select(x => x.Language)
    //                .ToList()
    //        },
    //        PropertiesSchema = GetPropertySchema(content.GetType())
    //            .Where(x => x.Value.IsRequired == false)
    //            .ToDictionary()
    //    };
    //}


    //[ShouldBeRefactored("Would be good to not need to inject translated languages")]
    //public static TranslateContentRequest GetTranslationSchema(ContentRoot contentRoot, List<Language> contentLanguages)
    //{
    //    return new UpdateContentRequest
    //    {
    //        Metadata = new UpdateContentRequestMetadata
    //        {
    //            ContentId = contentRoot.ContentId,
    //            CurrentVersionId = content.VersionId,
    //            Language = content.Language,
    //            StartPublish = content.ContentRoot.StartPublish,
    //            StopPublish = content.ContentRoot.StopPublish,
    //            LanguageTranslations = contentLanguages
    //        },
    //        PropertiesSchema = GetPropertySchema(content.GetType(), content)
    //            .Where(x => x.Value.IsRequired == false)
    //            .ToDictionary()
    //    };
    //}

    public static Dictionary<string, ContentPropertyValueDto> GetPropertySchema(Type contentType, Content? content = null)
    {
        ContentTypeRegistry.Guards.IsRegiesteredContentType(contentType);

        return contentType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => IsRequired(p) || HasContentMetaData(p))
            .ToDictionary(
                p => p.Name,
                p => new ContentPropertyValueDto
                {
                    PropertyTypeFullName = p.PropertyType.FullName!,
                    IsRequired = IsRequired(p),
                    Value = content != null
                        ? p.GetValue(content)
                        : null
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
    public object? Value { get; set; }
}