namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Configuration.ModelBuilders;

public class PortVlanConfiguration : IEntityTypeConfiguration<PortVlan>
{
  public void Configure(EntityTypeBuilder<PortVlan> builder)
  {
    builder.ToTable("PortVlans");

    builder.HasKey(x => new { x.PortId, x.VLANId });

    builder.HasOne(x => x.Port)
           .WithMany()
           .HasForeignKey(x => x.PortId);

    builder.HasOne(x => x.VLAN)
           .WithMany()
           .HasForeignKey(x => x.VLANId);
  }
}
