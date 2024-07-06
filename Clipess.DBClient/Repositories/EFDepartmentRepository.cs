using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;

namespace Clipess.DBClient.Repositories
{
    public class EFDepartmentRepository : IDepartmentRepository
    {
        public EFDbContext DbContext { get; set; }
        public EFDepartmentRepository(EFDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public IQueryable<Department> GetDepartments()
        {
            return DbContext.Departments;
        }

        public async Task<Department> AddDepartment(Department department)
        {
            DbContext.Departments.Add(department);
            await DbContext.SaveChangesAsync();
            return department;
        }

        public async Task<bool> UpdateDepartment(Department department)
        {
            var existingDepartment = await DbContext.Departments.FindAsync(department.DepartmentID);

            if (existingDepartment == null)
            {
                return false;
            }

            existingDepartment.DepartmentName = department.DepartmentName;
            DbContext.Departments.Update(existingDepartment);
            await DbContext.SaveChangesAsync();
            return true;
        }

        public async Task<Department> GetDepartmentById(int departmentID)
        {
            try
            {
                var department = await DbContext.Departments.FindAsync(departmentID);
                return department;
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                throw new Exception("Database operation failed", ex);
            }
        }
    }
}