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
      // Первичный ключ
      entity.HasKey(e => e.IdClient);

      // Настройка таблицы (опционально)
      entity.ToTable("Clients");

      // Отношение с COD
      entity.HasOne(e => e.COD)
            .WithMany() // Если COD не имеет коллекции Clients
            .HasForeignKey(e => e.Id_COD)
            .IsRequired(false); // Необязательная связь

      // Отношение с TfPlan
      entity.HasOne(e => e.TfPlan)
            .WithMany() // Если TfPlan не имеет коллекции Clients
            .HasForeignKey(e => e.Id_TfPlan)
            .IsRequired(false); // Необязательная связь

      // Отношение с SPRVlan
      entity.HasMany(e => e.SPRVlans)
            .WithOne() // Если SPRVlan не имеет навигации к Client
            .HasForeignKey(e => e.IdClient)
            .IsRequired(false); // Необязательная связь
    });

    modelBuilder.Entity<COD>(entity =>
    {
      entity.HasKey(e => e.IdCOD);
      entity.ToTable("CODs"); // Опционально
    });

    modelBuilder.Entity<TfPlan>(entity =>
    {
      entity.HasKey(e => e.IdTfPlan);
      entity.ToTable("TfPlans"); // Опционально
    });

    modelBuilder.Entity<SPRVlan>(entity =>
    {
      entity.HasKey(e => e.IdVlan);
      entity.ToTable("SPRVlans"); // Опционально
    });

    modelBuilder.Entity<Gate>(entity => 
    {
      entity.HasKey(e => e.Id);
      entity.ToTable("Gates");
    });
  }
}
