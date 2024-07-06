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

    [Route("api/user")]

    public class UserController : ControllerBase

    {

        private readonly IUserRepository _userRepository;

        private static ILog _logger;



        public UserController(IUserRepository userRepository)

        {

            _userRepository = userRepository;

            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        }



        [Route("user")]

        [HttpGet]

        public async Task<ActionResult> GetUser([FromQuery] int userId)

        {

            try

            {

                var user = _userRepository.GetUsers().FirstOrDefault(x => x.Id == userId && !x.Deleted);

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

        }

    }

}