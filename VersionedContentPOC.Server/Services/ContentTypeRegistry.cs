using VersionedContentPOC.Attributes;
using VersionedContentPOC.Data.Models;

namespace VersionedContentPOC.Server.Services;

public static class ContentTypeRegistry
{
    public static IEnumerable<Type> GetRegisteredContentTypes()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x =>
                x.IsClass &&
                !x.IsAbstract &&
                typeof(Content).IsAssignableFrom(x) &&
                x.IsDefined(typeof(ContentTypeAttribute), false));
    }
    
    public static Type GetRegisteredContentType(string typeName)
    {
        var contentType = GetRegisteredContentTypes().SingleOrDefault(x => x.Name == typeName);
        if (contentType == null)
            throw new KeyNotFoundException($"Content type '{typeName}' is not registered.");
        return contentType;
    }
}
