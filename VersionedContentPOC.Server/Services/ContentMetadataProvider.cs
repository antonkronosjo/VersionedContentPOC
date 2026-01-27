using System.Reflection;
using System.Runtime.CompilerServices;
using VersionedContentPOC.Attributes;
using VersionedContentPOC.Data.Enums;
using VersionedContentPOC.Data.Models;
using VersionedContentPOC.Server.Attributes;
using VersionedContentPOC.Server.Controllers.Requests;
using VersionedContentPOC.Server.Data.Enums;
using VersionedContentPOC.Server.Extensions;

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
            PropertiesSchema = GetPropertySchema(contentType, language)
        };  
    }

    [ShouldBeRefactored("Would be good to not need to inject translated languages")]
    [ShouldBeRefactored("Should be a new schema created for translations where i need current active version of main language")]
    [ShouldBeRefactored("I need to have current active version here")]
    public static UpdateContentRequest GetUpdateSchema(Content content, ContentRoot contentRoot, List<Language> contentLanguages)
    {
        return new UpdateContentRequest
        {
            Metadata = new UpdateContentRequestMetadata {
                ContentId = content.ContentId,
                VersionId = content.VersionId,
                ActiveVersionId = content.LanguageBranch?.ActiveVersionId,
                Language = content.Language,
                Created = contentRoot.Created,
                StartPublish = contentRoot.StartPublish,
                StopPublish = contentRoot.StopPublish,
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
                    IsRequired = IsRequired(p),
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
    public InputType InputType { get; set; }
    public bool IsRequired { get; set; }
    public object? Value { get; set; }
    public bool ReadOnly { get; set; }
}