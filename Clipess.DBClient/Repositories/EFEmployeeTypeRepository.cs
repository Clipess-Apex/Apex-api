using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;

namespace Clipess.DBClient.Repositories
{
    public class EFEmployeeTypeRepository : IEmployeeTypeRepository
    {
        public EFDbContext DbContext { get; set; }

        public EFEmployeeTypeRepository(EFDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public IQueryable<EmployeeType> GetEmployeeTypes()
        {
            return DbContext.EmployeeTypes;
        }

        public async Task<EmployeeType> AddEmployeeTypes(EmployeeType employeeType)
        {
            DbContext.EmployeeTypes.Add(employeeType);
            await DbContext.SaveChangesAsync();
            return employeeType;
        }

        public async Task<bool> UpdateEmployeeType(EmployeeType employeeType)
        {
            var existingEmployeeType = await DbContext.EmployeeTypes.FindAsync(employeeType.EmployeeTypeID);

            if (existingEmployeeType == null)
            {
                return false;
            }

            existingEmployeeType.EmployeeTypeName = employeeType.EmployeeTypeName;
            DbContext.EmployeeTypes.Update(existingEmployeeType);
            await DbContext.SaveChangesAsync();
            return true;
        }

        public async Task<EmployeeType> GetEmployeeTypeById(int employeeTypeID)
        {
            try
            {
                var employeeType = await DbContext.EmployeeTypes.FindAsync(employeeTypeID);
                return employeeType;
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                throw new Exception("Database operation failed", ex);
            }
        }
    }
}