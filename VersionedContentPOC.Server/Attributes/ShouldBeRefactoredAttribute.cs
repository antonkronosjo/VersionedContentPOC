namespace VersionedContentPOC.Server.Attributes;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public class ShouldBeRefactoredAttribute : Attribute
{
    public ShouldBeRefactoredAttribute(string reason)
    {
        
    }
}
