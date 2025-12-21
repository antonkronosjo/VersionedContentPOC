using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using VersionedContentPOC.Data.Enums;
using VersionedContentPOC.Data.Models;
using VersionedContentPOC.Server.Attributes;
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
    [ShouldBeRefactored("Need to compare current version before commiting updates to make sure not multiple people are updating the same content on the same time")]
    public IActionResult UpdateContent([FromBody] UpdateContentRequest request)
    {
        
        var content = _contentRepository.Get<Content>(request.Metadata.ContentId, request.Metadata.Language);
        if (content == null)
            return NotFound();

        if (content.VersionId != request.Metadata.CurrentVersionId)
            throw new Exception("You are currently overwriting some one elses changes");

        var updatedContent = _contentRepository.Update(content, request.PropertiesSchema);
        return Ok(updatedContent);
    }
}
