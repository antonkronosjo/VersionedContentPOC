using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VersionedContentPOC.CMS.Data.Models
{
    public abstract class SharedContentProperties
    {
        [Required]
        [JsonIgnore]
        public int Id { get; set; }

        [Required]
        [JsonIgnore]
        public int ContentId { get; set; }

        [JsonIgnore]
        public ContentRoot ContentRoot { get; set; }
    }
}
