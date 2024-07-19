using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;
using Microsoft.EntityFrameworkCore;

namespace Clipess.DBClient.Repositories
{
    public enum TimeEntryTypes
    {
        CheckIn = 1,
        LunchIn = 2,
        LunchOut = 3,
        CheckOut = 4,
        Task = 5
    }
    public class EFTimeEntryRepository : ITimeEntryRepository
    {
        public EFDbContext _DbContext { get; set; }

        public EFTimeEntryRepository(EFDbContext dbContext)
        {
            _DbContext = dbContext;
        }

        public IQueryable<TimeEntryType> GetTimeEntryTypesByEmployee()
        {
            return _DbContext.TimeEntryTypes;
        }
   
        public IQueryable<TimeEntry> GetTimeEntriesByEmployee(int id, DateTime date)
        {
            var timeEntries = _DbContext.TimeEntries.Where(x => x.EmployeeId == id && x.CreatedDate.Date == date.Date && x.Deleted == false);
            return timeEntries;
        }

        public async Task<TimeEntry> CreateTimeEntriesByEmployee(TimeEntry timeEntry)
        {
            await _DbContext.TimeEntries.AddAsync(timeEntry);
            await _DbContext.SaveChangesAsync();
            return timeEntry;
        }

        public async Task<DailyTimeEntry> CreateDailyTimeEntryCheckinByEmployee(int id, DateTime date)
        {
            var newDailyTimeEntry = new DailyTimeEntry
            {
                EmployeeId = id,
                Date = date,
                CheckIn = DateTime.Now
            };
            await _DbContext.DailyTimeEntries.AddAsync(newDailyTimeEntry);
            await _DbContext.SaveChangesAsync();
            return newDailyTimeEntry;
        }

        public async Task<DailyTimeEntry> UpdateDailyTimeEntryOtherTypesByEmployees(int id, DateTime date, int typeid)
        {
            var existingemployee = await _DbContext.DailyTimeEntries.FirstOrDefaultAsync(x => x.EmployeeId == id && x.Date.Date == date.Date);
           
            if (existingemployee == null)
            {
                return null;
            }

            if (typeid == (int)TimeEntryTypes.LunchIn)
            {
                existingemployee.LunchIn = DateTime.Now;
            }
            else if (typeid == (int)TimeEntryTypes.LunchOut)
            {
                existingemployee.LunchOut = DateTime.Now;
            }
            else if (typeid == (int)TimeEntryTypes.CheckOut)
            {
                existingemployee.CheckOut = DateTime.Now;
            }

            if (existingemployee.CheckIn != null && existingemployee.CheckOut != null &&
                existingemployee.LunchIn != null && existingemployee.LunchOut != null)
            {
                TimeSpan WorkingDuration = existingemployee.CheckOut.Value - existingemployee.CheckIn.Value;
                TimeSpan LunchDuration = existingemployee.LunchOut.Value - existingemployee.LunchIn.Value;
                existingemployee.TotalDuration = (int)(WorkingDuration.TotalMinutes - LunchDuration.TotalMinutes);   // Problem - Code provides error without ".TotalMinutes"
                
                string workingMonth = existingemployee.Date.ToString("yyyy-MM");

                if (existingemployee.TotalDuration != null)
                {
                    var monthlyTimeEntry = await _DbContext.MonthlyTimeEntries.FirstOrDefaultAsync(m => m.EmployeeId == id && m.Month == workingMonth);
                    if (monthlyTimeEntry != null)
                    {
                        if (monthlyTimeEntry.CompletedDuration == null)
                        {
                            monthlyTimeEntry.CompletedDuration = existingemployee.TotalDuration;
                        }
                        else
                        {
                            monthlyTimeEntry.CompletedDuration += existingemployee.TotalDuration;
                        }
                        await _DbContext.SaveChangesAsync();
                    }
                }
            }
            await _DbContext.SaveChangesAsync();
            return existingemployee;
        }

        public async Task<TimeEntry> DeleteTimeEntryTasksByEmployee(int id)
        {
            var timeEntry = await _DbContext.TimeEntries.FirstOrDefaultAsync(x => x.TimeEntryId == id);
            timeEntry.Deleted = true;
            _DbContext.SaveChanges();
            return timeEntry;
        }

