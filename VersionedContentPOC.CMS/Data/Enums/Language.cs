using System.Text.Json.Serialization;
namespace VersionedContentPOC.CMS.Data.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Language
{
    SV = 1,
    EN = 2
}
