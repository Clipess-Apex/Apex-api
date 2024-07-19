using Clipess.DBClient.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using log4net;
using System.Reflection;

namespace Clipess.API.Controllers
{
    public enum NextMonthMonthlyTimeEntries
    {
        RecordsAvailable = 1,
        RecordsUnavailable = 2
    }

    [Route("api/attendanceNotification")]
    [ApiController]
    public class AttendanceNotificationControler : ControllerBase
    {
        private readonly IAttendanceNotificationRepository _attendanceNotificationRepository;
        public static ILog _logger;

        public AttendanceNotificationControler(IAttendanceNotificationRepository attendanceNotificationRepository)
        {
            _attendanceNotificationRepository = attendanceNotificationRepository;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }


        [HttpGet]
        [Route("CreateTimeEntryNotification")]
        public async Task<IActionResult> CreateTimeEntryNotification()
        {
            try
            {
                var isMonthlyTimeEntriesAvailable = _attendanceNotificationRepository.CheckNextMonthMonthlyTimeEntries();

                if(isMonthlyTimeEntriesAvailable == (int)NextMonthMonthlyTimeEntries.RecordsUnavailable)
                {
                    var notification = await _attendanceNotificationRepository.CreateTimeEntryNotifications();
                    return Ok(notification);
                }
                return Ok("Records Available");
            }
            catch (Exception ex)
            {
                _logger.Error($"An error has occured in: {nameof(CreateTimeEntryNotification)} , exception: {ex.Message}.");
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("GetTimeEntryNotification")]
        public async Task<IActionResult> GetTimeEntryNotification([FromQuery] int employeeId)
        {
            try
            {
                var timeEntryNotification = _attendanceNotificationRepository.GetTimeEntryNotifications(employeeId).Select(x => new
                {
                    x.Id,
                    x.ReadBy,
                    x.Message,
                    x.IsRead,
                    CreatedDate = x.CreatedDate.ToString("yyyy-MM-dd hh:mm tt")
                }).ToList();

                if (timeEntryNotification == null)
                {
                    return NoContent();
                }

                return Ok(timeEntryNotification);

            }

            catch (Exception ex)
            {
                _logger.Error($"An error has occured in: {nameof(GetTimeEntryNotification)} , exception: {ex.Message}.");
                return BadRequest();
            }
        }


        [HttpDelete]
        [Route("HideTimeEntryNotification")]
        public async Task<IActionResult> HideTimeEntryNotification([FromQuery] int notificationId)
        {
            var notification = await _attendanceNotificationRepository.HideTimeEntryNotification(notificationId);

            if(notification == null)
            {
                return NoContent();
            }

            return Ok(notification);
        }
    }
}
