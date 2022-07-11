using AlgoPlus.Storage.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlgoPlus.Storage.Services
{
    public interface IStorageContext
    {
        Task<ReturnFileInfo> SaveAsync(string filename, string content, string serviceName);

        Task<ReturnFileInfo> SaveAsync(string filename, byte[] content, string serviceName);

        IStorage GetStorage(string storageName);
    }

    public class StorageContext : IStorageContext
    {
        private readonly IEnumerable<IStorage> storages;

        public StorageContext(IEnumerable<IStorage> storages)
        {
            this.storages = storages;
        }

        public IStorage GetStorage(string storageName)
        {
            var storage = storages.FirstOrDefault(x => x.Name == storageName);
            if (storage == null)
                throw new ServiceNotFoundException("Servico de armazenamento nao encontrado");
            return storage;
        }

        public async Task<ReturnFileInfo> SaveAsync(string filename, string content, string storageName)
        {
            var storage = GetStorage(storageName);
            return await storage.SaveAsync(filename, content);
        }

        public async Task<ReturnFileInfo> SaveAsync(string filename, byte[] content, string storageName)
        {
            var storage = GetStorage(storageName);
            return await storage.SaveAsync(filename, content);
        }
    }
}
