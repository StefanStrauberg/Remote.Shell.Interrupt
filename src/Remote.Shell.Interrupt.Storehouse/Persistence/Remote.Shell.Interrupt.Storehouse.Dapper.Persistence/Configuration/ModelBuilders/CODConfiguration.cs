namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Configuration.ModelBuilders;

public class CODConfiguration : IEntityTypeConfiguration<COD>
{
  public void Configure(EntityTypeBuilder<COD> builder)
  {
    builder.ToTable("CODs");

    builder.HasKey(x => x.IdCOD);

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
    builder.Property(x => x.IdCOD)
           .HasColumnName("IdCOD")
           .HasColumnType("integer");
    builder.Property(x => x.NameCOD)
           .HasColumnName("NameCOD")
           .HasColumnType("text");
    builder.Property(x => x.Telephone)
           .HasColumnName("Telephone")
           .HasColumnType("text");
    builder.Property(x => x.Email1)
           .HasColumnName("Email1")
           .HasColumnType("text");
    builder.Property(x => x.Email2)
           .HasColumnName("Email2")
           .HasColumnType("text");
    builder.Property(x => x.Contact)
           .HasColumnName("Contact")
           .HasColumnType("text");
    builder.Property(x => x.Description)
           .HasColumnName("Description")
           .HasColumnType("text");
    builder.Property(x => x.Region)
           .HasColumnName("Region")
           .HasColumnType("text");
  }
}
