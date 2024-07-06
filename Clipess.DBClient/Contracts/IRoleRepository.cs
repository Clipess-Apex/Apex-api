using Clipess.DBClient.EntityModels;

namespace Clipess.DBClient.Contracts
{
    public interface IRoleRepository
    {
        IQueryable<Role> GetRoles();
        Task<Role> AddRoles(Role role);
        Task<bool> UpdateRole(Role role);
        Task<Role> GetRoleById(int roleID);
    }
}
