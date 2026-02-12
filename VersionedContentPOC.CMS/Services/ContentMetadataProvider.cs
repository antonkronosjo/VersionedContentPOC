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
        return new CreateContentRequest
        {
            Metadata = new CreateContentRequestMetadata
            {
                ContentTypeName = contentType.Name,
                Language = language,
            },
            PropertiesSchema = GetPropertySchema(contentType, language)
        };  
    }

    [ShouldBeRefactored("Would be good to not need to inject translated languages")]
    [ShouldBeRefactored("Should be a new schema created for translations where i need current active version of main language")]
    [ShouldBeRefactored("I need to have current active version here")]
    [ShouldBeRefactored("DBR: Look over stop/start publish/active version")]
    public static UpdateContentRequest GetUpdateSchema(Content content, ContentRoot contentRoot, List<Language> contentLanguages)
    {
        return new UpdateContentRequest
        {
            Metadata = new UpdateContentRequestMetadata {
                ContentId = content.ContentId,
                ContentTypeName = content.GetType().Name,
                VersionId = content.VersionId,
                Language = content.Language,
                Created = contentRoot.Created,
                StartPublish = content.StartPublish,
                StopPublish = content.StopPublish,
                Status = content.Status,
                LanguageTranslations = contentLanguages
            },
            PropertiesSchema = GetPropertySchema(content.GetType(), contentRoot.MainLanguage, content)
        };
    }

    public static Dictionary<string, ContentPropertyValueDto> GetPropertySchema(Type contentType, Language mainLanguage, Content? content = null)
    {
        ContentTypeRegistry.Guards.IsRegiesteredContentType(contentType);

        return contentType.GetContentProperties()
            .ToDictionary(
                p => p.Name,
                p => new ContentPropertyValueDto
                {
                    InputType = p.GetCustomAttribute<ContentPropertyMetadataAttribute>().PropertyInputType,
                    IsRequired = p.IsDefined(typeof(RequiredAttribute), inherit: true),
                    Value = content != null
                        ? p.GetValue(content)
                        : null,
                    ReadOnly = GetReadOnly(p, mainLanguage, content)
                }
            );
    }

    private static bool GetReadOnly(PropertyInfo propertyInfo, Language mainLanguage, Content? content)
    {
        if (content == null)
            return false;

        if (content.Language == mainLanguage)
            return false;

        return propertyInfo.GetCustomAttribute<MainLanguageOnlyAttribute>()?.IsActive == true;
    }
}

public class ContentPropertyValueDto
{   
    public InputType InputType { get; set; }
    public bool IsRequired { get; set; }
    public object? Value { get; set; }
    public bool ReadOnly { get; set; }
}