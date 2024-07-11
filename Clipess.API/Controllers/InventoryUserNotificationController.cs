using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;
using Clipess.DBClient.Repositories;
using log4net;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Clipess.API.Controllers
{
    [Route("api/InventoryNotification")]
    [ApiController]
    public class InventoryUserNotificationController : ControllerBase
    {
        private readonly IInventoryUserNotificationRepository _InventoryUserNotificationRepository;
        private static ILog _logger;

        public InventoryUserNotificationController(IInventoryUserNotificationRepository iNotificationRepository)
        {
            _InventoryUserNotificationRepository = iNotificationRepository;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }


        [HttpGet("InventoryNotifications")]
        public async Task<ActionResult> GetNotifications()
        {
            try
            {
                var notifications = _InventoryUserNotificationRepository.GetInventoryNotifications().ToList();
                if (notifications != null && notifications.Any())
                {
                    return Ok(notifications);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in: {nameof(GetNotifications)}, exception: {ex.Message}.");
                return BadRequest(new { message = ex.Message });
            }
        }





        [Route("add")]
        [HttpPost]
        public async Task <IActionResult> AddInventoryNotification([FromBody] InventoryUserNotification notification)
        {
            try
            {
                if (notification == null)
                {
                    return BadRequest("no notifications");
                }

                // Create a new Inventory_Type
                var NotificationNew = new InventoryUserNotification
                {
                    Notification = notification.Notification,
                    EmployeeId = notification.EmployeeId,
                    CreatedDate = DateTime.Now,
                };


                // Add the newly created request
                _InventoryUserNotificationRepository.AddInventoryNotification(notification);

                // Save changes to the database
                _InventoryUserNotificationRepository.SaveChanges();

                // Return a response indicating successful creation
                return CreatedAtAction(nameof(AddInventoryNotification), new { userNotificationId = notification.UserNotificationId}, notification);
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.Error($"An error occurred in: {nameof(AddInventoryNotification)}, exception: {ex.Message}.");

                // Return a response indicating failure
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }



       
        [HttpPut("read/{NotificationId}")]
        public async Task<IActionResult> ReadInventoryNotification(int NotificationId, [FromBody] InventoryUserNotification notificationRead)
        {
            try
            {
                var notification = _InventoryUserNotificationRepository.GetInventoryNotificationById(NotificationId);

                if (notification == null)
                {
                    return NotFound($"Request with ID {NotificationId} not found.");
                }

                notification.IsRead = notificationRead.IsRead;
                


                _InventoryUserNotificationRepository.ReadNotification(notificationRead);
                _InventoryUserNotificationRepository.SaveChanges();

                return Ok(notification);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in {nameof(ReadInventoryNotification)}, exception: {ex.Message}.");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }

        }

       

       

       

      

       


        [HttpGet("getInventoryNotification/{employeeId?}")]
        public ActionResult GetInventoryNotifications(int? employeeId)
        {
            try
            {
                var notifications = _InventoryUserNotificationRepository.GetInventoryNotificationByEmployeeId(employeeId).Where(x => !x.IsRead).ToList();
                if (notifications != null && notifications.Any())
                {
                    return Ok(notifications);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in: {nameof(GetInventoryNotifications)}, exception: {ex.Message}.");
                return BadRequest(new { message = ex.Message });
            }
        }

       

       
        


    }
}





