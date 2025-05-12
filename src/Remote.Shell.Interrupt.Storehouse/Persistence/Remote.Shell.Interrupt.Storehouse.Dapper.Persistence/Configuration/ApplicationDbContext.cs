namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Configuration;

internal class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
  : DbContext(options)
{
  public DbSet<Client> Clients { get; set; }
  public DbSet<COD> CODs { get; set; }
  public DbSet<TfPlan> TfPlans { get; set; }
  public DbSet<SPRVlan> SPRVlans { get; set; }
  public DbSet<Gate> Gates { get; set; }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    builder.ApplyConfiguration(new ClientConfiguration());
    builder.ApplyConfiguration(new CODConfiguration());
    builder.ApplyConfiguration(new TfPlanConfiguration());
  }
}