        public async Task<TimeEntry> UpdateTimeEntryTasksByEmployee(int id, int duration, string description)
        {
            var timeEntry = await _DbContext.TimeEntries.FirstOrDefaultAsync(x => x.TimeEntryId == id);
            
            if (timeEntry == null)
            {
                return null;
            }

            timeEntry.Duration = duration;
            timeEntry.Description = description;
            _DbContext.SaveChanges();
            return timeEntry;
        }

        public async Task<TimeEntry> DeleteTimeEntryEventsByEmployee(int timeEntryId, int employeeId, DateTime date, int typeId)
        {
            var timeEntry = await _DbContext.TimeEntries.FirstOrDefaultAsync(x => x.TimeEntryId == timeEntryId);
            
            if (timeEntry == null)
            {
                return null;
            }

            timeEntry.Deleted = true;

            var dailyTimeEntry = await _DbContext.DailyTimeEntries.FirstOrDefaultAsync(x => x.EmployeeId == employeeId && x.Date.Date == date.Date);
           
            if (dailyTimeEntry == null)
            {
                return null;
            }

            if (typeId == (int)TimeEntryTypes.CheckIn)
            {
                _DbContext.DailyTimeEntries.Remove(dailyTimeEntry);
            }
            if (typeId == (int)TimeEntryTypes.LunchIn)
            {
                dailyTimeEntry.LunchIn = null;
            }
            if (typeId == (int)TimeEntryTypes.LunchOut)
            {
                dailyTimeEntry.LunchOut = null;
            }
            if (typeId == (int)TimeEntryTypes.CheckOut)
            {
                dailyTimeEntry.CheckOut = null;
                var existingDailyTotalDuration = dailyTimeEntry.TotalDuration;
                string month = dailyTimeEntry.Date.ToString("yyyy-MM");
                var monthlyTimeEntry = _DbContext.MonthlyTimeEntries.FirstOrDefault(x => x.EmployeeId == employeeId && x.Month == month);
                dailyTimeEntry.TotalDuration = null;
                if (monthlyTimeEntry != null)
                {
                    var existingMonthlyCompletedDuration = monthlyTimeEntry.CompletedDuration;
                    monthlyTimeEntry.CompletedDuration = existingMonthlyCompletedDuration - existingDailyTotalDuration;
                }
            }
            _DbContext.SaveChanges();
            return timeEntry;
        }

        public IQueryable<DailyTimeEntry> GetDailyTimeEntriesByEmployee(int? id, DateTime startDate, 
            DateTime endDate, string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000)
        {
            var dailytimeEntries = _DbContext.DailyTimeEntries.Where(x => x.EmployeeId==id && x.Date.Date >= startDate.Date && x.Date.Date <= endDate.Date);

            var dailyTimeEntriesWithEmployees = dailytimeEntries.Select(x => new
            {
                DailyTimeEntry = x,

                Employee = _DbContext.Employees.FirstOrDefault(u => u.EmployeeID == x.EmployeeId)
            });

            if (string.IsNullOrWhiteSpace(sortBy) == false)
            {
                if (sortBy.Equals("Date", StringComparison.OrdinalIgnoreCase))
                {
                    dailyTimeEntriesWithEmployees = isAscending ? dailyTimeEntriesWithEmployees.OrderBy(x => x.DailyTimeEntry.Date) : dailyTimeEntriesWithEmployees.OrderByDescending(x => x.DailyTimeEntry.Date);
                }
                else if (sortBy.Equals("Duration", StringComparison.OrdinalIgnoreCase))
                {
                    dailyTimeEntriesWithEmployees = isAscending ? dailyTimeEntriesWithEmployees.OrderBy(x => x.DailyTimeEntry.TotalDuration) : dailyTimeEntriesWithEmployees.OrderByDescending(x => x.DailyTimeEntry.TotalDuration);
                }
            }

            var skipResults = (pageNumber - 1) * pageSize;
            var paginatedResults = dailyTimeEntriesWithEmployees.Skip(skipResults).Take(pageSize);

            var finalResults = paginatedResults.Select(x => new DailyTimeEntry
            {
                DailyTimeEntryId = x.DailyTimeEntry.DailyTimeEntryId,
                EmployeeId = x.DailyTimeEntry.EmployeeId,
                Date = x.DailyTimeEntry.Date,
                CheckIn = x.DailyTimeEntry.CheckIn,
                LunchIn = x.DailyTimeEntry.LunchIn,
                LunchOut = x.DailyTimeEntry.LunchOut,
                CheckOut = x.DailyTimeEntry.CheckOut,
                TotalDuration = x.DailyTimeEntry.TotalDuration,
                Employee = x.Employee
            });

            return finalResults.AsQueryable();
        }

