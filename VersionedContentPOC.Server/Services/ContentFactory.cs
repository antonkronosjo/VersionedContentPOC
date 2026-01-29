using VersionedContentPOC.Data.Enums;
using VersionedContentPOC.Data.Models;

namespace VersionedContentPOC.Server.Services
{
    public interface IContentFactory
    {
        Content CreateInstance(Type contentType, Language language, int? contentId = null, IDictionary<string, ContentPropertyValueDto>? properties = null);
    }

    public class ContentFactory : IContentFactory
    {
        public Content CreateInstance(Type contentType, Language language, int? contentId = null, IDictionary<string, ContentPropertyValueDto>? properties = null)
        {
            ContentTypeRegistry.Guards.IsRegiesteredContentType(contentType);

            var instance = (Content?)Activator.CreateInstance(contentType, language) ?? throw new InvalidOperationException("Failed to create content");

            if (contentId != null && contentId.HasValue)
                instance.ContentId = contentId.Value;

            if (properties != null)
                ContentUpdater.ApplyUpdates(instance, properties);
            
            return instance;
        }
    }
}
