using AlgoPlus.Storage.Configs;

namespace AlgoPlus.Storage.Test
{
    internal class AppSettings
    {
        public AwsS3Config AwsConfig { get; set; }

        public AzureConfig AzureConfig { get; set; }
    }
}