namespace Remote.Shell.Interrupt.Storehouse.Persistence.Configurations;

public class VLANConfiguration : IEntityTypeConfiguration<VLAN>
{
  public void Configure(EntityTypeBuilder<VLAN> builder)
  {
    builder.HasMany(v => v.PortVLANS)
        .WithOne(pv => pv.VLAN)
        .HasForeignKey(pv => pv.VLANId);
  }
}
