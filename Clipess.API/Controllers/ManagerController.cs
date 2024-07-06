using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clipess.API.Controllers
{
    [Authorize(Roles = "Manager")]
    [Route("api/Admin")]
    [ApiController]
    public class ManagerController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("You have accessed the Manager Controller");
        }
    }
}
