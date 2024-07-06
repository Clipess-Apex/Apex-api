using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;
using Microsoft.EntityFrameworkCore;

namespace Clipess.DBClient.Repositories
{
    public class EFLeaveTypeRepository : ILeaveTypeRepository
    {
        public EFDbContext DbContext { get; set; }
        public EFLeaveTypeRepository(EFDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public IQueryable<LeaveType> GetLeaveTypes()
        {
            return DbContext.LeaveTypes;
        }
        public async Task<LeaveType> AddLeaveTypes(LeaveType leaveType)
        {
            DbContext.LeaveTypes.Add(leaveType);
            DbContext.SaveChanges();
            return leaveType;
        }
        
        public async Task<LeaveType> GetLeaveTypeById(int leaveTypeId)
        {
            return await DbContext.LeaveTypes.FirstOrDefaultAsync(l => l.LeaveTypeId == leaveTypeId);
        }

        public async Task<List<LeaveType>> GetLeaveTypeByName(string leaveTypeName)
        {
            return await DbContext.LeaveTypes
                .Where(l=>l.LeaveTypeName == leaveTypeName)
                .ToListAsync();
        }

        public async Task<bool> UpdateLeaveTypes(LeaveType leaveType)
        {
            DbContext.Entry(leaveType).State = EntityState.Modified;
            try
            {
                await DbContext.SaveChangesAsync();
                return true;
            }
            catch(DbUpdateConcurrencyException)
            {
                return false;
            }
        }
    }
}