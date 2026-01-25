namespace VersionedContentPOC.Server.Attributes
{
    //[AttributeUsage(AttributeTargets.Property, Inherited = false)]
    //[ShouldBeRefactored("Is this even correctly spelled? Maybe rename this to MainLanguageOnly or something similar")]
    //public class CulturalSpecificAttribute : Attribute
    //{
    //    public bool IsCulturalSpecific { get; set; }
    //    public CulturalSpecificAttribute(bool culturalSpecific = true)
    //    {
    //        IsCulturalSpecific = culturalSpecific;
    //    }
    //}

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
