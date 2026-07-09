using Microsoft.AspNetCore.Mvc;

namespace OrderFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class HealthController : ControllerBase
{
    /// <summary>
    /// Returns a simple health status for the API.
    /// </summary>
    [HttpGet]
    public IActionResult Get()
        => Ok(new
        {
            status = "Healthy",
            timestamp = DateTime.UtcNow,
            version = "1.0.0"
        });
}