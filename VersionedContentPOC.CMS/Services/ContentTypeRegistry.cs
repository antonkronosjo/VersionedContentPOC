using System.Reflection;
using VersionedContentPOC.CMS.Attributes;
using VersionedContentPOC.CMS.Data.Models;
namespace VersionedContentPOC.CMS.Services;

public static class ContentTypeRegistry
{
    public static class LocalizedVersions
    {
        [ShouldBeRefactored("Maybe add cache here to avoid using reflection as much as possible?")]
        public static IEnumerable<Type> GetRegisteredTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x =>
                    x.IsClass &&
                    !x.IsAbstract &&
                    typeof(LocalizableVersion).IsAssignableFrom(x) &&
                    x.IsDefined(typeof(ContentTypeAttribute), false));
        }

        public static Type GetRegisteredType(string typeName)
        {
            var contentType = GetRegisteredTypes().SingleOrDefault(x => x.Name == typeName);
            if (contentType == null)
                throw new KeyNotFoundException($"Content type '{typeName}' is not registered.");
            return contentType;
        }
    }

    public static class InvariantVersions
    {
        public static IEnumerable<Type> GetRegisteredTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x =>
                    x.IsClass &&
                    !x.IsAbstract &&
                    typeof(InvariantVersion).IsAssignableFrom(x) &&
                    x.IsDefined(typeof(SharedContentPropertiesAttribute), false));
        }

        public static Type GetRegisteredType(Type contentType)
        {
            var test = InvariantVersions.GetRegisteredTypes();
            return test.Single(x => x.GetCustomAttribute<SharedContentPropertiesAttribute>()?.ContentType == contentType);
        }
    }

    public static class Guards {
        public static void IsRegiesteredContentType(Type type)
        {
            if (!typeof(LocalizableVersion).IsAssignableFrom(type))
                throw new InvalidOperationException($"Type '{type.FullName}' does not inherit from {nameof(LocalizableVersion)}.");

            if (LocalizedVersions.GetRegisteredTypes().SingleOrDefault(x => x == type) == null)
                throw new InvalidOperationException($"Type '{type.FullName}' is not registered by decorating it with ContentType attribute {nameof(LocalizableVersion)}.");
        }
    }
}
