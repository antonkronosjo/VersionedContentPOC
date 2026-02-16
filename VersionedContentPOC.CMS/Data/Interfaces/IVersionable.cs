namespace VersionedContentPOC.CMS.Data.Interfaces;

public interface IVersionable
{
    int ContentId { get; set; }
    int VersionId { get; set; }
    DateTime VersionCreated { get; set; }
}
