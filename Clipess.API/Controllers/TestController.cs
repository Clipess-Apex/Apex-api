using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Clipess.API.Services;

namespace Clipess.API.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestEmailController : ControllerBase
    {
        private readonly EmailService _emailService;

        public TestEmailController(EmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send-email")]
        public async Task<IActionResult> SendTestEmail()
        {
            try
            {
                await _emailService.SendEmailAsync("recipient@example.com", "Test Email", "This is a test email.");
                return Ok("Email sent successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error sending email: {ex.Message}");
            }
        }
    }
}
