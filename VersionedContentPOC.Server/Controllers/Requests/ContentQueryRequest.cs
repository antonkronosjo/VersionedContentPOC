using VersionedContentPOC.Data.Enums;
using VersionedContentPOC.Server.Data.Enums;

namespace VersionedContentPOC.Server.Controllers.Requests
{
    public class ContentQueryRequest
    {
        public List<Language> Languages { get; set; } = new();
        public List<string> ContentTypeNames { get; set; } = new();
        public PublishState? PublishState { get; set; }
        public int? Skip { get; set; }  
        public int? Take { get; set; } 
        public List<SortField>? Sort { get; set; }
    }

    public class SortField
    {
        public string Field { get; set; } = default!;
        public bool Descending { get; set; } = false;
    }
}
