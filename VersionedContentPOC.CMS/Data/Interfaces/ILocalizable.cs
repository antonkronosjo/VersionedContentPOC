using VersionedContentPOC.CMS.Data.Enums;

namespace VersionedContentPOC.CMS.Data.Interfaces;

public interface ILocalizable
{
    Language Language { get; set; }
}
