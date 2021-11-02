using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AlgoPlus.Storage.Services
{
    public class LocalDiskStorage : IStorage
    {
        private readonly string baseDirectory;

        public LocalDiskStorage(string baseDirectory)
        {
            this.baseDirectory = baseDirectory;
        }

        public async Task<ReturnFileInfo> SaveAsync(string filename, string content)
        {
            if (filename == null)
                throw new ArgumentNullException(nameof(filename), "O nome do arquivo deve ser informado");
            if (content == null)
                throw new ArgumentNullException(nameof(content), "O conteúdo do arquivo deve ser informado");

            //var diretorio = Path.Combine(baseDirectory, subDiretorio);

            CriarDiretorioSeNaoExistir(baseDirectory);

            var file = Path.Combine(baseDirectory, filename);
            var stw = new StreamWriter(file);
            await stw.WriteLineAsync(content);
            stw.Close();
            var info = new FileInfo(file);
            var returnInfo = new ReturnFileInfo { 
                Filename = info.Name,
                AbsolutePath = info.FullName,
                RelativePath = info.DirectoryName,
                ContentLength = info.Length,
                CreatedOn = info.CreationTimeUtc,
                LastModified =info.LastWriteTimeUtc,
            };
            return returnInfo;
        }

        public Task<ReturnFileInfo> SaveAsync(string filename, byte[] content)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(string path)
        {
            throw new NotImplementedException();
        }

        public Task<IList<ReturnFileInfo>> GetFilesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> GetFileAsync(string path)
        {
            throw new NotImplementedException();
        }

        private static string CriarDiretorioSeNaoExistir(string diretorio)
        {
            if (diretorio == null)
                throw new ArgumentNullException(nameof(diretorio));

            if (!Directory.Exists(diretorio))
                Directory.CreateDirectory(diretorio);

            return diretorio;
        }
    }
}
