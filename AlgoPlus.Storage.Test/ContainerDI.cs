using AlgoPlus.Storage.Configs;
using AlgoPlus.Storage.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Text.Json;

namespace AlgoPlus.Storage.Test
{
    public abstract class ContainerDI
    {
        protected readonly IServiceProvider ServiceProvider;

        public ContainerDI()
        {
            var config = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText("appsettings.json"));

            var services = new ServiceCollection();

            services.AddSingleton<AzureConfig>(config.AzureConfig);
            services.AddSingleton<AwsS3Config>(config.AwsConfig);

            services.AddScoped<IStorage, AzureStorage>(x => new AzureStorage(config.AzureConfig, "AzureAlex"));
            services.AddScoped<IStorage, AwsS3Storage>(x => new AwsS3Storage(config.AwsConfig, "AWSSacFiscal"));            
            services.AddScoped<IStorage, AzureStorage>();
            services.AddScoped<IStorage, AwsS3Storage>();
            services.AddScoped<IStorage, LocalDiskStorage>(x => new LocalDiskStorage(@"D:\temp\ApagarStorage", "Localnfe"));
            services.AddScoped<IStorage, LocalDiskStorage>(x => new LocalDiskStorage(@"D:\temp\ApagarStorage"));

            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
