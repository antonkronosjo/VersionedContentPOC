using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using VersionedContentPOC.CMS.Data.Interfaces;
namespace VersionedContentPOC.CMS.Data.Models;

public abstract class InvariantVersion : ContentVersion, IVersionable
{
    public Collection<LocalizableVersion> LocalizedVersions { get; set; }
}
