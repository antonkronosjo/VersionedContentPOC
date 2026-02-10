using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using VersionedContentPOC.CMS.Extensions;
using VersionedContentPOC.CMS.Services;

namespace VersionedContentPOC.Controllers;

[ApiController]
[Route("api/validation")]
[Produces("application/json")]
public class ContentValidationController : ControllerBase
{
    IContentRepository _contentRepository;

    public ContentValidationController(IContentRepository contentRepository)
    {
        _contentRepository = contentRepository;
    }

    [HttpPost]
    [Route("property")]
    [ProducesResponseType(typeof(List<ValidationResult>), StatusCodes.Status200OK)]
    public ActionResult<List<ValidationResult>> ValidateProperty([FromQuery] string contentTypeName, [FromQuery] string propertyName, [FromBody] ContentPropertyValueDto contentPropertyValueDto)
    {
        var contentType = ContentTypeRegistry
            .GetRegisteredContentType(contentTypeName);

        var property = contentType
            .GetContentProperties()
            .Single(x => x.Name == propertyName);

        var attributes = property
            .GetCustomAttributes<ValidationAttribute>(inherit: true)
            .ToList();

        var results = new List<ValidationResult>();

        foreach (var attr in attributes)
        {
            if (!attr.IsValid(ContentUpdater.ResolveValue(property.PropertyType.FullName, contentPropertyValueDto.Value)))
            {
                var errorMessage = attr.FormatErrorMessage(property.Name);
                results.Add(new ValidationResult(errorMessage, new[] { propertyName }));
            }
        }

        return Ok(results);
    }
}
