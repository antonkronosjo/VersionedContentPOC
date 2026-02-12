using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using VersionedContentPOC.CMS.Attributes;
using VersionedContentPOC.CMS.Data.Enums;
using VersionedContentPOC.CMS.Data.Models;
using VersionedContentPOC.CMS.Requests;
using VersionedContentPOC.CMS.Services;

namespace VersionedContentPOC.CMS.Controllers;

[ApiController]
[Route("api/content")]
[Produces("application/json")]
public class ContentManagementController : ControllerBase
{
    IContentRepository _contentRepository;
    IContentFactory _contentFactory;
    IContentVersionRepository _contentVersionRepository;
    IContentPublishingService _contentPublishingService;

    public ContentManagementController(IContentRepository contentRepository,
        IContentFactory contentFactory, IContentVersionRepository contentVersionRepository, IContentPublishingService contentPublishingService)
    {
        _contentRepository = contentRepository;
        _contentFactory = contentFactory;
        _contentVersionRepository = contentVersionRepository;
        _contentPublishingService = contentPublishingService;
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
    public ActionResult<UpdateContentRequest> GetContentUpdateSchema([FromQuery] int contentId, [FromQuery] Language language, [FromQuery] int? versionId = null)
    {
        var contentLanguages = _contentRepository.GetTranslatedLanguages(contentId);
        if (contentLanguages.Contains(language))
        {
            var content = versionId == null
                ? _contentRepository.Get<Content>(contentId, language)
                : _contentVersionRepository.GetVersion<Content>(contentId, versionId.Value, language);

            if (content == null)
                return NotFound();

            var updateSchema = ContentMetadataProvider.GetUpdateSchema(content, content.ContentRoot, contentLanguages);
            return Ok(updateSchema);
        }
        else
        {
            var contentRoot = _contentRepository.QueryRoots().Single(x => x.ContentId == contentId);
            var contentType = _contentRepository.GetContentRootType(contentId);
            var contentInstance = _contentFactory.CreateInstance(contentType, language, contentId: contentId);
            var updateSchema = ContentMetadataProvider.GetUpdateSchema(contentInstance, contentRoot, contentLanguages);
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
        
        var contentRoot = _contentRepository
            .QueryRoots()
            .Include(x => x.Versions)
            .Single(x => x.ContentId == request.Metadata.ContentId);

        if (contentRoot.Versions.Any(x => x.Language == request.Metadata.Language))
        {
            var contentVersion = _contentVersionRepository.GetVersion<Content>(
                request.Metadata.ContentId,
                request.Metadata.VersionId,
                request.Metadata.Language
);
            var updatedContent = _contentRepository.Update(contentVersion, request.PropertiesSchema, request.Metadata.ForceNewVersion);
            return Ok(updatedContent);
        }
        else
        {
            var contentType = _contentRepository.GetContentRootType(request.Metadata.ContentId);
            var content = _contentFactory.CreateInstance(contentType, request.Metadata.Language, contentId: request.Metadata.ContentId);
            var updatedContent = _contentRepository.Update(content, request.PropertiesSchema, request.Metadata.ForceNewVersion);
            return Ok(updatedContent);
        }
    }

    [HttpGet]
    [Route("versions")]
    [ProducesResponseType(typeof(List<Content>), StatusCodes.Status200OK)]
    public ActionResult<List<Content>> Versions([FromQuery] int contentId, Language language)
    {

        var contentVersions = _contentVersionRepository
            .QueryVersions<Content>(language)
            .Where(x => x.ContentId == contentId)
            .OrderByDescending(x => x.VersionCreated)
            .ToList();

        return Ok(contentVersions);
    }

    [HttpPut]
    [Route("publish")]
    public IActionResult Publish([FromQuery] int versionId)
    {
        _contentPublishingService.Publish(versionId);
        return Ok();
    }

    [HttpPut]
    [Route("unpublish")]
    public IActionResult UnPublish([FromQuery] int versionId)
    {
        _contentPublishingService.Unpublish(versionId);
        return Ok();
    }

    [HttpDelete]
    [Route("delete")]
    public IActionResult DeleteContent([FromQuery] int contentId)
    {
        _contentRepository.Delete(contentId);
        return NoContent();
    }
}
