using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VersionedContentPOC.Server.Controllers.Requests;
using VersionedContentPOC.Server.Mappers;
using VersionedContentPOC.Server.Services;

namespace VersionedContentPOC.Controllers;

[ApiController]
[Route("api/contentsummary")]
[Produces("application/json")]
public class ContentSummaryController : ControllerBase
{
    IContentRepository _contentRepository;

    public ContentSummaryController(IContentRepository contentRepository)
    {
        _contentRepository = contentRepository;
    }

    [HttpGet]
    [Route("contentroots")]
    [ProducesResponseType(typeof(List<ContentRootSummary>), StatusCodes.Status200OK)]
    public IActionResult GetRootSummaries([FromQuery] GetRootSummariesRequest request)
    {
        var utcNow = DateTime.UtcNow;
        var contentRoots = _contentRepository.QueryRoots()
            .Include(x => x.LanguageBranches)
                .ThenInclude(x => x.Versions)
            .Where(x => x.StartPublish < utcNow && (x.StopPublish == null || x.StopPublish > utcNow))
            .ToList()
            .ToSummary(_contentRepository)
            .ToList();

        return Ok(contentRoots);
    }
}
