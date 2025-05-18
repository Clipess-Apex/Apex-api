using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clipess.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/Admin")]
    [ApiController]
    public class AdminController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("You have accessed the Admin Controller");
        }
    }
}
