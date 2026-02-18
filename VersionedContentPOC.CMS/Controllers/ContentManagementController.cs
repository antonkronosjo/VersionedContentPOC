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
        var contentTypes = ContentTypeRegistry.GetRegisteredTypes()
            .Where(x => typeof(LocalizableVersion).IsAssignableFrom(x))
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
            var contentType = ContentTypeRegistry.GetRegisteredType(contentTypeName);
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
    [ProducesResponseType(typeof(LocalizableVersion), StatusCodes.Status200OK)]
    public ActionResult<LocalizableVersion> CreateContent([FromBody] CreateContentRequest request)
    {
        var contentType = ContentTypeRegistry.GetRegisteredType(request.Metadata.ContentTypeName);
        var contentInstance = _contentFactory.CreateContentInstance(contentType, request.Metadata.Language, properties: request.PropertiesSchema);

        var sharedPropertiesType = ContentTypeRegistry.GetInvariantContentType(contentType);
        var sharedProperties = _contentFactory.CreateSharedPropertiesInstance(sharedPropertiesType);

        var createdContent = _contentRepository.Create(contentInstance, sharedProperties);
        return CreatedAtAction(nameof(CreateContent), createdContent);
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
            var content = versionId.HasValue
                ? _contentVersionRepository.GetVersion<LocalizableVersion>(versionId.Value)
                : _contentRepository.Get<LocalizableVersion>(contentId, language);

            if (content == null)
                return NotFound();

            var updateSchema = ContentMetadataProvider.GetUpdateSchema(content, contentLanguages);
            return Ok(updateSchema);
        }
        else
        {
            var contentType = _contentRepository.GetContentRootType(contentId);
            var contentInstance = _contentFactory.CreateContentInstance(contentType, language, contentId: contentId);

            var updateSchema = ContentMetadataProvider.GetUpdateSchema(contentInstance, contentLanguages);
            return Ok(updateSchema);
        }
    }

    [HttpPut]
    [Route("update")]
    [ProducesResponseType(typeof(LocalizableVersion), StatusCodes.Status200OK)]
    public ActionResult<LocalizableVersion> UpdateContent([FromBody] UpdateContentRequest request)
    {
        var contentExists = _contentRepository.Exists(request.Metadata.ContentId);
        if (!contentExists)
            return NotFound();

        if (request.Metadata.Language == Language.Invariant)
        {
            var contentVersion = _contentVersionRepository.GetVersion<InvariantVersion>(request.Metadata.VersionId);
            var updatedContent = _contentRepository.Update(contentVersion, request.PropertiesSchema, request.Metadata.ForceNewVersion);
        }

        var contentRoot = _contentRepository
            .QueryRoots()
            .Include(x => x.Versions)
            .Single(x => x.ContentId == request.Metadata.ContentId);

        if (contentRoot.Versions.Any(x => x.Language == request.Metadata.Language))
        {
            var contentVersion = _contentVersionRepository.GetVersion<LocalizableVersion>(request.Metadata.VersionId);
            var updatedContent = _contentRepository.Update(contentVersion, request.PropertiesSchema, request.Metadata.ForceNewVersion);
            return Ok(updatedContent);
        }
        else
        {
            var contentType = _contentRepository.GetContentRootType(request.Metadata.ContentId);
            var content = _contentFactory.CreateContentInstance(contentType, request.Metadata.Language, contentId: request.Metadata.ContentId);
            var updatedContent = _contentRepository.Update(content, request.PropertiesSchema, request.Metadata.ForceNewVersion);
            return Ok(updatedContent);
        }
    }

    [HttpGet]
    [Route("versions")]
    [ProducesResponseType(typeof(List<LocalizableVersion>), StatusCodes.Status200OK)]
    public ActionResult<List<LocalizableVersion>> Versions([FromQuery] int contentId, Language language)
    {
        if (language == Language.Invariant)
        {
            var contentVersions = _contentVersionRepository
                .QueryVersions<InvariantVersion>()
                .Where(x => x.ContentId == contentId)
                .OrderByDescending(x => x.VersionCreated)
                .ToList();
            return Ok(contentVersions);
        }
        else
        {
            var contentVersions = _contentVersionRepository
                .QueryVersions<LocalizableVersion>()
                .Where(x => x.ContentId == contentId && x.Language == language)
                .OrderByDescending(x => x.VersionCreated)
                .ToList();
            return Ok(contentVersions);
        }
    }

    [HttpPut]
    [Route("publish")]
    public IActionResult Publish([FromQuery] int versionId)
    {
        var version = _contentVersionRepository.GetVersion<LocalizableVersion>(versionId);
        _contentPublishingService.Publish(version);
        return Ok();
    }

    [HttpPut]
    [Route("unpublish")]
    public IActionResult UnPublish([FromQuery] int versionId)
    {
        var version = _contentVersionRepository.GetVersion<LocalizableVersion>(versionId);
        _contentPublishingService.Unpublish(version);
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
