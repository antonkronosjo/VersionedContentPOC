using VersionedContentPOC.Data.Enums;

namespace VersionedContentPOC.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ContentTypeAttribute : Attribute
    {
        public ContentType Discriminator { get; private set; }
        public ContentTypeAttribute(ContentType discriminator)
        {
            Discriminator = discriminator; 
        }
    }
}
