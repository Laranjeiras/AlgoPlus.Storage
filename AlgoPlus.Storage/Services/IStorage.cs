using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlgoPlus.Storage.Services
{
    public interface IStorage
    {
        string Name { get; }

        string BasePath { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subdirectory">Subdirectory or Container</param>
        /// <param name="filename"></param>
        /// <param name="content">file content</param>
        /// <returns></returns>
        Task<ReturnFileInfo> SaveAsync(string filename, string content);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subdirectory">Subdirectory or Container</param>
        /// <param name="filename"></param>
        /// <param name="content">file content</param>
        /// <returns></returns>
        Task<ReturnFileInfo> SaveAsync(string filename, byte[] content);

        Task<bool> DeleteAsync(string path);

        Task<byte[]> GetFileAsync(string path);

        Task<IList<ReturnFileInfo>> GetFilesAsync();
    }
}