        public IQueryable<MonthlyTimeEntry> GetMonthlyTimeEntriesByEmployee(int? employeeId, string year, string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000)
        {
            var monthlyTimeEntries = _DbContext.MonthlyTimeEntries.Where(x => x.EmployeeId == employeeId && x.Month.Substring(0, 4) == year);

            var monthlyTimeEntriesWithEmployees = monthlyTimeEntries.Select(x => new
            {
                MonthlyTimeEntry = x,
                Employee = _DbContext.Employees.FirstOrDefault(u => u.EmployeeID == x.EmployeeId)
            });

            if (string.IsNullOrWhiteSpace(sortBy) == false)
            {
                if (sortBy.Equals("Completed", StringComparison.OrdinalIgnoreCase))
                {
                    monthlyTimeEntriesWithEmployees = isAscending ? monthlyTimeEntriesWithEmployees.OrderBy(x => x.MonthlyTimeEntry.CompletedDuration) : monthlyTimeEntriesWithEmployees.OrderByDescending(x => x.MonthlyTimeEntry.CompletedDuration);
                }
            }

            var skipResults = (pageNumber - 1) * pageSize;
            var paginatedResults = monthlyTimeEntriesWithEmployees.Skip(skipResults).Take(pageSize);

            var finalResults = paginatedResults.Select(x => new MonthlyTimeEntry
            {
                MonthlyTimeEntryId = x.MonthlyTimeEntry.MonthlyTimeEntryId,
                Month = x.MonthlyTimeEntry.Month,
                EmployeeId = x.MonthlyTimeEntry.EmployeeId,
                AllocatedDuration = x.MonthlyTimeEntry.AllocatedDuration,
                CompletedDuration = x.MonthlyTimeEntry.CompletedDuration,
                Employee = x.Employee
            });

            return finalResults.AsQueryable();
        }

        public IQueryable<TimeEntry> GetTimeEntryTasks()
        {
            return _DbContext.TimeEntries.Where( x => x.TimeEntryTypeId == (int)TimeEntryTypes.Task);
        }

        public IQueryable<DailyTimeEntry> GetDailyTimeEntriesByManager(DateTime startDate, DateTime endDate, int? id, string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000)
        {
            var dailyEntries = _DbContext.DailyTimeEntries.Where(x => x.Date.Date >= startDate.Date && x.Date.Date <= endDate.Date);
  
            if (id.HasValue)
            {
                dailyEntries = dailyEntries.Where(x => x.EmployeeId == id);
            }

            var dailyEntriesWithUsers = dailyEntries.Select(x => new
            {
                DailyTimeEntry = x,
                Employee = _DbContext.Employees.FirstOrDefault(u => u.EmployeeID == x.EmployeeId)
            });

            if (string.IsNullOrWhiteSpace(sortBy) == false)
            {
                if (sortBy.Equals("Date", StringComparison.OrdinalIgnoreCase))
                {
                    dailyEntriesWithUsers = isAscending ? dailyEntriesWithUsers.OrderBy(x => x.DailyTimeEntry.Date) : dailyEntriesWithUsers.OrderByDescending(x => x.DailyTimeEntry.Date);
                }
                else if (sortBy.Equals("Duration", StringComparison.OrdinalIgnoreCase))
                {
                    dailyEntriesWithUsers = isAscending ? dailyEntriesWithUsers.OrderBy(x => x.DailyTimeEntry.TotalDuration) : dailyEntriesWithUsers.OrderByDescending(x => x.DailyTimeEntry.TotalDuration);
                }
            }

            var skipResults = (pageNumber - 1) * pageSize;
            var paginatedResults = dailyEntriesWithUsers.Skip(skipResults).Take(pageSize);

            var finalResults = paginatedResults.Select(x => new DailyTimeEntry
            {
                DailyTimeEntryId = x.DailyTimeEntry.DailyTimeEntryId,
                EmployeeId = x.DailyTimeEntry.EmployeeId,
                Date = x.DailyTimeEntry.Date,
                CheckIn = x.DailyTimeEntry.CheckIn,
                LunchIn = x.DailyTimeEntry.LunchIn,
                LunchOut = x.DailyTimeEntry.LunchOut,
                CheckOut = x.DailyTimeEntry.CheckOut,
                TotalDuration = x.DailyTimeEntry.TotalDuration,
                Employee = x.Employee
            });

            return finalResults.AsQueryable();
        }

