using Clipess.DBClient.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clipess.DBClient.Contracts
{
    public  interface IEmployeeInventoryRepository
    {
        IQueryable<Employee> GetEmployees();

       

    }
}
