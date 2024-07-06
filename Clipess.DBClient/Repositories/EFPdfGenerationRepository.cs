using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;

namespace Clipess.DBClient.Repositories
{
    public class EFPdfGenerationRepository : IPdfGenerationRepository
    {
        public EFDbContext _DbContext { get; set; }
        public EFPdfGenerationRepository(EFDbContext dbContext)
        {
            _DbContext = dbContext;
        }

        public IQueryable<MonthlyTimeEntry> GetMonthlyTimeEntries(int employeeId, string month)
        {
            var monthlyTimeEntries = _DbContext.MonthlyTimeEntries.Where(x => x.EmployeeId == employeeId && x.Month == month);
            if (monthlyTimeEntries == null)
            {
                return null;
            }

            var monthlyTimeEntriesWithUsers = monthlyTimeEntries.Select(x => new
            {
                MonthlyTimeEntry = x,
                Employee = _DbContext.Employees.FirstOrDefault(u => u.EmployeeID == x.EmployeeId)
            });

            var finalResults = monthlyTimeEntriesWithUsers.Select(x => new MonthlyTimeEntry
            {
                MonthlyTimeEntryId = x.MonthlyTimeEntry.MonthlyTimeEntryId,
                Month = x.MonthlyTimeEntry.Month,
                EmployeeId = x.MonthlyTimeEntry.EmployeeId,
                AllocatedDuration = x.MonthlyTimeEntry.AllocatedDuration,
                CompletedDuration = x.MonthlyTimeEntry.CompletedDuration,
                Employee = x.Employee // This will be null if there is no matching user
            });

            return finalResults.AsQueryable();
        }

        public IQueryable<DailyTimeEntry> GetDailyTimeEntries(int employeeId, string month)
        {
            DateTime currentMonthStart = DateTime.ParseExact(month, "yyyy-MM", null);
            DateTime nextMonthStart;

            if (currentMonthStart.Month == 12)
            {
                 nextMonthStart = new DateTime(currentMonthStart.Year + 1, 1, 1);
            }
            else
            {
                 nextMonthStart = currentMonthStart.AddMonths(1);
            }

            var dailyTimeEntries = _DbContext.DailyTimeEntries.Where(x => x.EmployeeId == employeeId && x.Date >= currentMonthStart && x.Date < nextMonthStart);

            if(dailyTimeEntries == null)
            {
                return null;
            }

            return dailyTimeEntries;
        }

        public List<int> GetEmployee()
        {
            var employees = _DbContext.Employees.Where( x => x.Deleted == false ).Select(u => u.EmployeeID).ToList();

            return employees;
        }

        public async Task<MonthlyTimeEntry> SaveAttendancePdf(int employeeId, string month, string Url)
        {
            var monthlyTimeEntry = _DbContext.MonthlyTimeEntries.FirstOrDefault(x => x.EmployeeId == employeeId && x.Month == month);

            if(monthlyTimeEntry == null)
            {
                return null;
            }

            monthlyTimeEntry.AttendancePdfUrl = Url;

            _DbContext.SaveChanges();
            return monthlyTimeEntry;
        }

        public IQueryable<MonthlyTimeEntry> GetMonthlyPdfByEmployee(int employeeId, string month)
        {
            var monthlyTimeEntry = _DbContext.MonthlyTimeEntries.Where(x => x.EmployeeId == employeeId && x.Month == month);

            if(monthlyTimeEntry == null)
            {
                return null;
            }

            return monthlyTimeEntry;
        }
    }
}
