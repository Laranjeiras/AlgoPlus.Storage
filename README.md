# AlgoPlus.Storage
Tools for save files to AWS, AZURE or LocalStorage

# Configure Dependency Injection

On Startup.cs or Program.cs (builder.Services), chose one

![AZURE](https://img.shields.io/badge/microsoft%20azure-0089D6?style=for-the-badge&logo=microsoft-azure&logoColor=white)

```
var azureConfig = new AzureConfig
{
    ConnectionString = "YOUR CONNECTION STRING",
    Container = "YOUR CONTAINER"
};

services.AddScoped<IStorage, AzureStorage>(x => new AzureStorage(azureConfig));
```

![AWS](https://img.shields.io/badge/Amazon_AWS-FF9900?style=for-the-badge&logo=amazonaws&logoColor=white)

```
var configAws = new AwsS3Config
{
    AccessKey = "YOUR ACCESS KEY",
    SecretKey = "YOUR SECRET KEY",
    RegionEndPoint = "region",
    Bucketname = "laranjeiras"
};

services.AddScoped<IStorage, AwsS3Storage>(x => new AwsS3Storage(config.AwsConfig));
```

#### LOCALSTORAGE
```
services.AddScoped<IStorage, LocalDiskStorage>(x => new LocalDiskStorage(@"D:\MySaveFiles"));
```


# Use in your class

```
public class TestStorage
{
  private readonly IStorage storage;

  public TestStorage(IStorage storage)
  {
      this.storage = storage;
  }

  public async Task SaveText()
  {
      var text = "TEST ALGOPLUS.STORAGE";
      await this.storage.SaveAsync("MyFile.txt", text);
  }

  public async Task LoadFile()
  {
      var file = await this.storage.GetFileAsync("MyFile.txt");
  }

  public async void DeleteFile()
  {
      await this.storage.DeleteAsync("MyFile.txt");
  }
}
```
