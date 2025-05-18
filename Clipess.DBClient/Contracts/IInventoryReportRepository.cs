using Clipess.DBClient.EntityModels;
using System.Linq;

namespace Clipess.DBClient.Contracts
{
    public interface IInventoryReportRepository
    {
        IQueryable<Inventory> GetInventoriesByType(int inventoryTypeId);
        InventoryType GetInventoryTypeById(int inventoryTypeId);
        void SaveChanges();
    }
}
