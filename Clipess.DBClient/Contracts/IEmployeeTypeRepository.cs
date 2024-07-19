using Clipess.DBClient.EntityModels;

namespace Clipess.DBClient.Contracts
{
    public interface IEmployeeTypeRepository
    {
        IQueryable<EmployeeType> GetEmployeeTypes();
        Task<EmployeeType> AddEmployeeTypes(EmployeeType employeeType);
        Task<bool> UpdateEmployeeType(EmployeeType employeeType);
        Task<EmployeeType> GetEmployeeTypeById(int employeeTypeID);
    }
}
