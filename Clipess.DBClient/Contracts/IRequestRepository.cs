

using Clipess.DBClient.EntityModels;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Clipess.DBClient.Contracts
{
    public interface IRequestRepository
    {
        IQueryable<Request> GetRequests();
        void AddRequest(Request request);
        public Request GetRequestById(int RequestId);
        public void AcceptRequest(Request request);
        public void DeleteRequest(Request request);
        public void ReadRequest(Request request);
        public void RejectRequest(Request request);
        IQueryable<Request> GetRequestsByEmployeeId(int? employeeId);
        public IQueryable<Request> GetNoOfUnreadRequests();
        public void UpdateEmployeeRequest(Request request);
        public IQueryable<Request> GetNoOfAllRequests();
        public IQueryable<Request> GetNoOfPendingRequests();
        public IQueryable<Request> GetNoOfRejectedRequests();
        public IQueryable<Request> GetNoOfAcceptedRequests();

        void SaveChanges();
    }
}
