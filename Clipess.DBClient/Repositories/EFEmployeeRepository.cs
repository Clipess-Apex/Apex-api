using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clipess.DBClient.Repositories
{
    public class EFEmployeeRepository : IEmployeeRepository
    {
        public EFDbContext DbContext { get; set; }
        public EFEmployeeRepository(EFDbContext dbContext)
        {
            DbContext = dbContext;
        }
        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await DbContext.Employees.ToListAsync();
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            return await DbContext.Employees.FindAsync(id);
        }

        public async Task<Employee> AddEmployeeAsync(Employee employee)
        {
            DbContext.Employees.Add(employee);
            await DbContext.SaveChangesAsync();
            return employee;
        }

        public async Task<Employee> UpdateEmployeeAsync(Employee employee)
        {
            DbContext.Entry(employee).State = EntityState.Modified;
            await DbContext.SaveChangesAsync();
            return employee;
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            var employee = await DbContext.Employees.FindAsync(id);
            if (employee == null)
            {
                return false;
            }

            DbContext.Employees.Remove(employee);
            await DbContext.SaveChangesAsync();
            return true;
        }
    }
}
