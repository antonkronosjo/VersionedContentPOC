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
[Produces("application/json")]
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
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    public ActionResult<List<string>> GetContentTypes()
    {
        var contentTypes = ContentTypeRegistry.GetRegisteredContentTypes()
            .Select(x => x.Name)
            .ToList();

        return Ok(contentTypes);
    }

    [HttpGet]
    [Route("creationschema")]
    [ProducesResponseType(typeof(CreateContentRequest), StatusCodes.Status200OK)]
    public ActionResult<CreateContentRequest> GetContentCreationSchema([FromQuery] string contentTypeName, [FromQuery] Language language)
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
    [ProducesResponseType(typeof(Content), StatusCodes.Status200OK)]
    public ActionResult<Content> CreateContent([FromBody] CreateContentRequest request)
    {
        try
        {
            var contentType = ContentTypeRegistry.GetRegisteredContentType(request.Metadata.ContentTypeName);
            var contentInstance = _contentFactory.CreateInstance(contentType, request.Metadata.Language, properties: request.PropertiesSchema);
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
    [ProducesResponseType(typeof(UpdateContentRequest), StatusCodes.Status200OK)]
    [ShouldBeRefactored("Logic regarding initializing translation vs updating in same language branch should no be inside controller")]
    public ActionResult<UpdateContentRequest> GetContentUpdateSchema([FromQuery] Guid contentId, [FromQuery] Language language)
    {
        var contentLanguages = _contentRepository.GetTranslatedLanguages(contentId);
        if (contentLanguages.Contains(language))
        {
            var content = _contentRepository.Get<Content>(contentId, language);
            if (content == null)
                return NotFound();

            var updateSchema = ContentMetadataProvider.GetUpdateSchema(content, contentLanguages);
            return Ok(updateSchema);
        }
        else
        {
            var contentType = _contentRepository.GetContentRootType(contentId);
            var contentInstance = _contentFactory.CreateInstance(contentType, language, contentId: contentId);
            var updateSchema = ContentMetadataProvider.GetUpdateSchema(contentInstance, contentLanguages);
            return Ok(updateSchema);
        }
    }

    [HttpPut]
    [Route("update")]
    [ProducesResponseType(typeof(Content), StatusCodes.Status200OK)]
    public ActionResult<Content> UpdateContent([FromBody] UpdateContentRequest request)
    {
        var contentExists = _contentRepository.Exists(request.Metadata.ContentId);
        if (!contentExists)
            return NotFound();

        var content = _contentRepository.Get<Content>(request.Metadata.ContentId, request.Metadata.Language);
        if (content != null)
        {
            var updatedContent = _contentRepository.Update(content, request.PropertiesSchema, request.Metadata.ForceUpdate);
            return Ok(updatedContent);
        }
        else
        {
            var contentType = _contentRepository.GetContentRootType(request.Metadata.ContentId);
            content = _contentFactory.CreateInstance(contentType, request.Metadata.Language, contentId: request.Metadata.ContentId);
            var updatedContent = _contentRepository.Update(content, request.PropertiesSchema, request.Metadata.ForceUpdate);
            return Ok(updatedContent);
        }
    }

    [HttpGet]
    [Route("versions")]
    [ProducesResponseType(typeof(List<Content>), StatusCodes.Status200OK)]
    public ActionResult<List<Content>> Versions([FromQuery] Guid contentId, Language language)
    {

        var contentVersions = _contentRepository
            .Versions<Content>(contentId, language)
            .OrderByDescending(x => x.VersionCreated)
            .ToList();

        return Ok(contentVersions);
    }

    [HttpPut]
    [Route("setasactive")]
    public IActionResult SetAsActiveVersion([FromQuery] Guid versionId)
    {
        _contentRepository.SetAsActiveVersion(versionId);
        return Ok();
    }

    [HttpDelete]
    [Route("delete")]
    public IActionResult DeleteContent([FromQuery] Guid contentId)
    {
        _contentRepository.Delete(contentId);
        return NoContent();
    }
}
