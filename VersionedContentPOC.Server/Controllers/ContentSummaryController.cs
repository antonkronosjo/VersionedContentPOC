using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VersionedContentPOC.CMS.Attributes;
using VersionedContentPOC.CMS.Extensions;
using VersionedContentPOC.CMS.Requests;
using VersionedContentPOC.CMS.Services;
using VersionedContentPOC.Server.Mappers;

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
    [ShouldBeRefactored("This query is very un-optimized")]
    public IActionResult GetRootSummaries([FromQuery] GetRootSummariesRequest request)
    {
        var utcNow = DateTime.UtcNow;

        var contentRoots = _contentRepository.QueryRoots()
            .Include(x => x.LanguageBranches)
                .ThenInclude(x => x.Versions)
            .WhereIf(request.Published == true, x => x.StartPublish < utcNow && (x.StopPublish == null || x.StopPublish > utcNow))
            .ToList()
            .ToSummary(_contentRepository)
            .Where(x => String.IsNullOrEmpty(request.ContentType) || request.ContentType == x.ContentTypeName)
            .Where(x => request.Language == null || x.LanguageVersions.Contains(request.Language.Value))
            .OrderByDescending(x => x.Created)
            .ToList();

        return Ok(contentRoots);
    }
}
