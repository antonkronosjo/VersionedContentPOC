using System.ComponentModel.DataAnnotations;
using System.Reflection;
using VersionedContentPOC.CMS.Data.Models;
using VersionedContentPOC.CMS.Data.Enums;
using VersionedContentPOC.CMS.Requests;
using VersionedContentPOC.CMS.Attributes;
using VersionedContentPOC.CMS.Extensions;

namespace VersionedContentPOC.CMS.Services;

public static class ContentMetadataProvider
{
    public static CreateContentRequest GetCreationSchema(Type contentType, Language language)
    {
        var sharedPropertiesType = ContentTypeRegistry.GetSharedContentPropertiesForContentType(contentType);

        return new CreateContentRequest
        {
            Metadata = new CreateContentRequestMetadata
            {
                ContentTypeName = contentType.Name,
                Language = language,
            },
            PropertiesSchema = GetPropertySchema(contentType),
            SharedPropertiesSchema = GetPropertySchema(sharedPropertiesType)
        };  
    }

    [ShouldBeRefactored("Would be good to not need to inject translated languages")]
    public static UpdateContentRequest GetUpdateSchema(Content content, ContentRoot contentRoot, List<Language> contentLanguages)
    {
        var sharedPropertiesType = ContentTypeRegistry.GetSharedContentPropertiesForContentType(content.GetType());

        return new UpdateContentRequest
        {
            Metadata = new UpdateContentRequestMetadata {
                ContentId = content.ContentId,
                VersionId = content.VersionId,
                Language = content.Language,
                ContentTypeName = content.GetType().Name,
                Created = contentRoot.Created,
                StartPublish = content.StartPublish,
                StopPublish = content.StopPublish,
                Status = content.Status,
                LanguageTranslations = contentLanguages
            },
            PropertiesSchema = GetPropertySchema(content.GetType(), content),
            SharedPropertiesSchema = GetPropertySchema(sharedPropertiesType)
        };
    }

    [ShouldBeRefactored("Guard should be re-added")]
    [ShouldBeRefactored("Maybe not use object as param?")]
    public static Dictionary<string, ContentPropertyValueDto> GetPropertySchema(Type contentType, object? instance = null)
    {
        //ContentTypeRegistry.Guards.IsRegiesteredContentType(contentType);

        return contentType.GetContentProperties()
            .ToDictionary(
                p => p.Name,
                p => new ContentPropertyValueDto
                {
                    InputType = p.GetCustomAttribute<ContentPropertyMetadataAttribute>().PropertyInputType,
                    IsRequired = p.IsDefined(typeof(RequiredAttribute), inherit: true),
                    Value = instance != null
                        ? p.GetValue(instance)
                        : null,
                    ReadOnly = instance is Content content && content.Status != PublishStatus.Draft
                }
            );
    }
}

public class ContentPropertyValueDto
{   
    public InputType InputType { get; set; }
    public bool IsRequired { get; set; }
    public object? Value { get; set; }
    public bool ReadOnly { get; set; }
}