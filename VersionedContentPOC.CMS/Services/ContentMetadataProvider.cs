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
    public static UpdateContentRequest GetUpdateSchema(Content content, ContentRoot contentRoot, List<Language> contentLanguages)
    {
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
                    ReadOnly = false
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