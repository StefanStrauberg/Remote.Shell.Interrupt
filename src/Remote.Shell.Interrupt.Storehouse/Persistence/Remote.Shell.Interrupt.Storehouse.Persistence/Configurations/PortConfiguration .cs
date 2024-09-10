namespace Remote.Shell.Interrupt.Storehouse.Persistence.Configurations;

public class PortConfiguration : IEntityTypeConfiguration<Port>
{
  public void Configure(EntityTypeBuilder<Port> builder)
  {
    builder.HasMany(p => p.ARPTableOfPort)
               .WithOne(a => a.Port)
               .HasForeignKey(a => a.PortId);

    builder.HasMany(p => p.NetworkTableOfPort)
           .WithOne(t => t.Port)
           .HasForeignKey(t => t.PortId);

    builder.HasMany(p => p.PortVLANS)
        .WithOne(pv => pv.Port)
        .HasForeignKey(pv => pv.PortId);
  }
}
