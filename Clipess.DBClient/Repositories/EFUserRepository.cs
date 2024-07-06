using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;

namespace Clipess.DBClient.Repositories
{
    public class EFUserRepository : IUserRepository
    {
        public EFDbContext DbContext { get; set; }

        public EFUserRepository(EFDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public IQueryable<User> GetUsers()
        {
            return DbContext.Users;
        }
    }
}
