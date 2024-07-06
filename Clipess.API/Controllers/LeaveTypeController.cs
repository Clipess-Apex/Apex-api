using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;
using Clipess.DBClient.Repositories;
using log4net;
using log4net.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Clipess.API.Controllers
{
    [ApiController]
    [Route("api/leaveType")]
    
    public class LeaveTypeController : ControllerBase
    {

        private readonly ILeaveTypeRepository _leaveTypeRepository;
        private static ILog _logger;

        public LeaveTypeController(ILeaveRepository leaveRepository, ILeaveTypeRepository leaveTypeRepository)
        {
            _leaveTypeRepository = leaveTypeRepository;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }

        // Get leave type data in unhided leave types
        [Route("leaveType")]
        [HttpGet]
        public ActionResult GetLeaveTypes()
        {
            try
            {
                var leaveTypes = _leaveTypeRepository.GetLeaveTypes();
                var visibleLeaveTypes = leaveTypes.Where(lt => lt.HideState == 0).ToList();
                if (leaveTypes != null)
                {
                    return Ok(visibleLeaveTypes);
                }
                return NoContent();
            }
            catch (Exception ex)

            {
                _logger.Error($"An error occurred in: {nameof(GetLeaveTypes)}, exception: {ex.Message}.");
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("HiddenLeaveTypes")]
        public async Task<IActionResult>GetHiddenLeaveTypes()
        {
            try
            {
                var leaveTypes = _leaveTypeRepository.GetLeaveTypes();
                var hiddenLeaveTypes = leaveTypes.Where(lt => lt.HideState == 1).ToList();
                if(hiddenLeaveTypes != null)
                {
                    return Ok(hiddenLeaveTypes);
                }
                return NoContent();
            }
            catch(Exception ex) 
            {
                    _logger.Error($"An error occurred in:{nameof(GetHiddenLeaveTypes)},exception:{ex.Message}.");
                    return BadRequest();
            }
        }

        [HttpGet]
        [Route("GetLeaveTypeById/{id}")]
        public async Task<IActionResult> GetLeaveTypeById(int id)
        {
            try
            {
                var leaveTypeTask = _leaveTypeRepository.GetLeaveTypeById(id);
                var leaveType = await leaveTypeTask;
                if (leaveType != null)
                {
                    return Ok(leaveType);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.Error($"An Error occurred in: {nameof(GetLeaveTypeById)},exception:{ex.Message}");
                return NotFound();
            }
         }

        [HttpGet]
        [Route("CheckLeaveTypeName/{name}")]
        public async Task<IActionResult> CheckLeaveTypeName(string name)
        {
            try
            {
                var leaveTypes = await _leaveTypeRepository.GetLeaveTypeByName(name);
                if (leaveTypes.Any())
                {
                    var hideStates = leaveTypes.Select(lt => lt.HideState).ToList();
                    var result = new
                    {
                        Exists = true,
                        HideState = hideStates
                    };
                    return Ok(result);
                }
                else
                {
                    var result = new
                    {
                        Exists = false,
                        HideState = new List<int>()
                    };
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"An Error occurred in: {nameof(CheckLeaveTypeName)},exception:{ex.Message}");
                return NotFound();
            }
        }


        /*[HttpGet]
        [Route("CheckLeaveTypeName/{name}/{id?}")]
        public async Task<IActionResult> CheckLeaveTypeName(string name,int? id = null)
        {
            try
            {
                var leaveTypes = await _leaveTypeRepository.GetLeaveTypes().ToListAsync();
                var exists = leaveTypes.Any(lt => lt.LeaveTypeName.Equals(name, StringComparison.OrdinalIgnoreCase) && (!id.HasValue || lt.LeaveTypeId != id.Value));
                return Ok(exists);
            }
            catch (Exception ex)
            {
                _logger.Error($"An Error occurred in: {nameof(CheckLeaveTypeName)},exception:{ex.Message}");
                return NotFound();
            }
        }*/

        [HttpPost]
       [Route("AddLeaveTypes")]

        public async Task<IActionResult> AddLeaveType([FromBody] LeaveType receivingLeaveTypes)
        {
            try
            {
                var leaveType = new LeaveType
                {
                    LeaveTypeName = receivingLeaveTypes.LeaveTypeName,
                    MaxLeaveCount = receivingLeaveTypes.MaxLeaveCount,
                };
                await _leaveTypeRepository.AddLeaveTypes(leaveType);
                return Ok(leaveType);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in: {nameof(AddLeaveType)}, exception: {ex.Message}.");
                return BadRequest();
            }
        }

        [HttpPut]
        [Route("EditLeaveTypes/{id}")]
        public async Task<IActionResult>EditLeaveTypes(int id, [FromBody] EditLeaveTypeRequest request)
        {
            try
            {
                var leaveType = await _leaveTypeRepository.GetLeaveTypeById(id);
                if (leaveType == null)
                {
                    return NotFound();
                }
                if (request.MaxLeaveCount.HasValue)
                {
                    leaveType.MaxLeaveCount = request.MaxLeaveCount.Value;
                }
                if(!string.IsNullOrEmpty(request.LeaveTypeName))
                {
                    leaveType.LeaveTypeName = request.LeaveTypeName;
                }
               await _leaveTypeRepository.UpdateLeaveTypes(leaveType);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in: {nameof(EditLeaveTypes)}, exception: {ex.Message}.");
                return BadRequest();
            }
        }

        [HttpPut]
        [Route("UpdateHideState/{id}")]
        public async Task<IActionResult>UpdateHideState(int id, [FromBody] UpdateHideStateRequest request)
        {
            try
            {
                var leaveType = await _leaveTypeRepository.GetLeaveTypeById(id);
                if (leaveType == null)
                {
                    return NotFound();
                }
                leaveType.HideState = request.HideState;
                await _leaveTypeRepository.UpdateLeaveTypes(leaveType);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.Error($"An error cccurred in: {nameof(UpdateHideState)},exception: {ex.Message}.");
                return BadRequest();
            }
        }    
    }
}
