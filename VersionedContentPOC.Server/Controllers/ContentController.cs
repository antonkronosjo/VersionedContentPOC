using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VersionedContentPOC.Data.Enums;
using VersionedContentPOC.Data.Models;
using VersionedContentPOC.Server.Services;

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
    public ActionResult<List<Content>> GetAllContent([FromQuery] Language language)
    {
        var news = _contentRepository
            .QueryActiveVersions<Content>(language)
            .Include(x => x.ContentRoot)
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
            .QueryActiveVersions<Content>(Language.SV)
            .Where(x => x.VersionCreated > fromDate)
            .Include(x => x.ContentRoot)
            .ToList();

        return Ok(latestContent);
    }
}
