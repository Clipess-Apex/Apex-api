using Clipess.DBClient.Contracts;
using Clipess.DBClient.Repositories;
using log4net;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Clipess.API.Controllers
{
    [Route("api/employee")]
    [ApiController]
    public class EmployeeInventoryController : ControllerBase
    {
        private readonly IEmployeeInventoryRepository _EmployeeRepository;
        private static ILog _logger;

        public EmployeeInventoryController(IEmployeeInventoryRepository employeeRepository)
        {
            _EmployeeRepository = employeeRepository;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }

        [Route("Employee")]
        [HttpGet]
        public ActionResult GetEmployee()
        {
            try
            {
                var inventoryTypes = _EmployeeRepository.GetEmployees().ToList();
                if (inventoryTypes != null && inventoryTypes.Any())
                {
                    return Ok(inventoryTypes);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in: {nameof(GetEmployee)}, exception: {ex.Message}.");
                return BadRequest();
            }
        }

    }
}

