using Clipess.DBClient.EntityModels;

namespace Clipess.DBClient.Contracts
{
    public interface IMaritalStatusRepository
    {
        IQueryable<MaritalStatus> GetMaritalStatus();
        Task<MaritalStatus> AddMaritalStatus(MaritalStatus maritalStatus);
    }
}
