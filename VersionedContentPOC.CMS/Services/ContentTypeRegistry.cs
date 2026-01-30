using VersionedContentPOC.CMS.Attributes;
using VersionedContentPOC.CMS.Data.Models;

namespace VersionedContentPOC.CMS.Services;

public static class ContentTypeRegistry
{
    [ShouldBeRefactored("Maybe add cache here to avoid using reflection as much as possible?")]
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



    public static class Guards {
        public static void IsRegiesteredContentType(Type type)
        {
            if (!typeof(Content).IsAssignableFrom(type))
                throw new InvalidOperationException($"Type '{type.FullName}' does not inherit from {nameof(Content)}.");

            if (GetRegisteredContentTypes().SingleOrDefault(x => x == type) == null)
                throw new InvalidOperationException($"Type '{type.FullName}' is not registered by decorating it with ContentType attribute {nameof(Content)}.");
        }
    }
}
