using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;
using Clipess.DBClient.Repositories;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection;
using static Clipess.API.Controllers.InventoryController;

/*namespace Clipess.API.Controllers
{
    [Route("api/inventory_type")]
    [ApiController]
    public class Inventory_TypeController : ControllerBase
    {
        private readonly IInventory_TypeRepository _inventory_typeRepository;
        private static ILog _logger;

        public Inventory_TypeController(IInventory_TypeRepository inventory_typeRepository)
        {
            _inventory_typeRepository = inventory_typeRepository;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }

        [Route("inventory_types")]
        [HttpGet]
        public ActionResult GetInventory_Types()
        {
            try
            {
                var inventoryTypes = _inventory_typeRepository.GetInventory_Types().Where(x => !x.Deleted).ToList();
                if (inventoryTypes != null && inventoryTypes.Any())
                {
                    return Ok(inventoryTypes);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in: {nameof(GetInventory_Types)}, exception: {ex.Message}.");
                return BadRequest();
            }
        }


        [Route("add")]
        [HttpPost]
        public async Task<IActionResult> AddInventoryType([FromBody] Inventory_Type inventoryTypeName1)
        {
            try
            {
                if (inventoryTypeName1 == null)
                {
                    return BadRequest("Inventory type name is missing.");
                }

                // Create a new Inventory_Type
                var inventoryType = new Inventory_Type
                {
                    InventoryType = inventoryTypeName1.InventoryType,
                    InventoryTypeId = inventoryTypeName1.InventoryTypeId,
                    CreatedDate= DateTime.UtcNow,
                    CreatedBy= inventoryTypeName1.CreatedBy,
                    Deleted= inventoryTypeName1.Deleted,
                   
                };
                if (inventoryTypeName1.Deleted)
                {
                    inventoryType.DeletedDate = DateTime.UtcNow;
                    inventoryType.DeletedBy = inventoryTypeName1.DeletedBy;
                }
                // Add the newly created Inventory_Type
                _inventory_typeRepository.AddInventory_Type(inventoryType);
                // Save changes to the database
                _inventory_typeRepository.SaveChanges();

                // Return a response indicating successful creation
                
                return CreatedAtAction(nameof(GetInventory_Types), new { inventoryTypeId = inventoryType.InventoryTypeId }, inventoryType);
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.Error($"An error occurred in: {nameof(AddInventoryType)}, exception: {ex.Message}.");

                // Return a response indicating failure
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [Route("update/{inventoryTypeId}")]
        [HttpPut]
        public async Task<IActionResult> UpdateInventoryType(int inventoryTypeId, [FromBody] Inventory_Type inventoryTypeUpdate)
        {
            try
            {
                // Retrieve the existing inventory type from the repository
                var existingInventoryType = _inventory_typeRepository.GetInventoryTypeById(inventoryTypeId);

                if (existingInventoryType == null)
                {
                    return NotFound("Inventory type not found.");
                }

                // Update the properties of the existing inventory type
                existingInventoryType.InventoryType = inventoryTypeUpdate.InventoryType;
              
                
                existingInventoryType.Deleted = inventoryTypeUpdate.Deleted;

                // If the inventory type is marked as deleted, update deleted date and deleted by
                if (existingInventoryType.Deleted)
                {
                    existingInventoryType.DeletedDate = DateTime.UtcNow;
                    existingInventoryType.DeletedBy = inventoryTypeUpdate.DeletedBy;
                }
                else
                {
                    // If the inventory type is no longer deleted, clear deleted date and deleted by
                    existingInventoryType.DeletedDate = null;
                    existingInventoryType.DeletedBy = null;
                }

                // Update the existing inventory type in the repository
                _inventory_typeRepository.UpdateInventory_Type(existingInventoryType);
                // Save changes to the database
                _inventory_typeRepository.SaveChanges();

                // Return a response indicating successful update
                return Ok(existingInventoryType);
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.Error($"An error occurred in: {nameof(UpdateInventoryType)}, exception: {ex.Message}.");

                // Return a response indicating failure
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPut("delete/{inventoryTypeId}")]
        public async Task<IActionResult> DeleteInventory_Type(int inventoryTypeId, [FromBody] Inventory_Type inventoryTypeDelete)
        {
            try
            {
                // Retrieve the existing inventory type from the repository
                var existingInventoryType = _inventory_typeRepository.GetInventoryTypeById(inventoryTypeId);

                if (existingInventoryType == null)
                {
                    return NotFound("Inventory type not found.");
                }
                existingInventoryType.InventoryType = inventoryTypeDelete.InventoryType;

                existingInventoryType.Deleted = inventoryTypeDelete.Deleted;


                // If the inventory type is marked as deleted, update deleted date and deleted by
                if (existingInventoryType.Deleted)
                {
                    existingInventoryType.DeletedDate = DateTime.UtcNow;
                    existingInventoryType.DeletedBy = inventoryTypeDelete.DeletedBy;
                }
                else
                {
                    // If the inventory type is no longer deleted, clear deleted date and deleted by
                    existingInventoryType.DeletedDate = null;
                    existingInventoryType.DeletedBy = null;
                }

                // Update the existing inventory type in the repository
                _inventory_typeRepository.DeleteInventory_Type(existingInventoryType);
                // Save changes to the database
                _inventory_typeRepository.SaveChanges();

                // Return a response indicating successful update
                return Ok(existingInventoryType);
            }
            
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in {nameof(DeleteInventory_Type)}, exception: {ex.Message}.");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }





    }
}
*/
namespace Clipess.API.Controllers
{
    [Route("api/inventory_type")]
    [ApiController]
    public class InventoryTypeController : ControllerBase
    {
        private readonly IInventoryTypeRepository _inventoryTypeRepository;
        private static ILog _logger;

