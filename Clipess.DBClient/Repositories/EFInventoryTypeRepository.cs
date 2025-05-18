using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;
using Microsoft.EntityFrameworkCore;

namespace Clipess.DBClient.Repositories
{
    public class EFInventoryTypeRepository : IInventoryTypeRepository
    {
        public EFDbContext DbContext { get; set;}

        public EFInventoryTypeRepository(EFDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public IQueryable<InventoryType> GetInventoryTypes()
        {
            return DbContext.InventoryTypes;
        }

        public void AddInventoryType(InventoryType inventoryType)
        {
            DbContext.InventoryTypes.Add(inventoryType);
        }

        public void UpdateInventoryType(InventoryType inventoryType)
        {
            DbContext.Entry(inventoryType).State = EntityState.Modified;
        }

        public InventoryType GetInventoryTypeById(int InventoryTypeId)
        {
            return DbContext.InventoryTypes.FirstOrDefault(i => i.InventoryTypeId == InventoryTypeId);
        }

        public void DeleteInventoryType(InventoryType inventoryType)
        {
            DbContext.InventoryTypes.Update(inventoryType);
        }

        public void UpdateInventoryTypeIdWhenDeleted(int inventoryTypeId)
        {
            var inventories = DbContext.Inventories.Where(i => i.InventoryTypeId == inventoryTypeId).ToList();
            foreach (var inventory in inventories)
            {
                inventory.InventoryTypeId = 0;
            }
            DbContext.SaveChanges();
        }

        public IQueryable<InventoryType> GetTotalInventoryTypes()
        {
            return DbContext.InventoryTypes;
        }


        public void SaveChanges()
        {
            DbContext.SaveChanges();
        }
    }
}