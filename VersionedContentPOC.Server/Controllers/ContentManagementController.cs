using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using VersionedContentPOC.Data.Enums;
using VersionedContentPOC.Data.Models;
using VersionedContentPOC.Server.Requests;
using VersionedContentPOC.Server.Services;

namespace VersionedContentPOC.Controllers;

[ApiController]
[Route("api/content")]
public class ContentManagementController : ControllerBase
{
    IContentRepository _contentRepository;
    IContentFactory _contentFactory;

    public ContentManagementController(IContentRepository contentRepository, IContentFactory contentFactory)
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
            var contentType = ContentTypeRegistry.GetRegisteredContentType(request.Metadata.ContentTypeName);
            var contentInstance = _contentFactory.CreateInstance(contentType, request.Metadata.Language, request.PropertiesSchema);
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

    [HttpGet]
    [Route("updateschema")]
    public IActionResult GetContentUpdateSchema([FromQuery] Guid contentId, [FromQuery] Language language)
    {
        var content = _contentRepository.Get<Content>(contentId, language);
        if (content == null)
            return NotFound();

        var updateSchema = ContentMetadataProvider.GetUpdateSchema(content);
        return Ok(updateSchema);
    }

    [HttpPut]
    [Route("update")]
    public IActionResult UpdateContent([FromBody] UpdateContentRequest request)
    {
        
        var content = _contentRepository.Get<Content>(request.Metadata.ContentId, request.Metadata.Language);
        if (content == null)
            return NotFound();

        var updatedContent = _contentRepository.Update(content, request.PropertiesSchema, request.Metadata.ForceUpdate);
        return Ok(updatedContent);
    }

    [HttpDelete]
    [Route("delete")]
    public IActionResult DeleteContent([FromQuery] Guid contentId)
    {
        _contentRepository.Delete(contentId);
        return NoContent();
    }
}
