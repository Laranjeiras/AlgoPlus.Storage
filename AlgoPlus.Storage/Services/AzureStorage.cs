using AlgoPlus.Storage.Configs;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AlgoPlus.Storage.Services
{
    public class AzureStorage : IStorage
    {
        private readonly BlobServiceClient blobServiceClient;
        private readonly bool replaceIfExist;
        private readonly string containerName;

        private readonly string name;
        public string Name => name;

        public AzureStorage(AzureConfig azureConfig, string name = null)
        {
            this.blobServiceClient = new BlobServiceClient(azureConfig.ConnectionString);
            this.replaceIfExist = azureConfig.ReplaceIfExist;
            this.containerName = azureConfig.Container;
            this.name = name ?? nameof(AzureStorage);
        }

        public async Task<ReturnFileInfo> SaveAsync(string filename, string content)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(content);
            return await SaveAsync(filename, bytes);
        }

        public async Task<ReturnFileInfo> SaveAsync(string filename, byte[] content)
        {
            try
            {
                if (string.IsNullOrEmpty(containerName))
                    throw new ArgumentNullException(nameof(containerName));

                BlobContainerClient containerClient = await CreateContainerAsync(containerName);

                var blobClient = containerClient.GetBlobClient(filename);
                await blobClient.UploadAsync(new MemoryStream(content), replaceIfExist);

                return new ReturnFileInfo { Filename = filename, AbsolutePath = blobClient.Uri.AbsoluteUri };

            }
            catch (RequestFailedException)
            {
                throw;
            }
        }

        public async Task<bool> DeleteAsync(string filename)
        {
            var blobContainer = blobServiceClient.GetBlobContainerClient(containerName);

            if (!blobContainer.Exists())
                return false;

            var response = await blobContainer.DeleteBlobIfExistsAsync(filename);
            return response;
        }

        public async Task<byte[]> GetFileAsync(string nomeArquivo)
        {
            var blobContainer = blobServiceClient.GetBlobContainerClient(containerName);

            if (!blobContainer.Exists())
                throw new FileNotFoundException();

            var blob = blobContainer.GetBlobClient(nomeArquivo);

            var ms = new MemoryStream();
            await blob.DownloadToAsync(ms);
            byte[] bytes = ms.ToArray();
            return bytes;
        }
        
        public async Task<IList<ReturnFileInfo>> GetFilesAsync()
        {
            try
            {
                var containerClient = await CreateContainerAsync(containerName);

                var infos = new List<ReturnFileInfo>();
                await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
                    infos.Add(new ReturnFileInfo { 
                        AbsolutePath = containerClient.Uri.AbsoluteUri + blobItem.Name,
                        RelativePath = containerClient.Uri.AbsolutePath,
                        Filename = blobItem.Name,
                        CreatedOn = blobItem.Properties?.CreatedOn?.DateTime,
                        LastModified = blobItem.Properties?.LastModified?.DateTime,
                        ContentLength = blobItem.Properties?.ContentLength
                    });

                return infos;
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        /// <summary>
        /// Create Container if not exist and return BlobContainerClient
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="throwExceptionIfExist"></param>
        /// <returns></returns>
        private async Task<BlobContainerClient> CreateContainerAsync(string containerName, bool throwExceptionIfExist = false)
        {
            try
            {
                var existe = blobServiceClient.GetBlobContainerClient(containerName);

                if (existe.Exists() && throwExceptionIfExist)
                    throw new Exception("Container Already exists");
                else if (existe.Exists())
                    return existe;

                BlobContainerClient container = await blobServiceClient.CreateBlobContainerAsync(containerName);

                if (await container.ExistsAsync())
                    return container;
                throw new Exception("Could not find container");
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine("HTTP error code {0}: {1}", e.Status, e.ErrorCode);
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}
