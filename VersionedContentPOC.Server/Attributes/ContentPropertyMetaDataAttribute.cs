namespace VersionedContentPOC.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class ContentPropertyMetadataAttribute : Attribute
{
    public bool Editable { get; private set; }
    public bool Required { get; private set; }
    public Editor PropertyEditor { get; private set; }

    public ContentPropertyMetadataAttribute(bool editable = false, bool required = false, Editor propertyEditor = Editor.Input)
    {
        Editable = editable;
        Required = required;
    }
}


public enum Editor {
    Input,
    TextArea,
    Date,
    DateTime
}
