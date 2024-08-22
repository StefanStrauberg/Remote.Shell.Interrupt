namespace Remote.Shell.Interrupt.Storehouse.Persistence.Configuration;

internal interface IMongoDbSettings
{
  string DatabaseName { get; set; }
  string ConnectionString { get; set; }
}
