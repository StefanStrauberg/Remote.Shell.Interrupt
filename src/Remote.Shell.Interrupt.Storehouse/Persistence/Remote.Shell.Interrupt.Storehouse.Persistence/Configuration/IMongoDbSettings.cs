namespace Remote.Shell.Interrupt.Storehouse.Persistence.Configuration;

public interface IMongoDbSettings
{
  string DatabaseName { get; set; }
  string ConnectionString { get; set; }
}
