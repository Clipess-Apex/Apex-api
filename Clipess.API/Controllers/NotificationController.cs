using Clipess.DBClient.Infrastructure;
using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;
using Clipess.DBClient.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;



namespace Clipess.API.Controllers
{
    [ApiController]
    [Route("api/notification")]
    public class NotificationController : Controller
    {
        private INotificationRepository _notificationRepository;
        private readonly IHubContext<SignalServer, INotificationClient> _hubContext;

        public NotificationController(INotificationRepository notificationRepository, IHubContext<SignalServer, INotificationClient> hubContext)
        {
            _notificationRepository = notificationRepository;
            _hubContext = hubContext;

        }

        [HttpGet("GetNotifications")]
        public async Task<ActionResult> GetNotifications([FromQuery] int userId)
        {
            try
            {
                var notifications = _notificationRepository.GetNotifications(userId);
                if (notifications != null)
                {
                    return Ok(notifications);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                // Log the exception
                Debug.WriteLine(ex);
                return BadRequest();
            }
        }

        [Route("CreateNotification")]
        [HttpPost]
        public async Task<IActionResult> CreateNotification([FromQuery] int notificationId, [FromQuery] int userId)
        {

            try
            {
                var notification = new UserNotification();
                notification.NotificationId = notificationId;
                notification.UserId = userId;
                var notificationText = _notificationRepository.GetNotify(notificationId);
                if (ConnectionManager._userConnections.TryGetValue(userId.ToString(), out var connectionId))
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

        [Route("ReadNotification")]
        [HttpPut]
        public async Task<ActionResult> ReadNotification([FromQuery] int userId)
        {
            try
            {
                _notificationRepository.ReadNotification(userId);

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