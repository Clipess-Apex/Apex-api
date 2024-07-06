using Clipess.DBClient.Contracts;

namespace Clipess.DBClient.Repositories
{
    public class EFSampleRepository : ISampleRepository
    {
        public EFDbContext DbContext { get; set; }

        public EFSampleRepository(EFDbContext dbContext)
        {
            DbContext = dbContext;
        }
    }
}
