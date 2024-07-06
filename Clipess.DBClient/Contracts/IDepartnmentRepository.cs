using Clipess.DBClient.EntityModels;

namespace Clipess.DBClient.Contracts
{
    public interface IDepartmentRepository
    {
        IQueryable<Department> GetDepartments();
        Task<Department> AddDepartment(Department department);
        //Task<Department> GetDepartment(int id);
        Task<bool> UpdateDepartment(Department department);
        Task<Department> GetDepartmentById(int departmentID);
    }
}
