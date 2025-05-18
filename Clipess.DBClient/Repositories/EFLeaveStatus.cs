using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;

namespace Clipess.DBClient.Repositories
{
    public class EFLeaveStateRepository : ILeaveStateRepository
    {
        public EFDbContext DbContext { get; set; }

        public EFLeaveStateRepository(EFDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public IQueryable<LeaveState> GetLeaveStatus()
        {
            return DbContext.LeaveStates;
        }
    }
}