namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class RemoteClientsRepository(MySQLDapperContext mySQLDapperContext) 
  : IRemoteClientsRepository
{
  protected readonly MySQLDapperContext _mySQLDapperContext = mySQLDapperContext
    ?? throw new ArgumentNullException(nameof(mySQLDapperContext));

  async Task<IEnumerable<RemoteClient>> IRemoteClientsRepository.GetAllAsync(CancellationToken cancellationToken)
  {
    StringBuilder sb = new();
    sb.Append("SELECT ");
    sb.Append($"cc.id_client as \"{nameof(RemoteClient.IdClient)}\", ");
    sb.Append($"cc.dat1 as \"{nameof(RemoteClient.Dat1)}\", ");
    sb.Append($"cc.dat2 as \"{nameof(RemoteClient.Dat2)}\", ");
    sb.Append($"cc.prim1 as \"{nameof(RemoteClient.Prim1)}\", ");
    sb.Append($"cc.prim2 as \"{nameof(RemoteClient.Prim2)}\", ");
    sb.Append($"cc.nik as \"{nameof(RemoteClient.Nik)}\", ");
    sb.Append($"cc.name as \"{nameof(RemoteClient.Name)}\", ");
    sb.Append($"cc.nr_dogovor as \"{nameof(RemoteClient.NrDogovor)}\", ");
    sb.Append($"cc.contact_C as \"{nameof(RemoteClient.ContactC)}\", ");
    sb.Append($"cc.telefon_C as \"{nameof(RemoteClient.TelephoneC)}\", ");
    sb.Append($"cc.contact_T as \"{nameof(RemoteClient.ContactT)}\", ");
    sb.Append($"cc.telefon_T as \"{nameof(RemoteClient.TelephoneT)}\", ");
    sb.Append($"cc.c_email as \"{nameof(RemoteClient.EmailC)}\", ");
    sb.Append($"cc.`_working` as \"{nameof(RemoteClient.Working)}\", ");
    sb.Append($"cc.t_email as \"{nameof(RemoteClient.EmailT)}\", ");
    sb.Append($"cc.id_cod as \"{nameof(RemoteClient.Id_COD)}\", ");
    sb.Append($"cc.id_tplan as \"{nameof(RemoteClient.Id_TfPlan)}\", ");
    sb.Append($"cc.history as \"{nameof(RemoteClient.History)}\", ");
    sb.Append($"cc.ad as \"{nameof(RemoteClient.AntiDDOS)}\" ");
    sb.Append($"FROM client_cod AS cc");

    var query = sb.ToString();
    var connection = await _mySQLDapperContext.CreateConnectionAsync(cancellationToken);
    return await connection.QueryAsync<RemoteClient>(query);
  }
}
