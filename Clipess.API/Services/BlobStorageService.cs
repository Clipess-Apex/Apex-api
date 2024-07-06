
/*using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;


namespace Clipess.API.Services
{
    public class BlobStorageService
    {

        private readonly string _connectionString;
        private readonly string _containerName;

        public BlobStorageService(string connectionString, string containerName)
        {
            _connectionString = connectionString;
            _containerName = containerName;
        }

        private async Task<string> SaveFile(IFormFile file)
        {
            // Get the connection string from configuration
            string connectionString = Configuration["AzureStorageConnectionString"];

            // Create a BlobServiceClient instance
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            // Get a reference to the container (create if it doesn't exist)
            string containerName = "inventory-images"; // Replace with your desired container name
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();

            // Generate a unique name for the blob
            string blobName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

            // Upload the file to the blob
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.UploadFromStreamAsync(file.OpenReadStream());

            // Return the blob URL
            return blobClient.Uri.ToString();
        }

        public bool IsValidFileType(IFormFile file)
        {
            string[] allowedFileTypes = { "image/jpeg", "image/jpg", "image/png", "application/pdf" };
            return allowedFileTypes.Contains(file.ContentType.ToLower());
        }
    }
}
*/