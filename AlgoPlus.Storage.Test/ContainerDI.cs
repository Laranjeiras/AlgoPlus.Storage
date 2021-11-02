using AlgoPlus.Storage.Configs;
using AlgoPlus.Storage.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Text.Json;

namespace AlgoPlus.Storage.Test
{
    public class ContainerDI
    {
        public IServiceProvider ServiceProvider { get; protected set; }

        public ContainerDI()
        {
            var config = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText("appsettings.json"));

            var services = new ServiceCollection();

            services.AddSingleton<AzureConfig>(config.AzureConfig);
            services.AddSingleton<AwsS3Config>(config.AwsConfig);

            services.AddScoped<IStorage, AzureStorage>();
            services.AddScoped<IStorage, LocalDiskStorage>();
            services.AddScoped<IStorage, AwsS3Storage>();

            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
