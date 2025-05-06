namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Models;

internal class JoinEntityBuilder<TJoinEntity>(EntityConfiguration config)
{
  public JoinEntityBuilder<TJoinEntity> HasLeftKey(string columnName)
  {
    config.Properties.Add(columnName);
    return this;
  }

  public JoinEntityBuilder<TJoinEntity> HasRightKey(string columnName)
  {
    config.Properties.Add(columnName);
    return this;
  }
}