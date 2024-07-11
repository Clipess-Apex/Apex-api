using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
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

        public IQueryable<Employee> GetEmployees()
        {
            return DbContext.Employees;
        }

        public async Task<Employee> AddEmployee(Employee employee)
        {
            await DbContext.Employees.AddAsync(employee);
            return employee;
        }

        public async Task<Employee> GetEmployeeWithDepartment()
        {
            throw new NotImplementedException();
        }

        public Task GetAllEmployeesWithDepartment()
        {
            throw new NotImplementedException();
        }

        public async Task<Employee> GetEmployee(int id)
        {
            return await DbContext.Employees.FindAsync(id);
        }

        public async Task<bool> UpdateEmployee(Employee employee)
        {
            var existingEmployee = await DbContext.Employees.FindAsync(employee.EmployeeID);
            if (existingEmployee == null)
            {
                return false;
            }

            // Update properties
            existingEmployee.FirstName = employee.FirstName;
            existingEmployee.LastName = employee.LastName;
            existingEmployee.DateOfBirth = employee.DateOfBirth;
            existingEmployee.NIC = employee.NIC;
            existingEmployee.PersonalEmail = employee.PersonalEmail;
            existingEmployee.CompanyEmail = employee.CompanyEmail;
            existingEmployee.MobileNumber = employee.MobileNumber;
            existingEmployee.TelephoneNumber = employee.TelephoneNumber;
            existingEmployee.Address = employee.Address;
            existingEmployee.Designation = employee.Designation;
            existingEmployee.ImageUrl = employee.ImageUrl;
            existingEmployee.FileUrl = employee.FileUrl;
            existingEmployee.EmergencyContactPersonName = employee.EmergencyContactPersonName;
            existingEmployee.EmergencyContactPersonMobileNumber = employee.EmergencyContactPersonMobileNumber;
            existingEmployee.EmergencyContactPersonAddress = employee.EmergencyContactPersonAddress;
            existingEmployee.ReportingEmployeeID = employee.ReportingEmployeeID;
            existingEmployee.EmployeeTypeID = employee.EmployeeTypeID;
            existingEmployee.RoleID = employee.RoleID;
            existingEmployee.DepartmentID = employee.DepartmentID;
            existingEmployee.MaritalStatusID = employee.MaritalStatusID;          

            DbContext.Employees.Update(existingEmployee);
            await DbContext.SaveChangesAsync();
            return true;
        }

        public async Task<Employee> GetEmployeeById(int employeeID)
        {
            try
            {
                var employee = await DbContext.Employees.FindAsync(employeeID);
                return employee;
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                throw new Exception("Database operation failed", ex);
            }
        }

        public void SaveChangesAsync()
        {
            DbContext.SaveChanges();
        }

        public async Task<Employee> GetEmployeeByNIC(string nic)
        {
            return await DbContext.Employees.SingleOrDefaultAsync(e => e.NIC == nic);
        }

        public async Task<Employee> GetEmployeeByEmail(string companyEmail)
        {
            return await DbContext.Employees.SingleOrDefaultAsync(e => e.CompanyEmail == companyEmail);
        }

        async Task<bool> IEmployeeRepository.ResetPassword(Employee user, string newPassword)
        {
            var existingEmployee = await DbContext.Employees.FindAsync(user.EmployeeID);
            if (existingEmployee == null)
            {
                return false;
            }

            // Update the password
            existingEmployee.Password = newPassword; // Assuming you have a Password property

            DbContext.Employees.Update(existingEmployee);
            await DbContext.SaveChangesAsync();
            return true;
        }

        public async Task<EmployeeCountsDto> GetEmployeeCountsAsync()
        {
            var total = await DbContext.Employees.Where (e => e.Deleted == false) .CountAsync();

            var roleCounts = await DbContext.Roles
                .Where(r => !r.Deleted)
                .Select(r => new
                {
                    r.RoleName,
                    Count = DbContext.Employees.Count(e => e.RoleID == r.RoleID && e.Deleted == false)
                })
                .ToListAsync();

            var roleCountsDict = roleCounts.ToDictionary(r => r.RoleName, r => r.Count);

            return new EmployeeCountsDto
            {
                Total = total,
                RoleCounts = roleCountsDict
            };
        }

        public async Task<DepartmentEmployeeCountDto> GetEmployeeCountByDepartmentAsync()
        {
            var total = await DbContext.Employees.Where(e => e.Deleted == false).CountAsync();

            var departmentCounts = await DbContext.Departments
                .Where(d => !d.Deleted)
                .Select(d => new
                {
                    d.DepartmentName,
                    Count = DbContext.Employees.Count(e => e.DepartmentID == d.DepartmentID && e.Deleted == false)
                })
                .ToListAsync();

            var departmentCountsDict = departmentCounts.ToDictionary(d => d.DepartmentName, d => d.Count);

            return new DepartmentEmployeeCountDto
            {
                DepartmentEmployeeCountTotal = total,
                DepartmentCounts = departmentCountsDict
            };
        }

        public async Task<List<int>> GetAllManagerIds()
        {
            return await DbContext.Employees
                .Where(e => e.role.RoleName == "Manager" && e.Deleted == false)
                .Select(e => e.EmployeeID)
                .ToListAsync();
        }

        public async Task<string> GetEmployeeNameById(int employeeId)
        {
            var employee = await DbContext.Employees.FindAsync(employeeId);
            return employee != null ? $"{employee.FirstName} {employee.LastName}" : null;
        }

    }
}
 