        public IQueryable<MonthlyTimeEntry> GetMonthlyTimeEntriesByManager(string startMonth, int? id, string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000)
        {
            var monthlyEntries = _DbContext.MonthlyTimeEntries.Where(x => x.Month == startMonth);

            if (id.HasValue)
            {
                monthlyEntries = monthlyEntries.Where(x => x.EmployeeId == id);
            }

            var monthlyEntriesWithUsers = monthlyEntries.Select(x => new
            {
                MonthlyTimeEntry = x,
                Employee = _DbContext.Employees.FirstOrDefault(u => u.EmployeeID == x.EmployeeId)
            });

            if (string.IsNullOrWhiteSpace(sortBy) == false)
            {
                if (sortBy.Equals("Completed", StringComparison.OrdinalIgnoreCase))
                {
                    monthlyEntriesWithUsers = isAscending ? monthlyEntriesWithUsers.OrderBy(x => x.MonthlyTimeEntry.CompletedDuration) : monthlyEntriesWithUsers.OrderByDescending(x => x.MonthlyTimeEntry.CompletedDuration);
                }
            }

            var skipResults = (pageNumber - 1) * pageSize;
            var paginatedResults = monthlyEntriesWithUsers.Skip(skipResults).Take(pageSize);

            var finalResults = paginatedResults.Select(x => new MonthlyTimeEntry
            {
                MonthlyTimeEntryId = x.MonthlyTimeEntry.MonthlyTimeEntryId,
                Month = x.MonthlyTimeEntry.Month,
                EmployeeId = x.MonthlyTimeEntry.EmployeeId,
                AllocatedDuration = x.MonthlyTimeEntry.AllocatedDuration,
                CompletedDuration = x.MonthlyTimeEntry.CompletedDuration,
                Employee = x.Employee  // This will be null if there is no matching user
            });

            return finalResults.AsQueryable();
        }

        public IQueryable<Employee> GetEmployeesByManager()
        {

            var employees = _DbContext.Employees.Where( x => x.Deleted == false);
            if(employees == null)
            {
                return null;
            }
            return employees;
        }

        public async Task<List<MonthlyTimeEntry>> CreateMonthlyTimeEntriesByManager(int allocatedTime, string workingMonth)
        {
             var allEmployeeIds = await _DbContext.Employees.Where(x => x.Deleted == false).Select(u => u.EmployeeID).ToListAsync();

            var monthlyTimeEntries = new List<MonthlyTimeEntry>();
            foreach (var x in allEmployeeIds)
            {
                var monthlyTime = new MonthlyTimeEntry
                {
                    EmployeeId = x,
                    Month = workingMonth,
                    AllocatedDuration = allocatedTime
                };
                monthlyTimeEntries.Add(monthlyTime);
            }
 
            var existingRecords = _DbContext.MonthlyTimeEntries.Where(x => x.Month == workingMonth).ToList();
            _DbContext.MonthlyTimeEntries.RemoveRange(existingRecords);

            _DbContext.MonthlyTimeEntries.AddRange(monthlyTimeEntries);
            await _DbContext.SaveChangesAsync();
            return monthlyTimeEntries;

        }

        public async Task<List<MonthlyTimeEntry>> UpdateMonthlyTimeEntriesByManager(int allocatedTime, string month)
        {
            var allEmployeeRecords = await _DbContext.MonthlyTimeEntries.Where(x => x.Month == month).ToListAsync();
            
            foreach (var record in allEmployeeRecords)
            {
                record.AllocatedDuration = allocatedTime;
            }

            _DbContext.SaveChanges();
            return allEmployeeRecords;
        }

        public async Task<List<MonthlyTimeEntry>> DeleteMonthlyTimeEntriesByManager(string month)
        {
            var allEmployeeRecords = await _DbContext.MonthlyTimeEntries.Where(x => x.Month == month).ToListAsync();
            _DbContext.MonthlyTimeEntries.RemoveRange(allEmployeeRecords);
            _DbContext.SaveChanges();
            return allEmployeeRecords;
        }

