namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Configuration.ModelBuilders;

public class MACEntityConfiguration : IEntityTypeConfiguration<MACEntity>
{
  public void Configure(EntityTypeBuilder<MACEntity> builder)
  {
    builder.ToTable("MACEntities");

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
    builder.Property(x => x.MACAddress)
           .HasColumnName("MACAddress")
           .HasColumnType<string>("text");
    builder.Property(x => x.PortId)
           .HasColumnName("PortId")
           .HasColumnType("uuid");
  }
}
