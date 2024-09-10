namespace Remote.Shell.Interrupt.Storehouse.Persistence.Configurations;

public class PortVLANConfiguration : IEntityTypeConfiguration<PortVLAN>
{
  public void Configure(EntityTypeBuilder<PortVLAN> builder)
  {
    builder.HasKey(pv => new { pv.PortId, pv.VLANId });

    builder.HasOne(pv => pv.Port)
        .WithMany(p => p.PortVLANS)
        .HasForeignKey(pv => pv.PortId);

    builder.HasOne(pv => pv.VLAN)
        .WithMany(v => v.PortVLANS)
        .HasForeignKey(pv => pv.VLANId);
  }
}