using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;

namespace Clipess.DBClient.Repositories
{
    public class EFMaritalStatusRepository : IMaritalStatusRepository
    {
        public EFDbContext DbContext { get; set; }

        public EFMaritalStatusRepository(EFDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public IQueryable<MaritalStatus> GetMaritalStatus()
        {
            return DbContext.MaritalStatus;
        }

        public async Task<MaritalStatus> AddMaritalStatus(MaritalStatus maritalStatus)
        {
            DbContext.MaritalStatus.Add(maritalStatus);
            await DbContext.SaveChangesAsync();
            return maritalStatus;
        }
    }
}
