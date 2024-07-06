using Clipess.DBClient.Infrastructure;
using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace Clipess.API.Controllers
{
    [ApiController]
    [Route("api/notification")]
    public class NotificationController : Controller
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IHubContext<SignalServer,INotificationClient> _hubContext;

        public NotificationController(INotificationRepository notificationRepository, IHubContext<SignalServer,INotificationClient> hubContext)
        {
            _notificationRepository = notificationRepository;
            _hubContext = hubContext;

        }

        [HttpGet("GetNotifications")]
        public async Task<ActionResult> GetNotifications([FromQuery] int EmployeeId)
        {
            try
            {
                var notifications = _notificationRepository.GetNotifications(EmployeeId);
                if(notifications != null)
                {
                    return Ok(notifications);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [Route("CreateNotification")]
        [HttpPost]
        public async Task<IActionResult> CreateNotification([FromQuery] int notificationId, [FromQuery] int EmployeeId)
        {

            try
            {
                var notification = new UserNotification
                {
                    NotificationId = notificationId,
                    EmployeeId = EmployeeId
                };
                var notificationText = _notificationRepository.GetNotify(notificationId);
                if (ConnectionManager._userConnections.TryGetValue(EmployeeId.ToString(), out var connectionId))
                {
                    Debug.WriteLine($"User ConnectionId : {connectionId}");
                    await _hubContext.Clients.Client(connectionId).RecieveNotification(notificationText);
                    Debug.WriteLine("Notifications sent via SignalR");
                }

                _notificationRepository.CreateNotification(notification);
                return Ok(new { message = "Notification created successfully." });

            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [Route("CreateTaskNotification")]
        [HttpPost]
        public async Task<IActionResult> CreateTaskNotification([FromQuery] int notificationId, [FromQuery] string EmployeeIds)
        {
            

            try
            {
                string[] dataArray = EmployeeIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                // Convert each substring to integer if needed
                int[] taskEmployees = Array.ConvertAll(dataArray, int.Parse);

                foreach (var item in taskEmployees)
                {
                    var notification = new UserNotification
                    {
                        NotificationId = notificationId,
                        EmployeeId = item
                    };
                    var notificationText = _notificationRepository.GetNotify(notificationId);
                    if (ConnectionManager._userConnections.TryGetValue(item.ToString(), out var connectionId))
                    {
                        Debug.WriteLine($"User ConnectionId : {connectionId}");
                        await _hubContext.Clients.Client(connectionId).RecieveNotification(notificationText);
                        Debug.WriteLine("Notifications sent via SignalR");
                    }

                    _notificationRepository.CreateNotification(notification);
                }

                
                    
                return Ok(new { message = "Notification created successfully." });


            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [Route("ReadNotification")]
        [HttpPut]
        public async Task<ActionResult> ReadNotification([FromQuery] int EmployeeId)
        {
            try
            {
                _notificationRepository.ReadNotification(EmployeeId);

                return Ok(new { message = "Notification updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                   "Error updating data");
            }

        }
    }
}