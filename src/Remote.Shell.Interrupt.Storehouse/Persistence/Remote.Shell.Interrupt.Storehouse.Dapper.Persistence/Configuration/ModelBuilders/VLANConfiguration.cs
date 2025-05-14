namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Configuration.ModelBuilders;

public class VLANConfiguration : IEntityTypeConfiguration<VLAN>
{
  public void Configure(EntityTypeBuilder<VLAN> builder)
  {
    builder.ToTable("VLANs");

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
    builder.Property(x => x.VLANTag)
           .HasColumnName("VLANTag")
           .HasColumnType("integer");
    builder.Property(x => x.VLANName)
           .HasColumnName("VLANName")
           .HasColumnType("text");

    builder.HasMany(x => x.Ports)
           .WithMany(x => x.VLANs);
  }
}
