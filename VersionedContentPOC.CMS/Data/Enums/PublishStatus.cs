using System.Text.Json.Serialization;
namespace VersionedContentPOC.CMS.Data.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PublishStatus
{
    Draft = 1,
    Published = 2,
    DelayedPublish = 3,
    Unpublished = 4
}
