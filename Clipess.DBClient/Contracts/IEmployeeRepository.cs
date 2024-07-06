using Clipess.DBClient.EntityModels;

namespace Clipess.DBClient.Contracts
{
    public interface IEmployeeRepository
    {
        IQueryable<Employee> GetEmployees();
        Task<Employee> AddEmployee(Employee employee);
        Task<Employee> GetEmployeeWithDepartment();
        Task GetAllEmployeesWithDepartment();
        Task<Employee> GetEmployee(int id);
        Task<bool> UpdateEmployee(Employee employee);
        Task<Employee> GetEmployeeById(int employeeID);
        public void SaveChangesAsync();
        Task<Employee> GetEmployeeByNIC(string nic);
        Task<Employee> GetEmployeeByEmail(string companyEmail);
        Task<bool> ResetPassword(Employee user, string newPassword);
        Task<EmployeeCountsDto> GetEmployeeCountsAsync();
    }
}