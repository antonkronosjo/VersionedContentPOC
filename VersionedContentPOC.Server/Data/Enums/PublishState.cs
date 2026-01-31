using System.Text.Json.Serialization;

namespace VersionedContentPOC.Server.Data.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PublishState
    {
        Draft = 0,
        Published = 1,
        Unpublished = 2,
    }
}
