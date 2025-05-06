namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Configuration;

internal class ApplicationDbContext : DbContext
{
  public DbSet<Client> Clients { get; }
  public DbSet<COD> CODs { get; }
  public DbSet<TfPlan> TfPlans { get; }
  public DbSet<SPRVlan> SPRVlans { get; }
  public DbSet<Gate> Gates { get; }

  public ApplicationDbContext()
  {
    Clients = Set<Client>();
    CODs = Set<COD>();
    TfPlans = Set<TfPlan>();
    SPRVlans = Set<SPRVlan>();
    Gates = Set<Gate>();
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Client>(entity =>
    {
      entity.ToTable("Clients")
            .HasKey(x => x.IdClient);

      entity.HasOne(x => x.COD)
            .WithMany()
            .HasForeignKey(x => x.Id_COD);

      entity.HasOne(x => x.TfPlan)
            .WithMany()
            .HasForeignKey(x => x.Id_TfPlan);

      entity.HasMany(x => x.SPRVlans)
            .WithOne()
            .HasForeignKey(x => x.IdClient);
    });

    modelBuilder.Entity<COD>(entity =>
    {
      entity.ToTable("CODs")
            .HasKey(x => x.IdCOD);
    });

    modelBuilder.Entity<TfPlan>(entity =>
    {
      entity.ToTable("TfPlans")
            .HasKey(x => x.IdTfPlan);
    });

    modelBuilder.Entity<SPRVlan>(entity =>
    {
      entity.ToTable("SPRVlans")
            .HasKey(x => x.IdVlan);
    });

    modelBuilder.Entity<Gate>(entity => 
    {
      entity.ToTable("Gates")
            .HasKey(x => x.Id);
    });

    modelBuilder.Entity<NetworkDevice>(entity => 
    {
      entity.ToTable("NetworkDevices")
            .HasKey(x => x.Id);

      entity.HasMany(x => x.PortsOfNetworkDevice)
            .WithOne()
            .HasForeignKey(x => x.NetworkDeviceId);
    });

    modelBuilder.Entity<Port>(entity => 
    {
      entity.ToTable("Ports")
            .HasKey(x => x.Id);

      entity.HasMany(x => x.ARPTableOfInterface)
            .WithOne()
            .HasForeignKey(x => x.PortId);

      entity.HasMany(x => x.MACTable)
            .WithOne()
            .HasForeignKey(x => x.PortId);

      entity.HasMany(x => x.NetworkTableOfInterface)
            .WithOne()
            .HasForeignKey(x => x.PortId);

      entity.HasManyToMany(x => x.VLANs)
            .UsingJoinEntity<PortVlan>()
            .HasForeignKeys(p => p.Id, t => t.Id);
    });

    modelBuilder.Entity<ARPEntity>(entity => 
    {
      entity.ToTable("ARPEntities")
            .HasKey(x => x.Id);
    });

    modelBuilder.Entity<MACEntity>(entity => 
    {
      entity.ToTable("MACEntities")
            .HasKey(x => x.Id);
    });

    modelBuilder.Entity<TerminatedNetworkEntity>(entity => 
    {
      entity.ToTable("TerminatedNetworkEntities")
            .HasKey(x => x.Id);
    });

    modelBuilder.Entity<VLAN>(entity => 
    {
      entity.ToTable("VLANs")
            .HasKey(x => x.Id);
    });
  }
}
