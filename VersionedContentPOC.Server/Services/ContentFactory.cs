using VersionedContentPOC.Data.Enums;
using VersionedContentPOC.Data.Models;

namespace VersionedContentPOC.Server.Services
{
    public interface IContentFactory
    {
        Content CreateInstance(Type contentType, Language language, IDictionary<string, ContentPropertyValueDto> properties);
    }

    public class ContentFactory : IContentFactory
    {
        public Content CreateInstance(Type contentType, Language language, IDictionary<string, ContentPropertyValueDto> properties)
        {
            if (!typeof(Content).IsAssignableFrom(contentType))
                throw new InvalidOperationException($"Type '{contentType.FullName}' does not inherit from {nameof(Content)}.");

            var instance = (Content?)Activator.CreateInstance(contentType, Guid.NewGuid(), language) ?? throw new InvalidOperationException("Failed to create content");
            ContentUpdater.ApplyUpdates(instance, properties);
            return instance;
        }
    }
}
