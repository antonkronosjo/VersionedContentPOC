using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VersionedContentPOC.Data.Enums;
using VersionedContentPOC.Data.Models;
using VersionedContentPOC.Server.Services;

namespace VersionedContentPOC.Controllers;

[ApiController]
[Route("api/content")]
public class ContentController : ControllerBase
{
    IContentRepository _contentRepository;

    public ContentController(IContentRepository contentRepository)
    {
        _contentRepository = contentRepository;
    }

    [HttpGet]
    [Route("all")]
    public IActionResult GetAllContent()
    {
        var news = _contentRepository
            .QueryActiveVersions<Content>(Language.SV)
            .Include(x => x.ContentRoot)
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
