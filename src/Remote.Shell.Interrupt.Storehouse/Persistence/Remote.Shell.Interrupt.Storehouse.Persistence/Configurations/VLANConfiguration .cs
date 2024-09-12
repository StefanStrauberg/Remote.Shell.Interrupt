namespace Remote.Shell.Interrupt.Storehouse.Persistence.Configurations;

public class VLANConfiguration : IEntityTypeConfiguration<VLAN>
{
  public void Configure(EntityTypeBuilder<VLAN> builder)
  {
    builder.HasMany(v => v.Ports)
           .WithMany(p => p.VLANs);

    builder.HasMany(p => p.Ports)
           .WithMany(v => v.VLANs)
           .UsingEntity<Dictionary<string, object>>(
            "PortVlans",
            x => x.HasOne<Port>()
                  .WithMany()
                  .HasForeignKey("PortId")
                  .OnDelete(DeleteBehavior.Cascade),
            x => x.HasOne<VLAN>()
                  .WithMany()
                  .HasForeignKey("VLANId")
                  .OnDelete(DeleteBehavior.Cascade)
           );
  }
}
