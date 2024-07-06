
using Clipess.DBClient.EntityModels;

namespace Clipess.DBClient.Contracts
{
    public interface ILeaveStateRepository
    {
        IQueryable<LeaveState> GetLeaveStatus();

    }
}