        public IQueryable<MonthlyWorkingDay> GetMonthlyWorkingDays()
        {
            return _DbContext.MonthlyWorkingDays;
        }

        public async Task<List<MonthlyWorkingDay>> CreateMonthlyWorkingDaysByManager(List<DateTime> selectedDates)
        {
            string month = selectedDates.FirstOrDefault().ToString("yyyy-MM");
           
            if (string.IsNullOrEmpty(month))
            {
                return null;
            }

            int year = selectedDates.First().Year;
            int monthNumber = selectedDates.First().Month;
            int daysInMonth = DateTime.DaysInMonth(year, monthNumber);

            var monthlyWorkingDays = new List<MonthlyWorkingDay>();

            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(year, monthNumber, day);
                var dataType = selectedDates.Contains(date) ? "working" : "holiday";

                var monthlyWorkingDay = new MonthlyWorkingDay
                {
                    Month = month,
                    Date = date,
                    DateType = dataType,
                };
                monthlyWorkingDays.Add(monthlyWorkingDay);
            }

            var existingRecords = _DbContext.MonthlyWorkingDays.Where(x => x.Month == month).ToList();
            _DbContext.MonthlyWorkingDays.RemoveRange(existingRecords);

            _DbContext.MonthlyWorkingDays.AddRange(monthlyWorkingDays);
            _DbContext.SaveChanges();
            return monthlyWorkingDays;
        }

