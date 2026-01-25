using System.Text.Json.Serialization;

namespace VersionedContentPOC.Data.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Language
    {
        SV = 1,
        EN = 2
    }
}
