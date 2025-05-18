
using Clipess.DBClient.EntityModels;
using System.Linq;
using System.Threading.Tasks;

namespace Clipess.DBClient.Contracts
{
    public interface ILeaveRepository
    {
        IQueryable<Leave> GetLeaves();
        Task<List<Leave>> GetLeavesByStatusId(int statusId);
        Task<IEnumerable<Leave>> GetLeavesByEmployeeId(int statusId, int employeeId);
        Task<Leave> AddLeaves(Leave leave);
        Task<Leave> GetLeaveById(int leaveId);
        Task<int> GetLeaveCountByEmployeeAndType(int employeeId, int leaveTypeId, DateTime startOfMonth, DateTime endOfMonth);
        Task<bool> UpdateLeave(Leave leave);
        Task<bool> DeleteLeave(int leaveId);
        //bool AcceptLeave(int leaveId);


    }
}


