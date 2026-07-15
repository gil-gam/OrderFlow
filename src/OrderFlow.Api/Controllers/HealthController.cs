using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace OrderFlow.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Produces("application/json")]
[SwaggerTag("Health — API health check")]
public sealed class HealthController : ControllerBase
{
    /// <summary>
    /// Returns a simple health status for the API.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Health check",
        Description = "Returns the current health status of the API.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Get()
        => Ok(new
        {
            status = "Healthy",
            timestamp = DateTime.UtcNow,
            version = "1.0.0"
        });
}