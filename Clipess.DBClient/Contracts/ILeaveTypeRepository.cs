
using Clipess.DBClient.EntityModels;

namespace Clipess.DBClient.Contracts
{
    public interface ILeaveTypeRepository
    {
        IQueryable<LeaveType> GetLeaveTypes();
        Task<LeaveType> AddLeaveTypes(LeaveType leaveType);
        Task<LeaveType> GetLeaveTypeById(int leaveTypeId);
        Task<bool> UpdateLeaveTypes(LeaveType leaveType);
        Task<List<LeaveType>> GetLeaveTypeByName(string leaveTypeName);
    }

}