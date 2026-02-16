using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using VersionedContentPOC.CMS.Data.Interfaces;
namespace VersionedContentPOC.CMS.Data.Models;

public abstract class InvariantVersion : ContentVersion, IVersionable//<T> : IContentVersion where T : Content
{

}
