using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Clipess.DBClient.Repositories
{
    public class EFRequestRepository : IRequestRepository
    {
        public EFDbContext DbContext { get; set; }

        public EFRequestRepository(EFDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public IQueryable<Request> GetRequests()
        {
            return DbContext.Requests
                .Include(i => i.InventoryType)
                .Include(i => i.Employee);
        }

        public void AddRequest(Request request)
        {
            DbContext.Requests.Add(request);
        }

        public Request GetRequestById(int requestId)
        {
            return DbContext.Requests
               .Include(i => i.InventoryType)
               .Include(i => i.Employee)
               .FirstOrDefault(i => i.RequestId == requestId);
        }

        public void AcceptRequest(Request request)
        {
            DbContext.Entry(request).State = EntityState.Modified;
            var inventory = DbContext.Inventories.FirstOrDefault(i => i.InventoryId == request.InventoryId);

            if (inventory != null)
            {
                // Update the employeeId in the inventory based on the request
                inventory.EmployeeId = request.EmployeeId;
                inventory.AssignedDate = DateTime.Now;
            }
        }

        public void DeleteRequest(Request request)
        {
            DbContext.Requests.Update(request);
        }

        public void ReadRequest(Request request)
        {
            DbContext.Requests.Update(request);
        }

        public void RejectRequest(Request request)
        {
            DbContext.Requests.Update(request);
        }

        public IQueryable<Request> GetRequestsByEmployeeId(int? employeeId)
        {
            IQueryable<Request> query = DbContext.Requests.Include(i => i.InventoryType);

            if (employeeId.HasValue)
            {
                query = query.Where(request => request.EmployeeId == employeeId);
            }

            return query;
        }

        public IQueryable<Request> GetNoOfUnreadRequests()
        {
            return DbContext.Requests;
        }

        public void UpdateEmployeeRequest(Request request)
        {
            DbContext.Entry(request).State = EntityState.Modified;
        }


        public IQueryable<Request> GetNoOfAcceptedRequests(){

            return DbContext.Requests;
        }
        public IQueryable<Request> GetNoOfRejectedRequests()
        {

            return DbContext.Requests;
        }
        public IQueryable<Request> GetNoOfPendingRequests()
        {

            return DbContext.Requests;
        }

        public IQueryable<Request> GetNoOfAllRequests()
        {

            return DbContext.Requests;
        }




        public void SaveChanges()
        {
            DbContext.SaveChanges();
        }
    }
}
