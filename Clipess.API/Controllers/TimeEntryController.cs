using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;
using log4net;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;


namespace Clipess.API.Controllers
{
    public enum TimeEntryTypes
    {
        CheckIn = 1,
        LunchIn = 2,
        LunchOut = 3,
        CheckOut = 4,
        Task = 5
    }



    [Route("api/TimeEntry")]
    [ApiController]
    public class TimeEntryController : ControllerBase
    {
        private readonly ITimeEntryRepository _timeEntryRepository;
        public static ILog _logger;

        public TimeEntryController(ITimeEntryRepository timeEntryRepository)
        {
            _timeEntryRepository = timeEntryRepository;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }

        [HttpGet]
        [Route("GetTimeEntryTypesByEmployee")]
        public async Task<IActionResult> GetTimeEntryTypesByEmployee()
        {
            try
            {
                var timeEntryTypes = _timeEntryRepository.GetTimeEntryTypesByEmployee();
                if (timeEntryTypes == null)
                {
                    return NoContent();
                }
                return Ok(timeEntryTypes);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error has occured in: {nameof(GetTimeEntryTypesByEmployee)}, exception: {ex.Message}.");
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("GetTimeEntriesByEmployee")]
        public async Task<IActionResult> GetTimeEntriesByEmployee([FromQuery] int id, DateTime date)
        {
            try
            {
                var timeEntries = _timeEntryRepository.GetTimeEntriesByEmployee(id, date);
                if (timeEntries == null)
                {
                    return NoContent();
                }
                return Ok(timeEntries);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error has occured in: {nameof(GetTimeEntriesByEmployee)} for id: {id}, exception: {ex.Message}.");
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("CreateTimeEntriesByEmployee")]
        public async Task<IActionResult> CreateTimeEntriesByEmployee([FromBody] TimeEntry receivingTimeDetails)
        {
            try
            {
                var time = new TimeEntry
                {
                    EmployeeId = receivingTimeDetails.EmployeeId,
                    TimeEntryTypeId = receivingTimeDetails.TimeEntryTypeId,
                    CreatedDate = DateTime.Now,
                    Duration = receivingTimeDetails.Duration,
                    Description = receivingTimeDetails.Description,
                };
                await _timeEntryRepository.CreateTimeEntriesByEmployee(time);

                if (receivingTimeDetails.TimeEntryTypeId == (int)TimeEntryTypes.CheckIn)
                {
                    await _timeEntryRepository.CreateDailyTimeEntryCheckinByEmployee(receivingTimeDetails.EmployeeId,
                        DateTime.Now);
                }
                else if (receivingTimeDetails.TimeEntryTypeId == (int)TimeEntryTypes.LunchIn || receivingTimeDetails.TimeEntryTypeId == (int)TimeEntryTypes.LunchOut
                    || receivingTimeDetails.TimeEntryTypeId == (int)TimeEntryTypes.CheckOut)
                {
                    await _timeEntryRepository.UpdateDailyTimeEntryOtherTypesByEmployees(receivingTimeDetails.EmployeeId,
                        DateTime.Now, receivingTimeDetails.TimeEntryTypeId);
                }
                return Ok(time);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error has occured in: {nameof(CreateTimeEntriesByEmployee)} for exception: {ex.Message}.");
                return BadRequest();
            }
        }

        [HttpDelete]
        [Route("DeleteTimeEntryTasksByEmployee")]
        public async Task<IActionResult> DeleteTimeEntryTasksByEmployee([FromQuery] int id)
        {
            try
            {
                var timeEntries = await _timeEntryRepository.DeleteTimeEntryTasksByEmployee(id);
                if (timeEntries == null)
                {
                    return NoContent();
                }
                return Ok(timeEntries);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error has occured in: {nameof(DeleteTimeEntryTasksByEmployee)} for id: {id}, exception: {ex.Message}.");
                return BadRequest();
            }
        }

        [HttpPut]
        [Route("UpdateTimeEntryTasksByEmployee")]
        public async Task<IActionResult> UpdateTimeEntryTasksByEmployee([FromQuery] int id, int duration, string description)
        {
            try
            {
                var timeEntry = await _timeEntryRepository.UpdateTimeEntryTasksByEmployee(id, duration, description);
                if (timeEntry == null)
                {
                    return NoContent();
                }
                return Ok(timeEntry);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error has occured in: {nameof(UpdateTimeEntryTasksByEmployee)} for id: {id}, exception: {ex.Message}.");
                return BadRequest();
            }
        }

        [HttpDelete]
        [Route("DeleteTimeEntryEventsByEmployee")]
        public async Task<IActionResult> DeleteTimeEntryEventsByEmployee([FromQuery] int timeEntryId, int employeeId, DateTime date, int typeId)
        {
            try
            {
                var timeEntry = await _timeEntryRepository.DeleteTimeEntryEventsByEmployee(timeEntryId, employeeId, date, typeId);
                if (timeEntry == null)
                {
                    return NoContent();
                }
                return Ok(timeEntry);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error has occured in: {nameof(DeleteTimeEntryEventsByEmployee)} , exception: {ex.Message}.");
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("GetDailyTimeEntriesByEmployee")]
        public async Task<IActionResult> GetDailyTimeEntriesByEmployee([FromQuery] int? id, DateTime startDate, DateTime endDate,
            [FromQuery] string? sortBy, [FromQuery] bool? isAscending, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
        {
            try
            {
                var dailyTimeEntry = _timeEntryRepository.GetDailyTimeEntriesByEmployee(id, startDate, endDate, sortBy, isAscending ?? true, pageNumber, pageSize).Select(x => new
                {
                    x.EmployeeId,
                    Date = x.Date.ToString("yyyy-MM-dd"),
                    CheckIn = x.CheckIn.HasValue ? x.CheckIn.Value.ToString("hh:mm tt") : null,
                    CheckOut = x.CheckOut.HasValue ? x.CheckOut.Value.ToString("hh:mm tt") : null,
                    LunchIn = x.LunchIn.HasValue ? x.LunchIn.Value.ToString("hh:mm tt") : null,
                    LunchOut = x.LunchOut.HasValue ? x.LunchOut.Value.ToString("hh:mm tt") : null,
                    x.TotalDuration,
                    FirstName = x.Employee.FirstName ?? null,
                    LastName = x.Employee.LastName ?? null,
                }).ToList();

                if (dailyTimeEntry == null)
                {
                    return Ok("No Content");
                }
                return Ok(dailyTimeEntry);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error has occured in: {nameof(GetDailyTimeEntriesByEmployee)} for id: {id}, exception: {ex.Message}.");
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("GetMonthlyTimeEntriesByEmployee")]
        public async Task<IActionResult> GetMonthlyTimeEntriesByEmployee([FromQuery] int? employeeId, string year,
            [FromQuery] string? sortBy, [FromQuery] bool? isAscending, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
        {
            try
            {
                var monthlyTimeEntry = _timeEntryRepository.GetMonthlyTimeEntriesByEmployee(employeeId, year, sortBy, isAscending ?? true, pageNumber, pageSize)
                     .Select(x => new
                     {
                         x.EmployeeId,
                         x.Month,
                         x.AllocatedDuration,
                         x.CompletedDuration,
                         FirstName = x.Employee.FirstName ?? null,
                         LastName = x.Employee.LastName ?? null,

                     }).ToList();

                if (monthlyTimeEntry == null)
                {
                    return NoContent();
                }
                return Ok(monthlyTimeEntry);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error has occured in: {nameof(GetMonthlyTimeEntriesByEmployee)} , exception: {ex.Message}.");
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("GetTimeEntryTasks")]
        public async Task<IActionResult> GetTimeEntryTasks([FromQuery] int id, DateTime startDate, DateTime endDate)
        {
            try
            {
                var tasks = _timeEntryRepository.GetTimeEntryTasks().Where(x => x.EmployeeId == id && x.CreatedDate.Date >= startDate.Date && x.CreatedDate.Date <= endDate.Date && x.Deleted == false)
                    .Select(x => new
                    {
                        x.EmployeeId,
                        x.Description,
                        x.Duration,
                        CreatedDate = x.CreatedDate.ToString("hh:mm tt")
                    }).ToList();

                if (tasks == null)
                {
                    return NoContent();
                }
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error has occured in: {nameof(GetTimeEntryTasks)} for id: {id}, exception: {ex.Message}.");
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("GetMonthlyTimeEntriesByManager")]
        public async Task<IActionResult> GetMonthlyTimeEntriesByManager([FromQuery] string month, int? id, [FromQuery] string? sortBy,
     [FromQuery] bool? isAscending, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
        {
            try
            {
                var monthlyTimeEntry = _timeEntryRepository.GetMonthlyTimeEntriesByManager(month, id, sortBy, isAscending ?? true, pageNumber, pageSize)
                    .Select(x => new
                    {
                        x.EmployeeId,
                        x.Month,
                        x.AllocatedDuration,
                        x.CompletedDuration,
                        FirstName = x.Employee.FirstName ?? null,
                        LastName = x.Employee.LastName ?? null,
                    }).ToList();

                if (monthlyTimeEntry == null)
                {
                    return NoContent();
                }

                return Ok(monthlyTimeEntry);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error has occured in: {nameof(GetMonthlyTimeEntriesByManager)} , exception: {ex.Message}.");
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("GetDailyTimeEntriesByManager")]
        public async Task<IActionResult> GetDailyTimeEntriesByManager([FromQuery]  DateTime startDate, DateTime endDate, int? id,
string? sortBy, bool? isAscending, int pageNumber = 1,  int pageSize = 1000)
        {
            try
            {
                var dailyTimeEntry = _timeEntryRepository.GetDailyTimeEntriesByManager(startDate, endDate, id, sortBy, isAscending ?? true, pageNumber, pageSize).Select(x => new
                {
                    x.DailyTimeEntryId,
                    x.EmployeeId,
                    Date = x.Date.ToString("yyyy-MM-dd"),
                    CheckIn = x.CheckIn.HasValue ? x.CheckIn.Value.ToString("hh:mm tt") : null,
                    CheckOut = x.CheckOut.HasValue ? x.CheckOut.Value.ToString("hh:mm tt") : null,
                    LunchIn = x.LunchIn.HasValue ? x.LunchIn.Value.ToString("hh:mm tt") : null,
                    LunchOut = x.LunchOut.HasValue ? x.LunchOut.Value.ToString("hh:mm tt") : null,
                    x.TotalDuration,
                    FirstName = x.Employee.FirstName ?? null,
                    LastName = x.Employee.LastName ?? null,
                }).ToList();

                if (dailyTimeEntry == null)
                {
                    return NoContent();
                }

                return Ok(dailyTimeEntry);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error has occured in: {nameof(GetDailyTimeEntriesByManager)}, exception: {ex.Message}.");
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("GetEmployeesByManager")]
        public async Task<IActionResult> GetEmployeesByManager()
        {
            try
            {
                var employees = _timeEntryRepository.GetEmployeesByManager();

                if (employees == null)
                {
                    return NoContent();
                }
                return Ok(employees);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error has occured in: {nameof(GetEmployeesByManager)} , exception: {ex.Message}.");
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("CreateMonthlyTimeEntriesByManager")]
        public async Task<IActionResult> CreateMonthlyTimeEntriesByManager([FromQuery] int allocatedTime, string workingMonth)
        {
            try
            {
                var monthlyTimeEntries = await _timeEntryRepository.CreateMonthlyTimeEntriesByManager(allocatedTime, workingMonth);
                if (monthlyTimeEntries == null)
                {
                    return null;
                }
                return Ok(monthlyTimeEntries);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error has occured in: {nameof(CreateMonthlyTimeEntriesByManager)} for exception: {ex.Message}.");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        [Route("UpdateMonthlyTimeEntriesByManager")]
        public async Task<IActionResult> UpdateMonthlyTimeEntriesByManager([FromQuery] int allocatedTime, string month)
        {
            try
            {
                var records = await _timeEntryRepository.UpdateMonthlyTimeEntriesByManager(allocatedTime, month);
                if (records == null)
                {
                    return NoContent();
                }
                return Ok(records);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error has occured in: {nameof(UpdateMonthlyTimeEntriesByManager)} , exception: {ex.Message}.");
                return BadRequest();
            }
        }

        [HttpDelete]
        [Route("DeleteMonthlyTimeEntriesByManager")]
        public async Task<IActionResult> DeleteMonthlyTimeEntriesByManager([FromQuery] string month)
        {
            try
            {
                var records = await _timeEntryRepository.DeleteMonthlyTimeEntriesByManager(month);
                if (records == null)
                {
                    return NoContent();
                }
                return Ok(records);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error has occured in: {nameof(DeleteMonthlyTimeEntriesByManager)} , exception: {ex.Message}.");
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("GetMonthlyWorkingDays")]
        public async Task<IActionResult> GetMonthlyWorkingDays()
        {
            try
            {
                var monthlyWorkingDays = _timeEntryRepository.GetMonthlyWorkingDays();
                if (monthlyWorkingDays == null)
                {
                    return NoContent();
                }
                return Ok(monthlyWorkingDays);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error has occured in: {nameof(GetMonthlyWorkingDays)} , exception: {ex.Message}.");
                return BadRequest();
            }
        }



        [HttpPost]
        [Route("CreateMonthlyWorkingDaysByManager")]
        public async Task<IActionResult> CreateMonthlyWorkingDaysByManager([FromBody] List<string> selectedDates)
        {
            try
            {
                var parsedDates = selectedDates.Select(date => DateTime.Parse(date)).ToList();
                var monthlyWorkingDays = await _timeEntryRepository.CreateMonthlyWorkingDaysByManager(parsedDates);
                if (monthlyWorkingDays == null)
                {
                    return NoContent();
                }
                return Ok(monthlyWorkingDays);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error has occured in: {nameof(CreateMonthlyWorkingDaysByManager)} , exception: {ex.Message}.");
                return BadRequest();
            }
        }



        [HttpPut]
        [Route("UpdateMonthlyWorkingDaysByManager")]
        public async Task<IActionResult> UpdateMonthlyWorkingDaysByManager([FromBody] List<string> selectedDates)
        {
            try
            {
                var parsedDates = selectedDates.Select(date => DateTime.Parse(date)).ToList();
                var monthlyWorkingDays = await _timeEntryRepository.UpdateMonthlyWorkingDaysByManager(parsedDates);
                if (monthlyWorkingDays == null)
                {
                    return NoContent();
                }
                return Ok(monthlyWorkingDays);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error has occured in: {nameof(UpdateMonthlyWorkingDaysByManager)} , exception: {ex.Message}.");
                return BadRequest();
            }
        }



        [HttpDelete]
        [Route("DeleteMonthlyWorkingDaysByManager")]
        public async Task<IActionResult> DeleteMonthlyWorkingDaysByManager([FromQuery] string month)
        {
            try
            {
                var monthlyWorkingDays = await _timeEntryRepository.DeleteMonthlyWorkingDaysByManager(month);
                if (monthlyWorkingDays == null)
                {
                    return NoContent();
                }
                return Ok(monthlyWorkingDays);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error has occured in: {nameof(DeleteMonthlyWorkingDaysByManager)} , exception: {ex.Message}.");
                return BadRequest();
            }
        }

        //Employee Pie Chart
        [HttpGet]
        [Route("GetMonthlyTimeEntryForPieChart")]
        public async Task<IActionResult> GetMonthlyTimeEntryForPieChart([FromQuery] int employeeId, DateTime currentDate)
        {
            try
            {
                var monthlyTimeEntry = _timeEntryRepository.GetMonthlyTimeEntryForPieChart(employeeId, currentDate);
                if (monthlyTimeEntry == null)
                {
                    return NoContent();
                }
                return Ok(monthlyTimeEntry);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error has occured in: {nameof(GetMonthlyTimeEntryForPieChart)} , exception: {ex.Message}.");
                return BadRequest();
            }
        }

        //Manager Pie Chart
        [HttpGet]
        [Route("GetMonthlyTimeEntryForBarChart")]
        public async Task<IActionResult> GetMonthlyTimeEntryForBarChart([FromQuery] DateTime currentDate)
        {
            try
            {
                var monthlyTimeEntries = _timeEntryRepository.GetMonthlyTimeEntryForBarChart(currentDate).Select(x => new
                {
                    x.EmployeeId,
                    x.Month,
                    x.AllocatedDuration,
                    x.CompletedDuration,
                    FirstName = x.Employee.FirstName ?? null,
                    LastName = x.Employee.LastName ?? null,
                }).ToList(); ;

                if (monthlyTimeEntries == null)
                {
                    return NoContent();
                }

                return Ok(monthlyTimeEntries);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error has occured in: {nameof(GetMonthlyTimeEntryForBarChart)} , exception: {ex.Message}.");
                return BadRequest();
            }
        }

        [HttpPut]
        [Route("UpdateDailyTimeEntriesByManager")]
        public async Task<IActionResult> UpdateDailyTimeEntriesByManager([FromBody] DailyTimeEntry receivingDailyTimeEntry)
        {
            try
            {
                var dailyTimeEntry = new DailyTimeEntry
                {
                    DailyTimeEntryId = receivingDailyTimeEntry.DailyTimeEntryId,
                    EmployeeId = receivingDailyTimeEntry.EmployeeId,
                    Date = receivingDailyTimeEntry.Date,
                    CheckIn = receivingDailyTimeEntry.CheckIn,
                    LunchIn = receivingDailyTimeEntry.LunchIn,
                    LunchOut = receivingDailyTimeEntry.LunchOut,
                    CheckOut = receivingDailyTimeEntry.CheckOut,
                };

                var records = await _timeEntryRepository.UpdateDailyTimeEntriesByManager(dailyTimeEntry);

                if (records == null)
                {
                    return null;
                }

                return Ok(records);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error has occured in: {nameof(UpdateDailyTimeEntriesByManager)} , exception: {ex.Message}.");
                return BadRequest();
            }
        }
    }
}
