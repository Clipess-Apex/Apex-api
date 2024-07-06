using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;
using Clipess.DBClient.Repositories;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq;
using System.Reflection;
using static Clipess.API.Controllers.InventoryController;
using System.Threading;
using Microsoft.EntityFrameworkCore;

namespace Clipess.API.Controllers
{
    [Route("api/Request")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        private readonly IRequestRepository _requestRepository;
        private static ILog _logger;

        public RequestController(IRequestRepository iRequestRepository)
        {
            _requestRepository = iRequestRepository;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }

        [HttpGet("Request")]
        public async Task<ActionResult> GetRequests()
        {
            try
            {
                var requests = _requestRepository.GetRequests().Where(x => !x.Deleted).ToList();
                if (requests != null && requests.Any())
                {
                    return Ok(requests);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in: {nameof(GetRequests)}, exception: {ex.Message}.");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("Requestby/{requestId}")]
        public async Task<ActionResult> GetRequestById(int requestId)
        {
            try
            {
                var request = _requestRepository.GetRequestById(requestId);

                if (request == null)
                {
                    return NotFound($"Request with ID {requestId} not found.");
                }

                return Ok(request);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in: {nameof(GetRequestById)}, exception: {ex.Message}.");
                return BadRequest(new { message = ex.Message });
            }
        }


        [Route("add")]
        [HttpPost]
        public async Task<IActionResult> AddRequest([FromBody] Request request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("Inventory type name is missing.");
                }

                // Create a new Inventory_Type
                var requestNew = new Request
                {
                    EmployeeId = request.EmployeeId,
                    InventoryTypeId = request.InventoryTypeId,
                    InventoryId = request.InventoryId,
                    CreatedDate = DateTime.Now,
                    Inventory = request.Inventory,
                    Deleted = request.Deleted,
                    IsRead = request.IsRead,
                    Message = request.Message,
                    Rejected = request.Rejected,
                     
                   

                };

                if (request.Rejected)
                {
                    requestNew.Reason = request.Reason;


                }
                requestNew.Reason = null;

                if (requestNew.Deleted)
                {
                    requestNew.DeletedDate = DateTime.Now;

                }
                // Add the newly created request
                _requestRepository.AddRequest(request);

                // Save changes to the database
                _requestRepository.SaveChanges();

                // Return a response indicating successful creation
                return CreatedAtAction(nameof(GetRequests), new { requestId = request.RequestId }, request);
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.Error($"An error occurred in: {nameof(AddRequest)}, exception: {ex.Message}.");

                // Return a response indicating failure
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [Route("accept/{requestId}")]
        [HttpPut]
        public async Task<IActionResult> AcceptRequest(int requestId, [FromBody] Request requestUpdate)
        {
            try
            {
                var request = _requestRepository.GetRequestById(requestId);

                if (request == null)
                {
                    return NotFound($"Request with ID {requestId} not found.");
                }

                if (request.IsRead && request.Rejected == false)
                {
                    
                    request.InventoryId = requestUpdate.InventoryId;
                    
                    
                }

                _requestRepository.AcceptRequest(request);
                _requestRepository.SaveChanges();

                return Ok(request);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in {nameof(AcceptRequest)}, exception: {ex.Message}.");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }



        [Route("updateEmployeeRequest/{requestId}")]
        [HttpPut]

        public async Task<IActionResult> UpdateEmployeeRequest(int requestId, [FromBody] Request requestUpdate)
        {
            try
            {
                var request = _requestRepository.GetRequestById(requestId);
                if(request == null)
                {
                    return NotFound($"Request with Id {requestId} not found");
                }

                if (!request.IsRead)
                 {
                    request.Inventory = requestUpdate.Inventory;
                    request.InventoryTypeId = requestUpdate.InventoryTypeId;
                    request.Message = requestUpdate.Message;
                 }
                _requestRepository.UpdateEmployeeRequest(request);
                _requestRepository.SaveChanges();

                return Ok(request);

            }
            catch(Exception ex)
            {

                _logger.Error($"An error occurred in {nameof(AcceptRequest)}, exception: {ex.Message}.");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }



            [HttpPut("delete/{requestId}")]
            public async Task<IActionResult> DeleteRequest(int requestId, [FromBody] Request requestDelete)
            {
                try
                {
                    var request = _requestRepository.GetRequestById(requestId);

                    if (request == null)
                    {
                        return NotFound($"Request with ID {requestId} not found.");
                    }
                if (!request.IsRead)
                {
                    request.Deleted = requestDelete.Deleted;
                    
                }
                if (request.Deleted)
                {
                    request.DeletedDate = DateTime.UtcNow;
                }
                    _requestRepository.DeleteRequest(request);
                    _requestRepository.SaveChanges();

                    return Ok(request);
                }
                catch (Exception ex)
                {
                    _logger.Error($"An error occurred in {nameof(DeleteRequest)}, exception: {ex.Message}.");
                    return StatusCode(500, $"An error occurred: {ex.Message}");
                }

            }

        [HttpPut("read/{requestId}")]
        public async Task<IActionResult> ReadRequest(int requestId, [FromBody] Request requestRead)
        {
            try
            {
                var request = _requestRepository.GetRequestById(requestId);

                if (request == null)
                {
                    return NotFound($"Request with ID {requestId} not found.");
                }

                request.IsRead = requestRead.IsRead;
                

                _requestRepository.ReadRequest(request);
                _requestRepository.SaveChanges();

                return Ok(request);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in {nameof(ReadRequest)}, exception: {ex.Message}.");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }

        }

        [HttpPut("reject/{requestId}")]
        public async Task<IActionResult> RejectRequest(int requestId, [FromBody] Request rejectRead)
        {
            try
            {
                var request = _requestRepository.GetRequestById(requestId);

                if (request == null)
                {
                    return NotFound($"Request with ID {requestId} not found.");
                }

                if (request.InventoryId == 0)
                {
                    request.Rejected = rejectRead.Rejected;
                    request.Reason = rejectRead.Reason;
                    //request.IsRead = rejectRead.IsRead;
                }

                _requestRepository.RejectRequest(request);
                _requestRepository.SaveChanges();

                return Ok(request);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in {nameof(RejectRequest)}, exception: {ex.Message}.");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }

        }

        [HttpGet("pendingrequests/{employeeId?}")]
        public ActionResult GetPendingRequests(int? employeeId)
        {
            try
            {
                var requests = _requestRepository.GetRequestsByEmployeeId(employeeId).Where(x => !x.Deleted && x.IsRead && !x.Rejected && x.InventoryId == 0).ToList();
                if (requests != null && requests.Any())
                {
                    return Ok(requests);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in: {nameof(GetPendingRequests)}, exception: {ex.Message}.");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("rejectedrequests/{employeeId?}")]
        public ActionResult GetRejectedRequests(int? employeeId)
        {
            try
            {
                var requests = _requestRepository.GetRequestsByEmployeeId(employeeId).Where(x => !x.Deleted && x.Rejected && x.IsRead && x.InventoryId == 0).ToList();
                if (requests != null && requests.Any())
                {
                    return Ok(requests);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in: {nameof(GetRejectedRequests)}, exception: {ex.Message}.");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("acceptedrequests/{employeeId?}")]
        public ActionResult GetAcceptedRequests(int? employeeId)
        {
            try
            {
                var requests = _requestRepository.GetRequestsByEmployeeId(employeeId).Where(x => !x.Deleted && x.InventoryId != 0 && x.IsRead && !x.Rejected).ToList();
                if (requests != null && requests.Any())
                {
                    return Ok(requests);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in: {nameof(GetAcceptedRequests)}, exception: {ex.Message}.");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("notreadrequests/{employeeId?}")]
        public ActionResult GetUnreadRequests(int? employeeId)
        {
            try
            {
                var requests = _requestRepository.GetRequestsByEmployeeId( employeeId).Where(x => !x.Deleted && !x.IsRead && !x.Rejected && x.InventoryId == 0).ToList();
                if (requests != null && requests.Any())
                {
                    return Ok(requests);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in: {nameof(GetUnreadRequests)}, exception: {ex.Message}.");
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("allrequests/{employeeId?}")]
        public ActionResult GetAllRequests(int? employeeId)
        {
            try
            {
                var requests = _requestRepository.GetRequestsByEmployeeId(employeeId).Where(x => !x.Deleted && x.InventoryTypeId!= 0 ).ToList();
                if (requests != null && requests.Any())
                {
                    return Ok(requests);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in: {nameof(GetAllRequests)}, exception: {ex.Message}.");
                return BadRequest(new { message = ex.Message });
            }
        }

        [Route("NoOfUnreadRequests")]
        [HttpGet]
         
        public ActionResult GetNoOfUnreadRequests()
        {
            try
            {
                var unreadRequests = _requestRepository.GetNoOfUnreadRequests().Where(x => !x.Deleted && !x.IsRead).ToList();
                var unreadRequestsCount = unreadRequests.Count();
                return Ok(unreadRequestsCount);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in: {nameof(GetAllRequests)}, exception: {ex.Message}.");
                return BadRequest(new { message = ex.Message });
            }
        }

        public class RequestCounts
        {
            public int UnreadRequests { get; set; }
            public int TotalRequests { get; set; }
            public int AcceptedRequests { get; set; }
            public int RejectedRequests { get; set; }
            public int PendingRequests { get; set; }
        }

        [Route("requestCounts")]
        [HttpGet]
        public ActionResult GetRequestCounts()
        {
            try
            {
                var unreadRequests = _requestRepository.GetNoOfUnreadRequests().Where(x => !x.Deleted && !x.IsRead).ToList();
                var totalRequests = _requestRepository.GetNoOfAllRequests().Where(i => !i.Deleted && i.InventoryTypeId != 0).ToList();
                var acceptedRequests = _requestRepository.GetNoOfAcceptedRequests().Where(x => !x.Deleted && x.InventoryId != 0 && x.IsRead && !x.Rejected).ToList();
                var rejectedRequests = _requestRepository.GetNoOfRejectedRequests().Where(x => !x.Deleted && x.Rejected && x.IsRead && x.InventoryId == 0).ToList();
                var pendingRequests = _requestRepository.GetNoOfPendingRequests().Where(x => !x.Deleted && x.IsRead && !x.Rejected && x.InventoryId == 0).ToList();

                var response = new RequestCounts
                {
                    UnreadRequests = unreadRequests.Count(),
                    TotalRequests = totalRequests.Count(),
                    AcceptedRequests = acceptedRequests.Count(),
                    RejectedRequests = rejectedRequests.Count(),
                    PendingRequests = pendingRequests.Count()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in {nameof(GetRequestCounts)}: {ex.Message}");
                return StatusCode(500, $"An error occurred while fetching request counts: {ex.Message}");
            }
        }


    }
}




      
   