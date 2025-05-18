using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clipess.DBClient.Repositories
{
    public class EFEmployeeInventoryRepository: IEmployeeInventoryRepository
    {
        public EFDbContext DbContext { get ; set; }

        public EFEmployeeInventoryRepository(EFDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public IQueryable<Employee> GetEmployees()
        {
            return DbContext.Employees;
        }
    }
}
