namespace VersionedContentPOC.CMS.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class MainLanguageOnlyAttribute : Attribute
    {
        public bool IsActive { get; set; }
        public MainLanguageOnlyAttribute(bool isActive = true)
        {
            IsActive = isActive;
        }
    }
}
