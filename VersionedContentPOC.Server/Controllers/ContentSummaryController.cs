using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VersionedContentPOC.Data.Enums;
using VersionedContentPOC.Data.Models;
using VersionedContentPOC.Server.Attributes;
using VersionedContentPOC.Server.Controllers.Requests;
using VersionedContentPOC.Server.Extensions;
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

    [HttpGet]
    [Route("contentrootsv2")]
    [ProducesResponseType(typeof(List<ContentRootSummary>), StatusCodes.Status200OK)]
    [ShouldBeRefactored("This query is very un-optimized")]
    public IActionResult GetRootSummariesV2([FromQuery] ContentQueryRequest request)
    {
        var contentRoots = _contentRepository.Query<Content>(Language.SV)
            .FilterByContentQueryRequest(request)
            .OrderByContentQueryRequest(request)
            .PaginateByContentQueryRequest(request)
            .Select(x => x.ContentRoot)
                .Include(x => x.LanguageBranches)
                    .ThenInclude(x => x.Versions)
            .ToSummary(_contentRepository)
            .ToList();

        return Ok(contentRoots);
    }
}
