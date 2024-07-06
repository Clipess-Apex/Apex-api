
using Clipess.DBClient.EntityModels;

namespace Clipess.DBClient.Contracts
{
    public interface IUserRepository
    {
        IQueryable<User>GetUsers();
        
    }
}
