using Clipess.DBClient.EntityModels;
using log4net;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using Clipess.DBClient.Contracts;

namespace Clipess.API.Controllers;

    [Route("api/department")]
    [ApiController]
    public class DepartmentControllercs : ControllerBase
    {        
        private static ILog _logger;
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentControllercs(IDepartmentRepository departmentRepository)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _departmentRepository = departmentRepository;        
        }

        [HttpGet]
        [Route("getDepartment")]
        public async Task<ActionResult> GetDepartment([FromQuery] int DepartmentID)
         {
           try
             {
                var department = _departmentRepository.GetDepartments();
                return Ok(department);            
                }
                catch (Exception ex)
                {
                    _logger.Error($"An error occurred in: {nameof(GetDepartment)} , exception: {ex.Message}.");
                    return BadRequest();
                }
         }


            [HttpPost]
            [Route("addDepartment")]            
            public async Task<IActionResult> AddDepartment([FromBody] Department receivingDepartment)
            {
            try
            {
                    var department = new Department
                    {
                        DepartmentID = receivingDepartment.DepartmentID,
                        DepartmentName = receivingDepartment.DepartmentName,
                    };

                    await _departmentRepository.AddDepartment(department);
                    return Ok(department);
            }
             catch (Exception ex)
             {
            _logger.Error($"An error occurred in: {nameof(AddDepartment)} for Department: {receivingDepartment}, exception: {ex.Message}.");
            return BadRequest();
            }
            }

        [HttpPut]
        [Route("UpdateDepartment/{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, [FromBody] Department updatedDepartment)
        {
            if (id != updatedDepartment.DepartmentID)
            {
                return BadRequest();
            }

            try
            {
                var existingDepartment = await _departmentRepository.GetDepartmentById(id);
                if (existingDepartment == null)
                {
                    return NotFound();
                }

                existingDepartment.DepartmentName = updatedDepartment.DepartmentName;

                var result = await _departmentRepository.UpdateDepartment(existingDepartment);
                if (!result)
                {
                    return StatusCode(500, "A problem happened while handling your request.");
                }

                return Ok(existingDepartment);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in: {nameof(UpdateDepartment)}, exception: {ex.Message}.");
                return BadRequest();
            }
        }

        [HttpPut]
        [Route("UpdateDeleteStatus/{id}")]
        public async Task<IActionResult> UpdateDeleteStatus(int id, [FromBody] Department request)
        {
            try
            {
                var departmentToDelete = await _departmentRepository.GetDepartmentById(id);

                if (departmentToDelete == null)
                {
                    return NotFound();
                }

                departmentToDelete.Deleted = request.Deleted;

                await _departmentRepository.UpdateDepartment(departmentToDelete);
                return Ok(departmentToDelete);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error cccurred in: {nameof(UpdateDeleteStatus)},exception: {ex.Message}.");
                return BadRequest();

            }
        }
}

