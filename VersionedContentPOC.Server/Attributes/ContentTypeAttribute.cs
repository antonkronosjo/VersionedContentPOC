using VersionedContentPOC.Data.Enums;

namespace VersionedContentPOC.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ContentTypeAttribute : Attribute
    {
        public ContentTypeAttribute()
        {
            
        }
    }
}
