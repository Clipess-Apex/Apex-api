/*using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;
using Microsoft.EntityFrameworkCore;








namespace Clipess.DBClient.Repositories
{
    public class EFInventoryRepository : IInventoryRepository
    {
        public EFDbContext DbContext { get; set; }

        public EFInventoryRepository(EFDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public IQueryable<Inventory> GetInventories()
        {
            return DbContext.Inventories;
        }
        

       
        public void AddInventory(Inventory inventory)
        {
            DbContext.Inventories.Add(inventory);
        }


    

        public IQueryable<Inventory> GetInventoryByEmployeeOrType(int? employeeId, int? typeId)
        {
            IQueryable<Inventory> query = DbContext.Inventories;

            if (employeeId.HasValue)
            {
                query = query.Where(inventory => inventory.EmployeeId == employeeId);
            }

            if (typeId.HasValue)
            {
                query = query.Where(inventory => inventory.InventoryTypeId == typeId);
            }

            return query;
        }



        public void SaveChanges()
        {
            DbContext.SaveChanges();
        }


    }
}*/

using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;
using Clipess.DBClient.Repositories;

using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Clipess.DBClient.Repositories
{
    public class EFInventoryRepository : IInventoryRepository
    {
        public EFDbContext DbContext { get; set; }
        

      

        public EFInventoryRepository(EFDbContext dbContext)
        {
            DbContext = dbContext;
        }

        

        public IQueryable<Inventory> GetInventories()
        {
            return DbContext.Inventories
                .Include(i => i.Employee)
                .Include(i => i.InventoryType);
        }

       

        public void AddInventory(Inventory inventory)
        {
            DbContext.Inventories.Add(inventory);
        }

        public IQueryable<Inventory> GetInventoryByEmployeeAndType(int? employeeId, int? typeId)
        {
            IQueryable<Inventory> query = DbContext.Inventories;

            if (employeeId.HasValue)
            {
                query = query.Where(inventory => inventory.EmployeeId == employeeId);
            }

            if (typeId.HasValue)
            {
                query = query.Where(inventory => inventory.InventoryTypeId == typeId);
            }

            return query;
        }

        public void UpdateInventory(Inventory inventory)
        {
            DbContext.Entry(inventory).State = EntityState.Modified;
        }

        public void DeleteInventory(Inventory inventory)
        {
            DbContext.Inventories.Update(inventory);
        }

        public Inventory GetInventoryById(int inventoryId)
        {
            return DbContext.Inventories.FirstOrDefault(i => i.InventoryId == inventoryId);
        }

        public IQueryable<Inventory> GetUnassignedInventoryByType(int employeeId, int typeId)
        {
            return DbContext.Inventories;
        }
        public IEnumerable<Inventory> GetInventoryAssignData() // Return IEnumerable for potentially multiple items
        {
            return DbContext.Inventories;
            // Load all matching inventories into memory for further processing
        }

        public IEnumerable<Inventory> GetNoOfInventoryByType()
        {
            return DbContext.Inventories;
        }

        public IQueryable<Inventory> GetTotalNoOfInventories()
        {
            return DbContext.Inventories;
        }

        public IQueryable<Inventory> GetNoOfInventoriesByType()
        {
            return DbContext.Inventories;
        }



        public void SaveChanges()
        {
            DbContext.SaveChanges();
        }

       
        
    }
}


