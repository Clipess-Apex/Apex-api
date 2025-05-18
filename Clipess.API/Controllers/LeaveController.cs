using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;
using Clipess.DBClient.Repositories;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Reflection;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace Clipess.API.Controllers

{
    [ApiController]
    [Route("api/leave")]
    public class LeaveController : ControllerBase
    {
        private readonly ILeaveRepository _leaveRepository;
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(LeaveController));
        public LeaveController(ILeaveRepository leaveRepository)
        {
            _leaveRepository = leaveRepository;
        }

        /*This method use to get all leaves according to the states(pending,approved or deleted)*/
        [Route("leave")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Leave>>> GetLeaves(int statusId)
        { 
            try
            {
                var leavesTask = _leaveRepository.GetLeavesByStatusId(statusId);
                var leaves = await leavesTask;
                if (leaves != null && leaves.Count() > 0)
                {
                    return Ok(leaves);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in: {nameof(GetLeaves)}, exception: {ex.Message}.");
                return BadRequest();
            }
        }

        /*This method use to get past leave details according employee Ids.
         * This Method use to get pending leaves according the employee id also*/
        [Route("pastLeaves")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Leave>>> ViewLeaves(int statusId,int employeeId)
        {
            try
            {
                var leavesTask = _leaveRepository.GetLeavesByEmployeeId(statusId,employeeId);
                var leaves = await leavesTask;
                if (leaves != null && leaves.Count() > 0)
                {
                    return Ok(leaves);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in: {nameof(ViewLeaves)}, exception: {ex.Message}.");
                return BadRequest();
            }
        }

        /* This method use to get leave details to edit leave request form*/



        [Route("getleave/{leaveId}")] 
        [HttpGet]
        public async Task<ActionResult<Leave>> GetLeaveById(int id) 
        {
            try
            {
                var leave = await _leaveRepository.GetLeaveById(id);
                if (leave == null)
                {
                    return NotFound(); 
                }
               return Ok(leave);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in: {nameof(GetLeaveById)}, exception: {ex.Message}.");
                return BadRequest();
            }
        }

        // Get count of leaves according the spesific leave type

        //[HttpGet("leaveCounts")]
        //public async Task<IActionResult> GetLeaveCounts(int employeeId,int leaveTypeId)
        //{
        //    try
        //    {
        //        var leaveCount = await _leaveRepository.GetLeaveCountByEmployeeAndType(employeeId, leaveTypeId);
        //        return Ok(leaveCount);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error($"An error occurred in: {nameof(GetLeaveCounts)}, exception: {ex.Message}.");
        //        return BadRequest();
        //    }
        //}

        [HttpGet("leaveCounts")]
        public async Task<IActionResult> GetLeaveCounts(int employeeId, int leaveTypeId, DateTime leaveDate)
        {
            try
            {
                // Extract the start and end dates for the month of the given leaveDate
                DateTime startOfMonth = new DateTime(leaveDate.Year, leaveDate.Month, 1);
                DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

                var leaveCount = await _leaveRepository.GetLeaveCountByEmployeeAndType(employeeId, leaveTypeId, startOfMonth, endOfMonth);
                return Ok(leaveCount);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in: {nameof(GetLeaveCounts)}, exception: {ex.Message}.");
                return BadRequest();
            }
        }


        [HttpPut]
        [Route("updateLeave/{id}")]
        public async Task<IActionResult> UpdateLeave(int id, [FromBody] int statusId)
        {
            try
            {
                var existingLeave = await _leaveRepository.GetLeaveById(id);
                if (existingLeave == null)
                {
                    return NotFound();
                }

                existingLeave.StatusId = statusId;
                

                var updateResult = await _leaveRepository.UpdateLeave(existingLeave);

                if (updateResult)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in: {nameof(UpdateLeave)}, exception: {ex.Message}.");
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("AddLeaveDetailes")]
        public async Task<IActionResult> AddLeave([FromBody] Leave receivingLeaves)
        {
            try
            {
                var leave = new Leave
                {
                    EmployeeId = receivingLeaves.EmployeeId,
                    CreatedDate = DateTime.Now ,
                    LeaveTypeId = receivingLeaves.LeaveTypeId,
                    LeaveDate = receivingLeaves.LeaveDate,
                    Reason = receivingLeaves.Reason,
                    StatusId = 1,
                };
                await _leaveRepository.AddLeaves(leave);

                return Ok(leave);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in: {nameof(AddLeave)}, exception: {ex.Message}.");
                return BadRequest();
            }

        }

        [HttpDelete("deleteLeave/{id}")]
        public async Task<IActionResult> DeleteLeave(int id)
        {
            try
            {
                var existingLeave = await _leaveRepository.GetLeaveById(id);
                if (existingLeave == null)
                {
                    return NotFound(new { Message = "Leave request not found" });
                }

                bool deleteResult = await _leaveRepository.DeleteLeave(id);
                if (!deleteResult)
                {
                    return StatusCode(500, new { Message = "Failed to delete leave request" });
                }

                return Ok(new { Message = "Leave request deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.Error($"Error occurred in {nameof(DeleteLeave)}: {ex.Message}");
                return StatusCode(500, new { Message = "An error occurred while deleting the leave request" });
            }
        }
    }
}
