using VersionedContentPOC.Server.Attributes;

namespace VersionedContentPOC.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class ContentPropertyMetadataAttribute : Attribute
{
    public bool Editable { get; private set; }

    public bool Required { get; private set; }

    [ShouldBeRefactored("This property is not used")]
    public Editor PropertyEditor { get; private set; }

    public ContentPropertyMetadataAttribute(bool editable = false, bool required = false, Editor editor = Editor.Input)
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
