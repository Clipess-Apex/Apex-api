using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;
using log4net;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Clipess.API.Controllers
{
    [Route("api/role")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleRepository _roleRepository;
        private static ILog _logger;
        public RoleController(IRoleRepository roleRepository) 
        {
            _roleRepository = roleRepository;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }

        [Route("getRole")]
        [HttpGet]
        public async Task<ActionResult> GetRole([FromQuery] int RoleID)
        {
            {
                try
                {
                    var role = _roleRepository.GetRoles();
                    if (role != null)
                    {
                        return Ok(role);
                    }
                    return NoContent();
                }
                catch (Exception ex)
                {
                    _logger.Error($"An error occurred in: {nameof(GetRole)} for RoleID: {RoleID}, exception: {ex.Message}.");
                    return BadRequest();
                }
            }
        }

        [HttpPost]
        [Route("addRole")]
        public async Task<IActionResult> AddRole([FromBody] Role recevingRole)
        {
            try
            {
                var role = new Role
                {
                    RoleID = recevingRole.RoleID,
                    RoleName = recevingRole.RoleName,
                };

                await _roleRepository.AddRoles(role);
                return Ok(role);
            }
            catch(Exception ex)
            {
                _logger.Error($"An error occurred in: {nameof(GetRole)} , exception: {ex.Message}.");
                return BadRequest();
            }
        }

        [HttpPut]
        [Route("UpdateRole/{id}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] Role updatedRole)
        {
            if (id != updatedRole.RoleID)
            {
                return BadRequest();
            }

            try
            {
                var existingRole = await _roleRepository.GetRoleById(id);
                if (existingRole == null)
                {
                    return NotFound();
                }

                existingRole.RoleName = updatedRole.RoleName;

                var result = await _roleRepository.UpdateRole(existingRole);
                if (!result)
                {
                    return StatusCode(500, "A problem happened while handling your request.");
                }

                return Ok(existingRole);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in: {nameof(UpdateRole)}, exception: {ex.Message}.");
                return BadRequest();
            }
        }

        [HttpPut]
        [Route("UpdateDeleteStatus/{id}")]
        public async Task<IActionResult> UpdateDeleteStatus(int id, [FromBody] Role request)
        {
            try
            {
                var roleToDelete = await _roleRepository.GetRoleById(id);

                if (roleToDelete == null)
                {
                    return NotFound();
                }

                roleToDelete.Deleted = request.Deleted;

                await _roleRepository.UpdateRole(roleToDelete);
                return Ok(roleToDelete);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error cccurred in: {nameof(UpdateDeleteStatus)},exception: {ex.Message}.");
                return BadRequest();

            }
        }
    }
}
