using System.Text.Json.Serialization;

namespace VersionedContentPOC.CMS.Data.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum InputType
{
    None = 0,
    Input = 1,
    TextArea = 2,
    DatePicker = 3,
    DateTimePicker = 4,
    Select = 5,
}
