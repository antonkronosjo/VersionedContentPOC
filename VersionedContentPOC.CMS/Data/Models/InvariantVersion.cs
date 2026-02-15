using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using VersionedContentPOC.CMS.Data.Interfaces;

namespace VersionedContentPOC.CMS.Data.Models
{
    public abstract class InvariantVersion : IContentVersion//<T> : IContentVersion where T : Content
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
