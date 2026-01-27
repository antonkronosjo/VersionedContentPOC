using System.Reflection;
using VersionedContentPOC.Attributes;

namespace VersionedContentPOC.Server.Extensions
{
    public static class ContentTypeExtensions
    {
        public static IEnumerable<PropertyInfo> GetContentProperties(this Type type)
        {
            return type
                .GetPublicInstanceProperties()
                .Where(x => x.IsDefined(typeof(ContentPropertyMetadataAttribute), true));
        }
    }
}
