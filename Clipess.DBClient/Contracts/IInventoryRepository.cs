
using Clipess.DBClient.EntityModels;
using Clipess.DBClient.Repositories;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Clipess.DBClient.Contracts
{
    public interface IInventoryRepository
    {
        IQueryable<Inventory> GetInventories();

        void AddInventory(Inventory inventory);

        IQueryable<Inventory> GetInventoryByEmployeeAndType(int? employeeId, int? typeId);


        public void UpdateInventory(Inventory inventory);

        public void DeleteInventory(Inventory inventory);

        public Inventory GetInventoryById(int inventoryId);

        public IQueryable<Inventory> GetUnassignedInventoryByType(int employeeId, int typeId);

        public IEnumerable<Inventory> GetInventoryAssignData();

        public IEnumerable<Inventory> GetNoOfInventoryByType();

        public IQueryable <Inventory> GetTotalNoOfInventories();

        public IQueryable<Inventory> GetNoOfInventoriesByType();

        void SaveChanges();

        
    }
}




