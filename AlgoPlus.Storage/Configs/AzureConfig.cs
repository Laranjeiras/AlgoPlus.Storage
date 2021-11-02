namespace AlgoPlus.Storage.Configs
{
    public class AzureConfig
    {
        public string ConnectionString { get; set; }
        public string Container { get; set; }
        public bool ReplaceIfExist { get; set; } = true;
    }
}
