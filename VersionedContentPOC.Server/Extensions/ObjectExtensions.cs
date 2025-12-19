using VersionedContentPOC.Attributes;
using System.Reflection;

namespace VersionedContentPOC.Data.Extensions
{
    public static class ObjectExtensions
    {
        public static T ApplyUpdates<T>(this T content, Dictionary<string, string?> updates) where T : class {
            var properties = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite && p.CanRead);

            foreach (var propertyToUpdate in updates)
            {
                var instanceProperty = properties.FirstOrDefault(x => x.Name == propertyToUpdate.Key);

                if (instanceProperty == null)
                    throw new KeyNotFoundException($"Content of type {typeof(T)} does not contain a property named \"{propertyToUpdate.Key}\"");

                var attr = instanceProperty.GetCustomAttribute<ContentPropertyMetaData>(inherit: false);
                if (attr == null || !attr.Editable)
                    throw new UnauthorizedAccessException($"Property \"{propertyToUpdate.Key}\" is not editable");

                //Todo: add mapping logic to handle more complex types
                if (instanceProperty.PropertyType.IsAssignableFrom(propertyToUpdate.Value?.GetType()))
                    instanceProperty.SetValue(content, propertyToUpdate.Value);
                else if (instanceProperty.PropertyType.IsValueType)
                    instanceProperty.SetValue(content, Convert.ChangeType(propertyToUpdate.Value, instanceProperty.PropertyType));
            }
            return content;
        }
    }
}
