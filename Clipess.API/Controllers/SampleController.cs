using Clipess.DBClient.Contracts;
using Clipess.DBClient.Repositories;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Reflection;

namespace Clipess.API.Controllers
{
    [ApiController]
    [Route("api/sample")]
    public class SampleController : ControllerBase
    {
        private readonly ISampleRepository _sampleRepository;
        private static ILog _logger;

        public SampleController(ISampleRepository sampleRepository)
        {
            _sampleRepository = sampleRepository;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }

        /*[Route("user")]
        [HttpGet]
        public async Task<ActionResult> GetUser([FromQuery] int userId)
        {
            try
            {
                var user = _sampleRepository.GetUser(userId);
                if (user != null)
                {
                    return Ok(user);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in: {nameof(GetUser)} for userId: {userId}, exception: {ex.Message}.");
                return BadRequest();
            }
        }*/
    }
}