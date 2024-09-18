namespace Remote.Shell.Interrupt.Storehouse.Persistence.Configurations;

public class PortConfiguration : IEntityTypeConfiguration<Port>
{
       public void Configure(EntityTypeBuilder<Port> builder)
       {
              builder.HasMany(p => p.ARPTableOfInterface)
                         .WithOne(a => a.Port)
                         .HasForeignKey(a => a.PortId);

              builder.HasMany(p => p.NetworkTableOfInterface)
                     .WithOne(t => t.Port)
                     .HasForeignKey(t => t.PortId);

              builder.HasMany(p => p.VLANs)
                     .WithMany(v => v.Ports)
                     .UsingEntity<Dictionary<string, object>>(
                      "PortVlans",
                      x => x.HasOne<VLAN>()
                            .WithMany()
                            .HasForeignKey("VLANId")
                            .OnDelete(DeleteBehavior.Cascade),
                      x => x.HasOne<Port>()
                            .WithMany()
                            .HasForeignKey("PortId")
                            .OnDelete(DeleteBehavior.Cascade)
                     );
       }
}