        public async Task<List<MonthlyWorkingDay>> UpdateMonthlyWorkingDaysByManager(List<DateTime> selectedDates)
        {
            string month = selectedDates.FirstOrDefault().ToString("yyyy-MM");
            
            if (string.IsNullOrEmpty(month))
            {
                return null;
            }

            int year = selectedDates.First().Year;
            int monthNumber = selectedDates.First().Month;
            int daysInMonth = DateTime.DaysInMonth(year, monthNumber);
            var existingRecords = _DbContext.MonthlyWorkingDays.Where(x => x.Month == month).ToList();

            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(year, monthNumber, day);
                bool isWorkingDay = selectedDates.Contains(date);
                var dataType = isWorkingDay ? "working" : "holiday";

                var existingRecord = existingRecords.FirstOrDefault(x => x.Date == date);
                if (existingRecord != null)
                {
                    existingRecord.DateType = dataType;
                }
            }
            _DbContext.SaveChanges();
            return existingRecords;
        }

        public async Task<List<MonthlyWorkingDay>> DeleteMonthlyWorkingDaysByManager(string month)
        {
            if (month == null)
            {
                return null;
            }

            var monthlyWorkingDays = _DbContext.MonthlyWorkingDays.Where(x => x.Month == month).ToList();
            if (monthlyWorkingDays == null)
            {
                return null;
            }

            _DbContext.MonthlyWorkingDays.RemoveRange(monthlyWorkingDays);
            _DbContext.SaveChanges();
            return monthlyWorkingDays;
        }

        public IQueryable<MonthlyTimeEntry> GetMonthlyTimeEntryForPieChart(int employeeId, DateTime currentDate)
        {
            string currentMonth = currentDate.ToString("yyyy-MM");
            var monthlyTimeEntry = _DbContext.MonthlyTimeEntries.Where(x => x.EmployeeId == employeeId && x.Month == currentMonth);
           
            if (monthlyTimeEntry == null)
            {
                throw new Exception("MonthlyTimeEntry Not Found");
            }

            return monthlyTimeEntry;    
        }

        public IQueryable<MonthlyTimeEntry> GetMonthlyTimeEntryForBarChart(DateTime currentDate)
        {
            string currentMonth = currentDate.ToString("yyyy-MM");
            var monthlyTimeEntries = _DbContext.MonthlyTimeEntries.Where(x => x.Month == currentMonth);

            if (monthlyTimeEntries == null)
            {
                throw new Exception("MonthlyTimeEntries Not Found");
            }

            var monthlyEntriesWithUsers = monthlyTimeEntries.Select(x => new
            {
                MonthlyTimeEntry = x,
                Employee = _DbContext.Employees.FirstOrDefault(u => u.EmployeeID == x.EmployeeId)
            });

            var finalResults = monthlyEntriesWithUsers.Select(x => new MonthlyTimeEntry
            {
                MonthlyTimeEntryId = x.MonthlyTimeEntry.MonthlyTimeEntryId,
                Month = x.MonthlyTimeEntry.Month,
                EmployeeId = x.MonthlyTimeEntry.EmployeeId,
                AllocatedDuration = x.MonthlyTimeEntry.AllocatedDuration,
                CompletedDuration = x.MonthlyTimeEntry.CompletedDuration,
                Employee = x.Employee// This will be null if there is no matching user
            });
            return finalResults.AsQueryable();
        }

        public async Task<DailyTimeEntry> UpdateDailyTimeEntriesByManager(DailyTimeEntry dailyTimeEntry)
        {
            var existingDailyTimeEntry =  _DbContext.DailyTimeEntries.FirstOrDefault(x => x.DailyTimeEntryId == dailyTimeEntry.DailyTimeEntryId);
            var oldTotalDuration = existingDailyTimeEntry.TotalDuration;
            if (existingDailyTimeEntry == null)
            {
                throw new Exception("Record not found");
            }
            else
            {
                existingDailyTimeEntry.CheckIn = dailyTimeEntry.CheckIn;
                existingDailyTimeEntry.CheckOut = dailyTimeEntry.CheckOut;
                existingDailyTimeEntry.LunchIn = dailyTimeEntry.LunchIn;
                existingDailyTimeEntry.LunchOut = dailyTimeEntry.LunchOut;   
            }

            if (existingDailyTimeEntry.CheckIn != null && existingDailyTimeEntry.CheckOut != null &&
            existingDailyTimeEntry.LunchIn != null && existingDailyTimeEntry.LunchOut != null)
            {
                TimeSpan WorkingDuration = existingDailyTimeEntry.CheckOut.Value - existingDailyTimeEntry.CheckIn.Value;
                TimeSpan LunchDuration = existingDailyTimeEntry.LunchOut.Value - existingDailyTimeEntry.LunchIn.Value;
                existingDailyTimeEntry.TotalDuration = (int)(WorkingDuration.TotalMinutes - LunchDuration.TotalMinutes);   // Problem - Code provides error without ".TotalMinutes"

                string workingMonth = existingDailyTimeEntry.Date.ToString("yyyy-MM");

                if (existingDailyTimeEntry.TotalDuration != null)
                {
                    var monthlyTimeEntry = _DbContext.MonthlyTimeEntries.FirstOrDefault(m => m.EmployeeId == dailyTimeEntry.EmployeeId && m.Month == workingMonth);

                    if (monthlyTimeEntry != null)
                    {
                        if (monthlyTimeEntry.CompletedDuration == null)
                        {
                            monthlyTimeEntry.CompletedDuration = existingDailyTimeEntry.TotalDuration;
                        }
                        else
                        {
                            if (oldTotalDuration != null)
                            {
                                monthlyTimeEntry.CompletedDuration = monthlyTimeEntry.CompletedDuration - oldTotalDuration + existingDailyTimeEntry.TotalDuration;
                            }
                            else
                            {
                                monthlyTimeEntry.CompletedDuration = monthlyTimeEntry.CompletedDuration + existingDailyTimeEntry.TotalDuration;
                            }
                        }
                    }
                }
            }
            _DbContext.SaveChanges();
            return existingDailyTimeEntry;
        }


        public async Task<DailyTimeEntry> CreateDailyTimeEntriesByManager(DailyTimeEntry dailyTimeEntry)
        {
            await _DbContext.DailyTimeEntries.AddAsync(dailyTimeEntry);
            await _DbContext.SaveChangesAsync();

            var existingemployee = await _DbContext.DailyTimeEntries.Where(x => x.EmployeeId == dailyTimeEntry.EmployeeId
                && x.Date.Date == dailyTimeEntry.Date.Date).FirstOrDefaultAsync();

            if (existingemployee == null)
            {
                return null;
            }

            if (existingemployee.CheckIn != null && existingemployee.CheckOut != null &&
                existingemployee.LunchIn != null && existingemployee.LunchOut != null)
            {
                TimeSpan WorkingDuration = existingemployee.CheckOut.Value - existingemployee.CheckIn.Value;
                TimeSpan LunchDuration = existingemployee.LunchOut.Value - existingemployee.LunchIn.Value;
                existingemployee.TotalDuration = (int)(WorkingDuration.TotalMinutes - LunchDuration.TotalMinutes);
                await _DbContext.SaveChangesAsync();

            }
            return dailyTimeEntry;
        }
    }
}
