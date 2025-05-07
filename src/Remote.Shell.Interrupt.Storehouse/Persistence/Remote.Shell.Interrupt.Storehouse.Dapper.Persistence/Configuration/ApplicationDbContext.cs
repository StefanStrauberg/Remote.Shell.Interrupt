namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Configuration;

internal class ApplicationDbContext(IRelationshipValidatorFactory validatorFactory) : DbContext
{
  readonly IRelationshipValidatorFactory _validatorFactory = validatorFactory;
  public DbSet<Client> Clients => Set<Client>();
  public DbSet<COD> CODs => Set<COD>();
  public DbSet<TfPlan> TfPlans => Set<TfPlan>();
  public DbSet<SPRVlan> SPRVlans => Set<SPRVlan>();
  public DbSet<Gate> Gates => Set<Gate>();

  protected override IRelationshipValidatorFactory RelationshipValidatorFactory 
      => _validatorFactory;

  protected override void OnModelCreating(ModelBuilder builder)
  {
    builder.Entity<Client>(e =>
    {
      e.ToTable("Clients")
       .HasKey(x => x.IdClient);

      e.HasOne(x => x.COD)
       .WithMany()
       .HasForeignKey(x => x.Id_COD);

      e.HasOne(x => x.TfPlan)
       .WithMany()
       .HasForeignKey(x => x.Id_TfPlan);

      e.HasMany(x => x.SPRVlans)
       .WithOne()
       .HasForeignKey(x => x.IdClient);
    });

    builder.Entity<COD>(e =>
    {
      e.ToTable("CODs")
       .HasKey(x => x.IdCOD);
    });

    builder.Entity<TfPlan>(e =>
    {
      e.ToTable("TfPlans")
       .HasKey(x => x.IdTfPlan);
    });

    builder.Entity<SPRVlan>(e =>
    {
      e.ToTable("SPRVlans")
       .HasKey(x => x.IdVlan);
    });

    builder.Entity<Gate>(e => 
    {
      e.ToTable("Gates")
       .HasKey(x => x.Id);
    });

    builder.Entity<NetworkDevice>(e => 
    {
      e.ToTable("NetworkDevices")
       .HasKey(x => x.Id);

      e.HasMany(x => x.PortsOfNetworkDevice)
       .WithOne()
       .HasForeignKey(x => x.NetworkDeviceId);
    });

    builder.Entity<Port>(e => 
    {
      e.ToTable("Ports")
       .HasKey(x => x.Id);

      e.HasMany(x => x.ARPTableOfInterface)
       .WithOne()
       .HasForeignKey(x => x.PortId);

      e.HasMany(x => x.MACTable)
       .WithOne()
       .HasForeignKey(x => x.PortId);

      e.HasMany(x => x.NetworkTableOfInterface)
       .WithOne()
       .HasForeignKey(x => x.PortId);

      e.HasManyToMany(x => x.VLANs)
       .UsingJoinEntity<PortVlan>()
       .HasForeignKeys(p => p.Id, t => t.Id);
    });

    builder.Entity<ARPEntity>(e => 
    {
      e.ToTable("ARPEntities")
       .HasKey(x => x.Id);
    });

    builder.Entity<MACEntity>(e => 
    {
      e.ToTable("MACEntities")
       .HasKey(x => x.Id);
    });

    builder.Entity<TerminatedNetworkEntity>(e => 
    {
      e.ToTable("TerminatedNetworkEntities")
       .HasKey(x => x.Id);
    });

    builder.Entity<VLAN>(e => 
    {
      e.ToTable("VLANs")
       .HasKey(x => x.Id);
    });
  }
}
