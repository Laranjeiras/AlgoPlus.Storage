using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AlgoPlus.Storage.Services
{
    public class LocalDiskStorage : IStorage
    {
        private readonly string baseDirectory;

        private readonly string name;
        public string Name => name;
        public string BasePath => baseDirectory;

        public LocalDiskStorage(string baseDirectory, string name = null)
        {
            this.baseDirectory = baseDirectory;
            this.name = name ?? nameof(LocalDiskStorage);
        }

        /// <summary>
        /// Save file to base directory, concat base directory + filename
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<ReturnFileInfo> SaveAsync(string filename, string content)
        {
            var bytes = Encoding.ASCII.GetBytes(content);
            return await SaveAsync(filename, bytes);
        }

        /// <summary>
        /// Save file to base directory, concat base directory + filename
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<ReturnFileInfo> SaveAsync(string filename, byte[] content)
        {
            if (filename == null)
                throw new ArgumentNullException(nameof(filename), "The file name must be provided");
            if (content == null)
                throw new ArgumentNullException(nameof(content), "The contents of the file must be informed");

            var file = Path.Combine(baseDirectory, filename);
            var info = new FileInfo(file);

            CreateDirectoryIfNotExist(info.Directory.FullName);

            await File.WriteAllBytesAsync(file, content);

            var returnInfo = new ReturnFileInfo
            {
                Filename = info.Name,
                AbsolutePath = info.FullName,
                RelativePath = info.DirectoryName,
                ContentLength = info.Length,
                CreatedOn = info.CreationTimeUtc,
                LastModified = info.LastWriteTimeUtc,
            };
            return returnInfo;
        }

        public async Task<bool> DeleteAsync(string path)
        {
            File.Delete(path);
            return await Task.FromResult(true);
        }

        public async Task<IList<ReturnFileInfo>> GetFilesAsync()
        {
            var files = Directory.GetFiles(baseDirectory, "*", SearchOption.AllDirectories);
            var returnFiles = new List<ReturnFileInfo>();

            foreach (var file in files)
            {
                var info = new FileInfo(file);
                var returnFile = new ReturnFileInfo
                {
                    AbsolutePath = info.FullName,
                    ContentLength = info.Length,
                    CreatedOn = info.CreationTime,
                    LastModified = info.LastWriteTime,
                    Filename = info.Name,
                };
                returnFiles.Add(returnFile);
            }

            return await Task.FromResult(returnFiles);
        }

        public async Task<byte[]> GetFileAsync(string path)
        {
            var file = await File.ReadAllBytesAsync(path);
            return file;
        }

        private static string CreateDirectoryIfNotExist(string directory)
        {
            if (directory == null)
                throw new ArgumentNullException(nameof(directory));

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            return directory;
        }
    }
}
