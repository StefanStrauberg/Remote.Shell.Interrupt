namespace Remote.Shell.Interrupt.Storehouse.Persistence.Configurations;

public class ARPEntityConfiguration : IEntityTypeConfiguration<ARPEntity>
{
  public void Configure(EntityTypeBuilder<ARPEntity> builder)
  {
    builder.HasOne(a => a.Port)
        .WithMany(p => p.ARPTableOfInterface)
        .HasForeignKey(a => a.PortId);
  }
}