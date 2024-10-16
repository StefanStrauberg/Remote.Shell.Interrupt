namespace Remote.Shell.Interrupt.Storehouse.Persistence.Configurations;

public class TerminatedNetworkEntityConfiguration : IEntityTypeConfiguration<TerminatedNetworkEntity>
{
  public void Configure(EntityTypeBuilder<TerminatedNetworkEntity> builder)
  {
    builder.HasOne(t => t.Port)
        .WithMany(p => p.NetworkTableOfInterface)
        .HasForeignKey(t => t.PortId);
  }
}