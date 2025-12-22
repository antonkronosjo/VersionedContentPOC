using VersionedContentPOC.Server.Attributes;

namespace VersionedContentPOC.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class ContentPropertyMetadataAttribute : Attribute
{
    [ShouldBeRefactored("Should be enum with flags property to make sure all cases can be handeled (some properties maybe only should be editable on creation)")]
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
