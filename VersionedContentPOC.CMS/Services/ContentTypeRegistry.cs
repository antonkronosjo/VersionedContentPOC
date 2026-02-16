using System.Reflection;
using VersionedContentPOC.CMS.Attributes;
using VersionedContentPOC.CMS.Data.Models;
namespace VersionedContentPOC.CMS.Services;

public static class ContentTypeRegistry
{
    public static IEnumerable<Type> GetRegisteredTypes()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x =>
                x.IsClass &&
                !x.IsAbstract &&
                typeof(ContentVersion).IsAssignableFrom(x) &&
                x.IsDefined(typeof(ContentVersionAttribute), false));
    }

    public static Type GetRegisteredType(string typeName)
    {
        return GetRegisteredTypes().Single(x => x.Name == typeName);
    }

    public static Type GetInvariantContentType(Type contentType)
    {
        return GetRegisteredTypes().Single(x => x.GetCustomAttribute<SharedContentPropertiesAttribute>()?.ContentType == contentType); ;
    }

    public static class Guards {
        public static void IsRegiesteredContentType(Type type)
        {
            if (!typeof(ContentVersion).IsAssignableFrom(type))
                throw new InvalidOperationException($"Type '{type.FullName}' does not inherit from {nameof(ContentVersion)}.");

            if (GetRegisteredTypes().SingleOrDefault(x => x == type) == null)
                throw new InvalidOperationException($"Type '{type.FullName}' is not registered. Register it by decorating it with ContentType attribute {nameof(ContentVersionAttribute)}.");
        }
    }
}
