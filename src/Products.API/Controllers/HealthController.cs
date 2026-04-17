using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Products.Application.Common;
using Products.Application.Common.DTOs;

namespace Products.API.Controllers
{

    [Route("health")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<HealthResponse>), StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            var data = new HealthResponse("healthy", DateTime.UtcNow, "1.0.0");
            return Ok(ApiResponse<HealthResponse>.Success(data));
        }
    }
}