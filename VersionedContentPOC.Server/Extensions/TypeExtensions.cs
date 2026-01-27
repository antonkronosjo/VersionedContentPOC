using System;
using System.Reflection;
using VersionedContentPOC.Attributes;
using VersionedContentPOC.Data.Models;
using VersionedContentPOC.Server.Attributes;

namespace VersionedContentPOC.Server.Extensions
{
    public static class TypeExtensions
    {
        public static IEnumerable<PropertyInfo> GetPublicInstanceProperties(this Type type)
        {
            return type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite && p.CanRead);
        }

        public static IEnumerable<PropertyInfo> FilterByAttribute<T>(this IEnumerable<PropertyInfo> propertyInfos, Func<T?, bool> predicate) where T : Attribute
        {
            return propertyInfos
                .Where(p =>
                {
                    var attr = p.GetCustomAttribute<T>(inherit: true);
                    return predicate(attr);
                });
        }
    }
}
