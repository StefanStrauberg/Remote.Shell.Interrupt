namespace Remote.Shell.Interrupt.Storehouse.Persistence.Configuration;

public class MongoDbSettings : IMongoDbSettings
{
  public string DatabaseName { get; set; } = string.Empty;
  public string ConnectionString { get; set; } = string.Empty;
}