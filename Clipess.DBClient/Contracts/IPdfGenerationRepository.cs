using Clipess.DBClient.EntityModels;

namespace Clipess.DBClient.Contracts
{
    public interface IPdfGenerationRepository
    {
        IQueryable<MonthlyTimeEntry> GetMonthlyTimeEntries(int employeeId, string month);
        IQueryable<DailyTimeEntry> GetDailyTimeEntries(int employeeId, string month);
        List<int> GetEmployee();
        Task<MonthlyTimeEntry> SaveAttendancePdf(int  employeeId, string month,string Url);
        IQueryable<MonthlyTimeEntry> GetMonthlyPdfByEmployee(int employeeId, string month);
    }
}
