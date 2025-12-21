using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using VersionedContentPOC.Data.Enums;
using VersionedContentPOC.Data.Models;
using VersionedContentPOC.Server.Data.Models;
using VersionedContentPOC.Server.Requests;
using VersionedContentPOC.Server.Services;

namespace VersionedContentPOC.Controllers;

[ApiController]
[Route("api/content")]
public class ContentController : ControllerBase
{
    IContentRepository _contentRepository;
    IContentFactory _contentFactory;

    public ContentController(IContentRepository contentRepository, IContentFactory contentFactory)
    {
        _contentRepository = contentRepository;
        _contentFactory = contentFactory;
    }

    [HttpGet]
    [Route("types")]
    public IActionResult GetContentTypes()
    {
        var contentTypes = ContentTypeRegistry.GetRegisteredContentTypes()
            .Select(x => x.Name)
            .ToList();

        return Ok(contentTypes);
    }

    [HttpGet]
    [Route("creationschema")]
    public IActionResult GetContentCreationSchema([FromQuery] string contentTypeName, [FromQuery] Language language)
    {
        try
        {
            var contentType = ContentTypeRegistry.GetRegisteredContentType(contentTypeName);
            var creationSchema = ContentMetadataProvider.GetCreationSchema(contentType, language);
            return Ok(creationSchema);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    [Route("create")]
    public IActionResult CreateContent([FromBody] CreateContentRequest request)
    {
        try
        {
            var contentType = ContentTypeRegistry.GetRegisteredContentType(request.ContentTypeName);
            var contentInstance = _contentFactory.CreateInstance(contentType, request.Language, request.Properties);
            var createdContent = _contentRepository.Create(contentInstance);
            return CreatedAtAction(nameof(CreateContent), createdContent);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPut]
    [Route("update")]
    public IActionResult UpdateContent([FromBody] UpdateContentRequest request)
    {
        var content = _contentRepository.Get<Content>(request.ContentId, request.Language);
        if (content == null)
            return NotFound();

        var updatedContent = _contentRepository.Update<Content>(request.ContentId, request.Language, request.Updates);
        return Ok(updatedContent);
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
    public IActionResult GetLatestNews()
    {
        var fromDate = DateTime.UtcNow.AddMinutes(-1);
        var latestNews = _contentRepository
            .QueryActiveVersions<NewsContent>(Language.SV)
            .Where(x => x.VersionCreated > fromDate)
            .Include(x => x.ContentRoot)
            .ToList();

        return Ok(latestNews);
    }
}
