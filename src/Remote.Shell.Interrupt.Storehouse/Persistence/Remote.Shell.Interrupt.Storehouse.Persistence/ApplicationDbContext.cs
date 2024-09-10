namespace Remote.Shell.Interrupt.Storehouse.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
  : DbContext(options)
{
  public DbSet<Assignment> Assignments { get; set; }
  public DbSet<BusinessRule> BusinessRules { get; set; }
  public DbSet<NetworkDevice> NetworkDevices { get; set; }
  public DbSet<PortVLAN> PortVLANS { get; set; }
  public DbSet<Port> Ports { get; set; }
  public DbSet<VLAN> VLANs { get; set; }
  public DbSet<ARPEntity> ARPEntities { get; set; }
  public DbSet<TerminatedNetworkEntity> TerminatedNetworkEntities { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfiguration(new PortConfiguration());
    modelBuilder.ApplyConfiguration(new PortVLANConfiguration());
    modelBuilder.ApplyConfiguration(new VLANConfiguration());
    modelBuilder.ApplyConfiguration(new BusinessRuleConfiguration());
    modelBuilder.ApplyConfiguration(new AssignmentConfiguration());
    modelBuilder.ApplyConfiguration(new NetworkDeviceConfiguration());
    modelBuilder.ApplyConfiguration(new ARPEntityConfiguration());
    modelBuilder.ApplyConfiguration(new TerminatedNetworkEntityConfiguration());

    base.OnModelCreating(modelBuilder);
  }

  public override int SaveChanges()
  {
    SetTimestamps();
    return base.SaveChanges();
  }

  public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    SetTimestamps();
    return base.SaveChangesAsync(cancellationToken);
  }

  private void SetTimestamps()
  {
    var entries = ChangeTracker.Entries<BaseEntity>();

    foreach (var entry in entries)
    {
      if (entry.State == EntityState.Added)
      {
        entry.Entity.Id = Guid.NewGuid();
        entry.Entity.CreatedAt = DateTime.UtcNow;
      }

      if (entry.State == EntityState.Modified)
      {
        entry.Entity.UpdatedAt = DateTime.UtcNow;
      }
    }
  }
}
