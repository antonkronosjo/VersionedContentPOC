namespace VersionedContentPOC.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ContentPropertyMetaDataAttribute : Attribute
    {
        public bool Editable { get; private set; }
        public bool Required { get; private set; }
        public ContentPropertyMetaDataAttribute(bool editable = false, bool required = false)
        {
            Editable = editable;
            Required = required;
        }
    }
}
