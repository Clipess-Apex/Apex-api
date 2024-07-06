using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;

namespace Clipess.DBClient.Repositories
{
    public class EFRoleRepository : IRoleRepository
    {
        public EFDbContext DbContext{ get; set; }

        public EFRoleRepository(EFDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public IQueryable<Role> GetRoles()
        {
            return DbContext.Roles;
        }

        public async Task<Role> AddRoles(Role role)
        {
            DbContext.Roles.Add(role);
            await DbContext.SaveChangesAsync();
            return role;
        }

        public async Task<bool> UpdateRole(Role role)
        {
            var existingRole = await DbContext.Roles.FindAsync(role.RoleID);

            if (existingRole == null)
            {
                return false;
            }

            existingRole.RoleName = role.RoleName;
            DbContext.Roles.Update(existingRole);
            await DbContext.SaveChangesAsync();
            return true;
        }

        public async Task<Role> GetRoleById(int roleID)
        {
            try
            {
                var role = await DbContext.Roles.FindAsync(roleID);
                return role;
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                throw new Exception("Database operation failed", ex);
            }
        }
    }
}
