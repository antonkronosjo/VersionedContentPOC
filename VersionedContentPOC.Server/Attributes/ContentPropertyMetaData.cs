namespace VersionedContentPOC.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ContentPropertyMetaData : Attribute
    {
        public bool Editable { get; private set; }
        public ContentPropertyMetaData(bool editable = false)
        {
            Editable = editable;
        }
    }
}
