using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Clipess.DBClient.Repositories
{
    public class EFInventoryReportRepository : IInventoryReportRepository
    {
        private readonly EFDbContext DbContext;

        public EFInventoryReportRepository(EFDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public IQueryable<Inventory> GetInventoriesByType(int inventoryTypeId)
        {
            return DbContext.Inventories.Where(u => u.InventoryTypeId == inventoryTypeId)
                .Include(i => i.Employee);
                
        }

        public InventoryType GetInventoryTypeById(int inventoryTypeId)
        {
            return DbContext.InventoryTypes.FirstOrDefault(it => it.InventoryTypeId == inventoryTypeId);
        }

        public void SaveChanges()
        {
            DbContext.SaveChanges();
        }
    }
}
