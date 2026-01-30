using System.Reflection;
using VersionedContentPOC.CMS.Attributes;

namespace VersionedContentPOC.CMS.Extensions
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
