namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Configuration.ModelBuilders;

public class PortVlanConfiguration : IEntityTypeConfiguration<PortVlan>
{
  public void Configure(EntityTypeBuilder<PortVlan> builder)
  {
    builder.ToTable("PortVlans");

    builder.HasKey(x => x.Id);

    builder.Property(x => x.Id)
           .HasColumnName("Id")
           .HasColumnType("uuid")
           .HasDefaultValueSql("gen_random_uuid()");
    builder.Property(x => x.CreatedAt)
           .HasColumnName("CreatedAt")
           .HasColumnType("timestamptz")
           .HasDefaultValueSql("CURRENT_TIMESTAMP");
    builder.Property(x => x.UpdatedAt)
           .HasColumnName("UpdatedAt")
           .HasColumnType("timestamptz");
    builder.HasOne(x => x.Port)
           .WithMany()
           .HasForeignKey(x => x.PortId);
    builder.HasOne(x => x.VLAN)
           .WithMany()
           .HasForeignKey(x => x.VLANId);
  }
}
