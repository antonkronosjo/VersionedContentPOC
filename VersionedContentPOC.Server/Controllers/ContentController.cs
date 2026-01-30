using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VersionedContentPOC.CMS.Data.Models;
using VersionedContentPOC.CMS.Data.Enums;
using VersionedContentPOC.CMS.Services;
using VersionedContentPOC.CMS.Extensions;

namespace VersionedContentPOC.Controllers;

[ApiController]
[Route("api/content")]
[Produces("application/json")]
public class ContentController : ControllerBase
{
    IContentRepository _contentRepository;

    public ContentController(IContentRepository contentRepository)
    {
        _contentRepository = contentRepository;
    }

    [HttpGet]
    [Route("all")]
    [ProducesResponseType(typeof(List<Content>), StatusCodes.Status200OK)]
    public ActionResult<List<Content>> GetAllContent([FromQuery] Language language, [FromQuery] bool published)
    {
        var utcNow = DateTime.UtcNow;
        var news = _contentRepository
            .Query<Content>(language)
            .Include(x => x.ContentRoot)
            .WhereIf(published, x => x.ContentRoot.StartPublish < utcNow)
            .OrderByDescending(x => x.ContentRoot.Created)
            .ToList();

        return Ok(news);
    }

    [HttpGet]
    [Route("latest")]
    public IActionResult GetLatestContent()
    {
        var fromDate = DateTime.UtcNow.AddMinutes(-1);
        var latestContent = _contentRepository
            .Query<Content>(Language.SV)
            .Where(x => x.VersionCreated > fromDate)
            .Include(x => x.ContentRoot)
            .ToList();

        return Ok(latestContent);
    }
}
