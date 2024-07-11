using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Clipess.API.Controllers
{
    [ApiController]
    [Route("api/LeaveNotification")]
    public class LeaveNotificationController : ControllerBase
    {
        private readonly ILeaveNotificationRepository _leaveNotificationRepository;
        private readonly IHubContext<NotificationHub> _notificationHubContext;
        private readonly IEmployeeRepository _employeeRepository;

        public LeaveNotificationController(
            ILeaveNotificationRepository leaveNotificationRepository,
            //IHubContext<NotificationHub> notificationHubContext,
            IEmployeeRepository employeeRepository)
        {
            _leaveNotificationRepository = leaveNotificationRepository;
            //_notificationHubContext = notificationHubContext;
            _employeeRepository = employeeRepository;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateNotification([FromBody] LeaveNotification createNotificationDto)
        {
            if (createNotificationDto == null)
            {
                return BadRequest("Invalid notification data.");
            }

            var managerIds = await _employeeRepository.GetAllManagerIds();

            if (managerIds == null || managerIds.Count == 0)
            {
                return StatusCode(500, "No managers found.");
            }

            var success = await _leaveNotificationRepository.CreateNotificationsForManagers(
                createNotificationDto.EmployeeId,
                createNotificationDto.LeaveId,
                createNotificationDto.Message,
                managerIds
            );

            if (success)
            {
                foreach (var managerId in managerIds)
                {
                    var notificationMessage = new
                    {
                        Message = createNotificationDto.Message,
                        EmployeeId = createNotificationDto.EmployeeId,
                        LeaveId = createNotificationDto.LeaveId,
                        ManagerId = managerId,
                        CreatedAt = DateTime.UtcNow.ToString("o") // ISO 8601 format
                    };

                    // Serialize the message as JSON
                    var serializedMessage = JsonSerializer.Serialize(notificationMessage);

                    // Send the serialized message to all clients via SignalR
                    //await _notificationHubContext.Clients.All.SendAsync("ReceiveNotification", serializedMessage);
                }
                return Ok("Notification created successfully.");
            }

            return StatusCode(500, "An error occurred while creating the notification.");
        }

        [HttpGet]
        [Route("managerUnread/{sendTo}")]
        public async Task<IActionResult> GetNotificationsForManager(int sendTo)
        {
            try
            {
                if (sendTo <= 0)
                {
                    return BadRequest("Invalid manager ID.");
                }

                var unreadNotifications = await _leaveNotificationRepository.GetNotificationsForManager(sendTo);

                if (!unreadNotifications.Any())
                {
                    return NotFound("No unread notifications found for the manager.");
                }

                return Ok(unreadNotifications);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut]
        [Route("mark-as-read/{notificationId}")]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            var success = await _leaveNotificationRepository.MarkNotificationAsRead(notificationId);

            if (success)
            {
                return Ok("Notification marked as read.");
            }

            return StatusCode(500, "An error occurred while updating the notification.");
        }

        [HttpPost]
        [Route("managerCreate")]
        public async Task<IActionResult> CreateManagerNotification([FromBody] LeaveNotification createNotificationDto)
        {
            if (createNotificationDto == null)
            {
                return BadRequest("Invalid notification data.");
            }

            var success = await _leaveNotificationRepository.CreateNotificationsForEmployee(
                createNotificationDto.EmployeeId,
                createNotificationDto.LeaveId,
                createNotificationDto.Message,
                createNotificationDto.ManagerId // Ensure the ManagerId is passed correctly
            );

            if (success)
            {
                var notificationMessage = new
                {
                    Message = createNotificationDto.Message,
                    EmployeeId = createNotificationDto.EmployeeId,
                    LeaveId = createNotificationDto.LeaveId,
                    ManagerId = createNotificationDto.ManagerId,
                    CreatedAt = DateTime.UtcNow.ToString("o") // ISO 8601 format
                };

                // Serialize the message as JSON
                var serializedMessage = JsonSerializer.Serialize(notificationMessage);

                // Send the serialized message to all clients via SignalR
                await _notificationHubContext.Clients.All.SendAsync("ReceiveNotification", serializedMessage);

                return Ok("Notification created successfully.");
            }

            return StatusCode(500, "An error occurred while creating the notification.");
        }


    }
}
