namespace VersionedContentPOC.CMS.Attributes;

public class SharedContentPropertiesAttribute : Attribute
{
    public Type ContentType { get; set; }
    public SharedContentPropertiesAttribute(Type type)
    {
        ContentType = type;
    }
}
