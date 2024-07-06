
using Clipess.DBClient.EntityModels;
using Microsoft.EntityFrameworkCore;

namespace Clipess.DBClient.Contracts
{
    public interface IInventoryTypeRepository

    {
        IQueryable<InventoryType> GetInventoryTypes();

        void AddInventoryType(InventoryType inventoryType);

        public void UpdateInventoryType(InventoryType inventoryType);

        public InventoryType GetInventoryTypeById(int InventoryTypeId);

        public void DeleteInventoryType(InventoryType inventoryType);

        void UpdateInventoryTypeIdWhenDeleted(int inventoryTypeId);

        public IQueryable<InventoryType> GetTotalInventoryTypes();




        void SaveChanges();



    }
}
