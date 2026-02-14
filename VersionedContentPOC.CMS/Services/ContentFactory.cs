using VersionedContentPOC.CMS.Data.Enums;
using VersionedContentPOC.CMS.Data.Models;

namespace VersionedContentPOC.CMS.Services
{
    public interface IContentFactory
    {
        Content CreateContentInstance(Type contentType, Language language, int? contentId = null, IDictionary<string, ContentPropertyValueDto>? properties = null);
        SharedContentProperties CreateSharedPropertiesInstance(Type sharedPropertiesType, int? contentId = null);
    }

    internal class ContentFactory : IContentFactory
    {
        public Content CreateContentInstance(Type contentType, Language language, int? contentId = null, IDictionary<string, ContentPropertyValueDto>? properties = null)
        {
            ContentTypeRegistry.Guards.IsRegiesteredContentType(contentType);

            var instance = (Content?)Activator.CreateInstance(contentType, language) ?? throw new InvalidOperationException("Failed to create content");

            if (contentId != null && contentId.HasValue)
                instance.ContentId = contentId.Value;

            if (properties != null)
                ContentUpdater.ApplyUpdates(instance, properties);
            
            return instance;
        }

        public SharedContentProperties CreateSharedPropertiesInstance(Type sharedPropertiesType, int? contentId = null)
        {
            var instance = (SharedContentProperties?)Activator.CreateInstance(sharedPropertiesType) ?? throw new InvalidOperationException("Failed to create shared properties");

            if (contentId != null && contentId.HasValue)
                instance.ContentId = contentId.Value;

            return instance;
        }
    }
}
