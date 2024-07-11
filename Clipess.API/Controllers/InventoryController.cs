using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Clipess.DBClient.Repositories;
using System.Linq.Expressions;

namespace Clipess.API.Controllers
{
    [Route("api/inventory")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryRepository _inventoryRepository;
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IConfiguration _configuration;
        private readonly Cloudinary _cloudinary;

        public InventoryController(IInventoryRepository inventoryRepository, IConfiguration configuration)
        {
            _inventoryRepository = inventoryRepository;
            _configuration = configuration;
            _cloudinary = new Cloudinary(new Account
            {
                Cloud = _configuration["Cloudinary:CloudName"],
                ApiKey = _configuration["Cloudinary:ApiKey"],
                ApiSecret = _configuration["Cloudinary:ApiSecret"]
            });
        }



        [Route("inventory/{inventoryId}")]
        [HttpGet]
        public ActionResult GetInventory(int inventoryId)
        {
            try
            {
                var inventory = _inventoryRepository.GetInventories().FirstOrDefault(x => x.InventoryId == inventoryId && !x.Deleted);

                if (inventory != null)
                {
                    return Ok(inventory);
                }
                else
                {
                    return NotFound($"Inventory with ID {inventoryId} not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in {nameof(GetInventory)} for inventoryId: {inventoryId}, exception: {ex.Message}.");
                return StatusCode(500, $"An error occurred while fetching inventory: {ex.Message}");
            }
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddInventory([FromForm] InventoryFormData formData)
        {
            try
            {
                if (formData == null)
                {
                    return BadRequest("Inventory details are missing.");
                }

                var otherData = JsonConvert.DeserializeObject<Inventory>(formData.OtherData);

                var inventory = new Inventory
                {
                    InventoryTypeId = otherData.InventoryTypeId,
                    InventoryName = otherData.InventoryName,
                    CreatedDate = DateTime.UtcNow,
                    EmployeeId = otherData.EmployeeId,
                    CreatedBy = otherData.CreatedBy,
                    Deleted = otherData.Deleted,
                   
                    FileUrl = null, // Assign null if not provided
                    ImageUrl = null, // Assign null if not provided
                   
                };

                if (inventory.EmployeeId != 0)
                {
                    inventory.AssignedDate = DateTime.UtcNow;
                }

                if(inventory.Deleted != false)
                {
                    inventory.DeletedDate = DateTime.UtcNow;
                    inventory.DeletedBy = otherData.DeletedBy;

                }
                // check if file is provided and has valid file types
                if (formData.File != null && formData.File.Length > 0)
                {
                    if (!IsValidFileType(formData.File, FileType.File))
                    {
                        return BadRequest("Invalid file type. Only jpg, jpeg, png are allowed.");
                    }

                    using (var memoryStream = new MemoryStream())
                    {
                        await formData.File.CopyToAsync(memoryStream);
                        memoryStream.Position = 0; // Reset stream position for upload

                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(formData.File.FileName, memoryStream)
                        };

                        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                        inventory.FileUrl = uploadResult.Url.ToString();
                    }
                }
              

                //check if image is provided and file has valid file type
                if (formData.Image != null && formData.Image.Length > 0)
                {
                    if (!IsValidFileType(formData.Image, FileType.Image))
                    {
                        return BadRequest("Invalid image type Only jpg, jpeg, png, are allowed.");
                    }

                    using (var memoryStream = new MemoryStream())
                    {
                        await formData.Image.CopyToAsync(memoryStream);
                        memoryStream.Position = 0; // Reset stream position for upload

                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(formData.Image.FileName, memoryStream)
                        };

                        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                        inventory.ImageUrl = uploadResult.Url.ToString();
                    }
                }

                _inventoryRepository.AddInventory(inventory);
                _inventoryRepository.SaveChanges();

                return CreatedAtAction(nameof(AddInventory), new { inventoryId = inventory.InventoryId }, inventory);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in {nameof(AddInventory)}, exception: {ex.Message}.");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
      

        private bool IsValidFileType(IFormFile file, FileType fileType)
        {
            // Allowed file extensions based on the file type
            string[] allowedExtensions;

            if (fileType == FileType.Image)
            {
                allowedExtensions = new string[] { ".jpg", ".jpeg", ".png" };
            }
            else if (fileType == FileType.File)
            {
                allowedExtensions = new string[] { ".pdf" };
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(fileType), "Invalid file type");
            }

            string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return allowedExtensions.Contains(extension);
        }

        public enum FileType
        {
            Image,
            File
        }


        [HttpPut("update/{inventoryId}")]
        public async Task<IActionResult> UpdateInventory(int inventoryId, [FromForm] InventoryFormData formData)
        {
            try
            {
                if (formData == null)
                {
                    return BadRequest("Inventory details are missing.");
                }

                var inventory = _inventoryRepository.GetInventoryById(inventoryId);
                if (inventory == null)
                {
                    return NotFound($"Inventory with ID {inventoryId} not found.");
                }

                if (!string.IsNullOrEmpty(formData.OtherData))
                {
                    var otherData = JsonConvert.DeserializeObject<Inventory>(formData.OtherData);

                    inventory.InventoryTypeId = otherData.InventoryTypeId;
                    inventory.InventoryName = otherData.InventoryName;
                    inventory.EmployeeId = otherData.EmployeeId;
                    inventory.CreatedBy = otherData.CreatedBy;
                    inventory.Deleted = otherData.Deleted;
                    if (inventory.EmployeeId != 0)
                    {
                        inventory.AssignedDate = DateTime.UtcNow;
                    }

                }

                if (formData.File != null)
                {

                    if (!IsValidFileType(formData.File, FileType.File))
                    {
                        return BadRequest("Invalid file type. Only pdf files are allowed.");
                    }
                    using (var memoryStream = new MemoryStream())
                    {
                        await formData.File.CopyToAsync(memoryStream);
                        memoryStream.Position = 0; // Reset stream position for upload

                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(formData.File.FileName, memoryStream)
                        };

                        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                        inventory.FileUrl = uploadResult.Url.ToString();
                    }

                   
                   
                }

                if (formData.Image != null)
                {
                    if (!IsValidFileType(formData.Image, FileType.Image))
                    {
                        return BadRequest("Invalid image type. Only jpg, jpeg, png are allowed.");
                    }

                    using (var memoryStream = new MemoryStream())
                    {
                        await formData.Image.CopyToAsync(memoryStream);
                        memoryStream.Position = 0; // Reset stream position for upload

                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(formData.Image.FileName, memoryStream)
                        };

                        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                        inventory.ImageUrl = uploadResult.Url.ToString();
                    }
                }

                _inventoryRepository.UpdateInventory(inventory);
                _inventoryRepository.SaveChanges();

                return Ok(inventory);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in {nameof(UpdateInventory)}, exception: {ex.Message}.");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }





        [Route("filter/type/{typeId?}/{employeeId?}")]
         [Route("filter/employee/{employeeId?}/{typeId?}")]
         [HttpGet]
         public ActionResult GetInventoryByEmployeeAndType(int? employeeId, int? typeId)
         {
             try
             {
                 // Call the modified stored procedure based on employeeId and typeId
                 var inventories = _inventoryRepository.GetInventoryByEmployeeAndType(employeeId, typeId)
                     .Where(x => !x.Deleted && x.InventoryTypeId !=0)

                     .ToList();
                if (inventories != null && inventories.Any())
                {
                    return Ok(inventories);
                }

                return NoContent();


            }
             catch (Exception ex)
             {
                 _logger.Error($"An error occurred in: {nameof(GetInventoryByEmployeeAndType)}, exception: {ex.Message}.");
                  return StatusCode(500, $"An error occurred: {ex.Message}");
             }
         }

        [Route("unassigned/{typeId?}")]
        
        [HttpGet]
        public ActionResult GetUnassignedInventoryByType(int employeeId, int typeId)
        {
            try
            {
                // Call the modified stored procedure based on employeeId and typeId
                var inventories = _inventoryRepository.GetUnassignedInventoryByType(employeeId, typeId)
                    .Where(x => !x.Deleted && x.EmployeeId == 0 && x.InventoryTypeId == typeId)

                    .ToList();
                return Ok(inventories);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in: {nameof(GetInventoryByEmployeeAndType)}, exception: {ex.Message}.");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }





        [HttpPut("delete/{inventoryId}")]
        public async Task<IActionResult> DeleteInventory(int inventoryId, [FromForm] InventoryFormData formData)
        {
            try
            {
                if (formData == null)
                {
                    return BadRequest("Inventory details are missing.");
                }

                var inventory = _inventoryRepository.GetInventoryById(inventoryId);
                if (inventory == null)
                {
                    return NotFound($"Inventory with ID {inventoryId} not found.");
                }

                if (!string.IsNullOrEmpty(formData.OtherData))
                {
                    var otherData = JsonConvert.DeserializeObject<Inventory>(formData.OtherData);

                    inventory.Deleted = otherData.Deleted;
                    inventory.DeletedBy = otherData.DeletedBy;
                }

                _inventoryRepository.UpdateInventory(inventory);
                _inventoryRepository.SaveChanges();

                return Ok(inventory);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in {nameof(UpdateInventory)}, exception: {ex.Message}.");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }



        [Route("AssigningData")]
        [HttpGet]
        public ActionResult GetInventoryAssignData()
        {
            try
            {
                var assignedInventories = _inventoryRepository.GetInventoryAssignData().Where(i => !i.Deleted).ToList();


                int assignedCount = assignedInventories.Count(i => i.EmployeeId != 0);
                int unassignedCount = assignedInventories.Count(i => i.EmployeeId == 0);

                var result = new
                {
                    AssignedCount = assignedCount,
                    UnassignedCount = unassignedCount
                };

                return Ok(result); // Return success with count information
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in {nameof(GetInventoryAssignData)}: {ex.Message}");
                return StatusCode(500, $"An error occurred while fetching inventory data: {ex.Message}");
            }
        }

        [Route("countByType")]
        [HttpGet]
        public ActionResult GetNoOfInventoryByType()
        {
            try
            {
               var inventories = _inventoryRepository.GetNoOfInventoryByType() 
                    .Where(i => !i.Deleted && i.InventoryTypeId != 0) // Filter out deleted inventories
                    .GroupBy(i => i.InventoryTypeId) // Group by inventory type ID
                    .Select(group => new 
                    {
                        InventoryTypeId = group.Key, // Inventory type ID
                        Count = group.Count() // Number of inventories in this type
                    })
                    .OrderByDescending(i => i.Count) // Order by count descending
                    .Take(8) 
                    .ToList(); 

                return Ok(inventories); 
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in {nameof(GetInventoryAssignData)}: {ex.Message}");
                return StatusCode(500, $"An error occurred while fetching inventory data: {ex.Message}");
            }
        }


        [Route("totalNoOfInventories")]
        [HttpGet]
            public ActionResult GetTotalNoOfInventories()
        {
            try
            {
                var TotalInventories = _inventoryRepository.GetTotalNoOfInventories().Where(i => !i.Deleted && i.InventoryTypeId!=0).ToList();

                var lengthOfTotalInventories = TotalInventories.Count();

                return Ok(lengthOfTotalInventories);
            } catch (Exception ex)
            {
                _logger.Error($"An error occurred in {nameof(GetInventoryAssignData)}: {ex.Message}");
                return StatusCode(500, $"An error occurred while fetching inventory data: {ex.Message}");
             }

         }

        [Route("totalInventories/{employeeId}")]
        [HttpGet]
        public ActionResult GetTotalNoOfInventories(int employeeId)
        {
            try
            {
                var TotalInventories = _inventoryRepository.GetTotalNoOfInventories().Where(i => !i.Deleted && i.InventoryTypeId != 0 && i.EmployeeId==employeeId).ToList();

                var lengthOfTotalInventories = TotalInventories.Count();

                return Ok(lengthOfTotalInventories);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in {nameof(GetInventoryAssignData)}: {ex.Message}");
                return StatusCode(500, $"An error occurred while fetching inventory data: {ex.Message}");
            }

        }

        [Route("inventoriesCountByType")]
        [HttpGet]
        public ActionResult GetNoOfInventoryOfAllType()
        {
            try
            {
                var inventories = _inventoryRepository.GetNoOfInventoryByType()
                     .Where(i => !i.Deleted && i.InventoryTypeId != 0) // Filter out deleted inventories
                     .GroupBy(i => i.InventoryTypeId) // Group by inventory type ID
                     .Select(group => new
                     {
                         InventoryTypeId = group.Key, // Inventory type ID
                         Count = group.Count() // Number of inventories in this type
                     })
                     .OrderByDescending(i => i.Count) // Order by count descending
                     .ToList();


                return Ok(inventories);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in {nameof(GetInventoryAssignData)}: {ex.Message}");
                return StatusCode(500, $"An error occurred while fetching inventory data: {ex.Message}");
            }
        }






    }












    public class InventoryFormData
    {
        public string OtherData { get; set; }

        public Microsoft.AspNetCore.Http.IFormFile? File { get; set; }

        public Microsoft.AspNetCore.Http.IFormFile? Image { get; set; }
    }
}


