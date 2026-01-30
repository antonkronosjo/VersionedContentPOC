
using VersionedContentPOC.CMS.Data.Enums;

namespace VersionedContentPOC.CMS.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class ContentPropertyMetadataAttribute : Attribute
{
    [ShouldBeRefactored("Should be enum with flags property to make sure all cases can be handeled (some properties maybe only should be editable on creation etc)")]
    public bool Editable { get; private set; }

    public InputType PropertyInputType { get; private set; }

    public ContentPropertyMetadataAttribute(bool editable = false, InputType inputType = InputType.Input)
    {
        Editable = editable;
        PropertyInputType = inputType;
    }
}
