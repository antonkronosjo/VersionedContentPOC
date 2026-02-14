using System.Reflection;

namespace VersionedContentPOC.CMS.Extensions
{
    public static class TypeExtensions
    {
        public static IEnumerable<PropertyInfo> GetPublicInstanceProperties(this Type type)
        {
            return type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite && p.CanRead);
        }
    }
}