        public InventoryTypeController(IInventoryTypeRepository inventory_typeRepository)
        {
            _inventoryTypeRepository = inventory_typeRepository;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }

        [Route("inventory_types")]
        [HttpGet]
        public ActionResult GetInventoryTypes()
        {
            try
            {
                var inventoryTypes = _inventoryTypeRepository.GetInventoryTypes().Where(x => !x.Deleted).ToList();
                if (inventoryTypes != null && inventoryTypes.Any())
                {
                    return Ok(inventoryTypes);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in: {nameof(GetInventoryTypes)}, exception: {ex.Message}.");
                return BadRequest();
            }
        }

        [Route("add")]
        [HttpPost]
        public async Task<IActionResult> AddInventoryType([FromBody] InventoryType inventoryTypeName1)
        {
            try
            {
                if (inventoryTypeName1 == null)
                {
                    return BadRequest("Inventory type name is missing.");
                }

                // Create a new Inventory_Type
                var inventoryType = new InventoryType
                {
                    InventoryTypeName = inventoryTypeName1.InventoryTypeName,
                    InventoryTypeId = inventoryTypeName1.InventoryTypeId,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = inventoryTypeName1.CreatedBy,
                    Deleted = inventoryTypeName1.Deleted,
                };
                if (inventoryTypeName1.Deleted)
                {
                    inventoryType.DeletedDate = DateTime.UtcNow;
                    inventoryType.DeletedBy = inventoryTypeName1.DeletedBy;
                }
                // Add the newly created Inventory_Type
                _inventoryTypeRepository.AddInventoryType(inventoryType);
                // Save changes to the database
                _inventoryTypeRepository.SaveChanges();

                // Return a response indicating successful creation
                return CreatedAtAction(nameof(GetInventoryTypes), new { inventoryTypeId = inventoryType.InventoryTypeId }, inventoryType);
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.Error($"An error occurred in: {nameof(AddInventoryType)}, exception: {ex.Message}.");

                // Return a response indicating failure
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [Route("update/{inventoryTypeId}")]
        [HttpPut]
        public async Task<IActionResult> UpdateInventoryType(int inventoryTypeId, [FromBody] InventoryType inventoryTypeUpdate)
        {
            try
            {
                // Retrieve the existing inventory type from the repository
                var existingInventoryType = _inventoryTypeRepository.GetInventoryTypeById(inventoryTypeId);

                if (existingInventoryType == null)
                {
                    return NotFound("Inventory type not found.");
                }

                // Update the properties of the existing inventory type
                existingInventoryType.InventoryTypeName = inventoryTypeUpdate.InventoryTypeName;
                existingInventoryType.Deleted = inventoryTypeUpdate.Deleted;

                // If the inventory type is marked as deleted, update deleted date and deleted by
                if (existingInventoryType.Deleted)
                {
                    existingInventoryType.DeletedDate = DateTime.UtcNow;
                    existingInventoryType.DeletedBy = inventoryTypeUpdate.DeletedBy;

                    // Update the Inventory table
                    _inventoryTypeRepository.UpdateInventoryTypeIdWhenDeleted(existingInventoryType.InventoryTypeId);
                }
                else
                {
                    // If the inventory type is no longer deleted, clear deleted date and deleted by
                    existingInventoryType.DeletedDate = null;
                    existingInventoryType.DeletedBy = null;
                }

                // Update the existing inventory type in the repository
                _inventoryTypeRepository.UpdateInventoryType(existingInventoryType);
                // Save changes to the database
                _inventoryTypeRepository.SaveChanges();

                // Return a response indicating successful update
                return Ok(existingInventoryType);
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.Error($"An error occurred in: {nameof(UpdateInventoryType)}, exception: {ex.Message}.");

                // Return a response indicating failure
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPut("delete/{inventoryTypeId}")]
        public async Task<IActionResult> DeleteInventoryType(int inventoryTypeId, [FromBody] InventoryType inventoryTypeDelete)
        {
            try
            {
                // Retrieve the existing inventory type from the repository
                var existingInventoryType = _inventoryTypeRepository.GetInventoryTypeById(inventoryTypeId);

                if (existingInventoryType == null)
                {
                    return NotFound("Inventory type not found.");
                }

                existingInventoryType.InventoryTypeName = inventoryTypeDelete.InventoryTypeName;
                existingInventoryType.Deleted = inventoryTypeDelete.Deleted;

                // If the inventory type is marked as deleted, update deleted date and deleted by
                if (existingInventoryType.Deleted)
                {
                    existingInventoryType.DeletedDate = DateTime.UtcNow;
                    existingInventoryType.DeletedBy = inventoryTypeDelete.DeletedBy;

                    // Update the Inventory table
                    _inventoryTypeRepository.UpdateInventoryTypeIdWhenDeleted(existingInventoryType.InventoryTypeId);
                }
                else
                {
                    // If the inventory type is no longer deleted, clear deleted date and deleted by
                    existingInventoryType.DeletedDate = null;
                    existingInventoryType.DeletedBy = null;
                }

                // Update the existing inventory type in the repository
                _inventoryTypeRepository.DeleteInventoryType(existingInventoryType);
                // Save changes to the database
                _inventoryTypeRepository.SaveChanges();

                // Return a response indicating successful update
                return Ok(existingInventoryType);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in {nameof(DeleteInventoryType)}, exception: {ex.Message}.");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [Route("inventoryTypeCount")]
        [HttpGet]
            public ActionResult GetTotalInventoryTypes()
        {
            try
            {
                var totalInventoryTypes = _inventoryTypeRepository.GetTotalInventoryTypes().Where(i => !i.Deleted && i.InventoryTypeId != 0);
                var totalInventoryCount = totalInventoryTypes.Count();
                return Ok(totalInventoryCount);

            }
            catch(Exception ex)
            {
                _logger.Error($"An error occurred in {nameof(DeleteInventoryType)}, exception: {ex.Message}.");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }










    }
}
