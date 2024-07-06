using Clipess.DBClient.EntityModels;

namespace Clipess.DBClient.Contracts
{
    public interface ITimeEntryRepository
    {
        IQueryable<TimeEntryType> GetTimeEntryTypesByEmployee();
        IQueryable<TimeEntry> GetTimeEntriesByEmployee(int id, DateTime date);
        Task<TimeEntry> CreateTimeEntriesByEmployee(TimeEntry timeEntry);
        Task<DailyTimeEntry> CreateDailyTimeEntryCheckinByEmployee(int id, DateTime date);
        Task<DailyTimeEntry> UpdateDailyTimeEntryOtherTypesByEmployees(int id, DateTime date, int typeid);
        Task<TimeEntry> DeleteTimeEntryTasksByEmployee(int id);
        Task<TimeEntry> UpdateTimeEntryTasksByEmployee(int id, int duration, string description);
        Task<TimeEntry> DeleteTimeEntryEventsByEmployee(int timeEntryId, int employeeId, DateTime date, int typeId);
        IQueryable<DailyTimeEntry> GetDailyTimeEntriesByEmployee(int? id, DateTime startDate, DateTime endDate, string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000);
        IQueryable<MonthlyTimeEntry> GetMonthlyTimeEntriesByEmployee(int? employeeId, string year, string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000);
        IQueryable<TimeEntry> GetTimeEntryTasks();
        IQueryable<MonthlyTimeEntry> GetMonthlyTimeEntriesByManager(string startMonth, int? id, string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000);
        IQueryable<DailyTimeEntry> GetDailyTimeEntriesByManager(DateTime startDate, DateTime endDate, int? id, string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000);
        IQueryable<Employee> GetEmployeesByManager();
        Task<List<MonthlyTimeEntry>> CreateMonthlyTimeEntriesByManager(int allocatedTime, string workingMonth);
        Task<List<MonthlyTimeEntry>> UpdateMonthlyTimeEntriesByManager(int allocatedTime, string month);
        Task<List<MonthlyTimeEntry>> DeleteMonthlyTimeEntriesByManager(string month);
        IQueryable<MonthlyWorkingDay> GetMonthlyWorkingDays();
        Task<List<MonthlyWorkingDay>> CreateMonthlyWorkingDaysByManager(List<DateTime> selectedDates);
        Task<List<MonthlyWorkingDay>> UpdateMonthlyWorkingDaysByManager(List<DateTime> selectedDates);
        Task<List<MonthlyWorkingDay>> DeleteMonthlyWorkingDaysByManager(string month);
        Task<DailyTimeEntry> UpdateDailyTimeEntriesByManager(DailyTimeEntry dailyTimeEntry);
        IQueryable<MonthlyTimeEntry> GetMonthlyTimeEntryForPieChart(int employeeId,DateTime currentDate);
        IQueryable<MonthlyTimeEntry> GetMonthlyTimeEntryForBarChart(DateTime currentDate);
    }
}
