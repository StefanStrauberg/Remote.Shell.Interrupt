namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Configuration;

internal class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
  : DbContext(options)
{
  public DbSet<Client> Clients { get; set; }
  public DbSet<COD> CODs { get; set; }
  public DbSet<TfPlan> TfPlans { get; set; }
  public DbSet<SPRVlan> SPRVlans { get; set; }
  public DbSet<Gate> Gates { get; set; }
  public DbSet<NetworkDevice> NetworkDevices { get; set; }
  public DbSet<Port> Ports { get; set; }
  public DbSet<ARPEntity> ARPEntities { get; set; }
  public DbSet<MACEntity> MACEntities { get; set; }
  public DbSet<TerminatedNetworkEntity> TerminatedNetworkEntities { get; set; }
  public DbSet<VLAN> VLANs { get; set; }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    builder.ApplyConfiguration(new ClientConfiguration());
    builder.ApplyConfiguration(new CODConfiguration());
    builder.ApplyConfiguration(new TfPlanConfiguration());
    builder.ApplyConfiguration(new SPRVlanConfiguration());
    builder.ApplyConfiguration(new GateConfiguration());
    builder.ApplyConfiguration(new NetworkDeviceConfiguration());
    builder.ApplyConfiguration(new PortConfiguration());
    builder.ApplyConfiguration(new ARPEntityConfiguration());
    builder.ApplyConfiguration(new MACEntityConfiguration());
    builder.ApplyConfiguration(new TerminatedNetworkEntityConfiguration());
    builder.ApplyConfiguration(new VLANConfiguration());
    builder.ApplyConfiguration(new PortVlanConfiguration());
  }
}