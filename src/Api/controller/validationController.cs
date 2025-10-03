using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ValidApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValidationController : ControllerBase
    {
        [HttpGet("check")]
        [Authorize]
        public IActionResult Check()
        {
            var userName = User.Identity?.Name ?? "Unknown";

            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();

            return Ok(new
            {
                Message = "Token válido!",
                User = userName,
                Claims = claims
            });
        }
    }
}
