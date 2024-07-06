using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Clipess.DBClient.Repositories
{
    public class EFLeaveRepository : ILeaveRepository
    {
        public EFDbContext DbContext { get; set; }

        public EFLeaveRepository(EFDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public IQueryable<Leave> GetLeaves()
        {
            return DbContext.Leaves;
        }

        public async Task<IEnumerable<Leave>> GetLeavesByEmployeeId(int statusId, int employeeId)
        {
            return await DbContext.Leaves
                .Where(l => l.StatusId == statusId && l.EmployeeId == employeeId)
                .Include(l => l.LeaveType)
                .ToListAsync();
        }
        public async Task<List<Leave>> GetLeavesByStatusId(int statusId )
        {
            return await DbContext.Leaves
                .Include(l => l.LeaveType)
                .Include(l => l.employee)
                .Where(l => l.StatusId == statusId)
                .ToListAsync();
        }

        public async Task<Leave> AddLeaves(Leave leave)
        {
            DbContext.Leaves.Add(leave);
           await DbContext.SaveChangesAsync();
            return leave;
        }
        public async Task<Leave> GetLeaveById(int leaveId)
        {
            return await DbContext.Leaves
                .Include(l => l.LeaveType)
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.LeaveId == leaveId);
        }

        //public async Task<int> GetLeaveCountByEmployeeAndType(int employeeId, int leaveTypeId)
        //{
        //    return await DbContext.Leaves
        //        .Where(leave => leave.EmployeeId == employeeId &&
        //        leave.LeaveTypeId == leaveTypeId &&
        //        leave.StatusId != 3)
        //        .CountAsync();
        //}

        public async Task<int> GetLeaveCountByEmployeeAndType(int employeeId, int leaveTypeId, DateTime startOfMonth, DateTime endOfMonth)
        {
            return await DbContext.Leaves
                .Where(leave => leave.EmployeeId == employeeId &&
                 leave.LeaveTypeId == leaveTypeId &&
                                leave.StatusId != 3 &&
                                leave.LeaveDate >= startOfMonth &&
                                leave.LeaveDate <= endOfMonth)
                .CountAsync();
        }
        public async Task<bool> UpdateLeave(Leave leave)
        {
            try
            {
                DbContext.Entry(leave).State = EntityState.Modified;
                await DbContext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteLeave(int leaveId)
        {
            var leave = await DbContext.Leaves.FirstOrDefaultAsync(leave => leave.LeaveId == leaveId);
            if(leave == null) return false;

            DbContext.Leaves.Remove(leave);
            await DbContext.SaveChangesAsync();
            return true;
        }

        public void Dispose()
        {
            DbContext.Dispose();
        }

        /*public bool AcceptLeave(int leaveId)
        {
            var dbEntry = DbContext.Leaves.FirstOrDefault(x => x.LeaveId == leaveId);
            if (dbEntry != null) 
            {
                dbEntry.StatusId = 2;
                dbEntry.ConsideredDate = DateTime.UtcNow;
            }
            var rowAffected = DbContext.SaveChanges();
            return rowAffected > 0;            
        }*/
}

}
