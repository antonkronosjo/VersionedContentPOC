using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VersionedContentPOC.CMS.Data.Enums;
using VersionedContentPOC.CMS.Data.Models;
using VersionedContentPOC.CMS.Services;

namespace VersionedContentPOC.CMS.Controllers;

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
    [Route("get")]
    [ProducesResponseType(typeof(Content), StatusCodes.Status200OK)]
    public ActionResult<Content> GetById([FromQuery] int contentId, [FromQuery] Language language)
    {
        var utcNow = DateTime.UtcNow;
        var content = _contentRepository
            .Query<Content>(language)
            .FirstOrDefault(x => x.ContentId == contentId);

        return Ok(content);
    }

    [HttpGet]
    [Route("all")]
    [ProducesResponseType(typeof(List<Content>), StatusCodes.Status200OK)]
    public ActionResult<List<Content>> GetAllContent([FromQuery] Language language)
    {
        var utcNow = DateTime.UtcNow;
        var news = _contentRepository
            .Query<Content>(language)
            .Include(x => x.ContentRoot)
            .OrderByDescending(x => x.ContentRoot.Created)
            .ToList();

        return Ok(news);
    }
}
