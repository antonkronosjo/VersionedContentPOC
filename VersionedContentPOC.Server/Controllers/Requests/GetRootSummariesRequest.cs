namespace VersionedContentPOC.Server.Controllers.Requests
{
    public class GetRootSummariesRequest
    {
        public string ContentType { get; set; }
        public bool Published { get; set; }
    }
}
