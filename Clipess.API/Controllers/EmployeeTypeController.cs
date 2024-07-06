using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;
using log4net;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Clipess.API.Controllers
{
    [Route("api/employeeType")]
    [ApiController]
    public class EmployeeTypeController : ControllerBase
    {
        private readonly IEmployeeTypeRepository _employeeTypeRepository;
        private static ILog _logger;

        public EmployeeTypeController(IEmployeeTypeRepository employeeTypeRepository)
        {
            _employeeTypeRepository = employeeTypeRepository;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }

        [Route("getEmployeeType")]
        [HttpGet]
        public async Task<ActionResult> GetEmployeeType([FromQuery] int EmployeeTypeID)
        {
            try
            {
                var employeeType = _employeeTypeRepository.GetEmployeeTypes();
                if (employeeType != null)
                {
                    return Ok(employeeType);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in: {nameof(GetEmployeeType)} for EmployeeTypeID: {EmployeeTypeID}, exception: {ex.Message}.");
                return BadRequest();
            }
        }
    

        [HttpPost]
        [Route("addEmployeeType")]
        public async Task<IActionResult> AddEmployeeType([FromBody] EmployeeType recevingEmployeeType)
        {
            try
            {
                var employeeType = new EmployeeType
                {
                    EmployeeTypeID = recevingEmployeeType.EmployeeTypeID,
                    EmployeeTypeName = recevingEmployeeType.EmployeeTypeName,
                };

                await _employeeTypeRepository.AddEmployeeTypes(employeeType);
                return Ok(employeeType);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in: {nameof(GetEmployeeType)} , exception: {ex.Message}.");
                return BadRequest();
            }
        }

        [HttpPut]
        [Route("UpdateEmployeeType/{id}")]
        public async Task<IActionResult> UpdateEmployeeType(int id, [FromBody] EmployeeType updatedEmployeeType)
        {
            if (id != updatedEmployeeType.EmployeeTypeID)
            {
                return BadRequest();
            }

            try
            {
                var existingEmployeeType = await _employeeTypeRepository.GetEmployeeTypeById(id);
                if (existingEmployeeType == null)
                {
                    return NotFound();
                }

                existingEmployeeType.EmployeeTypeName = updatedEmployeeType.EmployeeTypeName;

                var result = await _employeeTypeRepository.UpdateEmployeeType(existingEmployeeType);
                if (!result)
                {
                    return StatusCode(500, "A problem happened while handling your request.");
                }

                return Ok(existingEmployeeType);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in: {nameof(UpdateEmployeeType)}, exception: {ex.Message}.");
                return BadRequest();
            }
        }

        [HttpPut]
        [Route("UpdateDeleteStatus/{id}")]
        public async Task<IActionResult> UpdateDeleteStatus(int id, [FromBody] EmployeeType request)
        {
            try
            {
                var employeeTypeToDelete = await _employeeTypeRepository.GetEmployeeTypeById(id);

                if (employeeTypeToDelete == null)
                {
                    return NotFound();
                }

                employeeTypeToDelete.Deleted = request.Deleted;

                await _employeeTypeRepository.UpdateEmployeeType(employeeTypeToDelete);
                return Ok(employeeTypeToDelete);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error cccurred in: {nameof(UpdateDeleteStatus)},exception: {ex.Message}.");
                return BadRequest();

            }
        }
    }
}
