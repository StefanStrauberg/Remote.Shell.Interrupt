namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Configuration.ModelBuilders;

public class CODConfiguration : IEntityTypeConfiguration<COD>
{
  public void Configure(EntityTypeBuilder<COD> builder)
  {
    builder.ToTable("CODs");

    builder.HasKey(x => x.IdCOD);

    builder.Property(x => x.Id)
           .HasColumnName("Id")
           .HasColumnType<Guid>("uuid")
           .HasDefaultValueSql("gen_random_uuid()");
    builder.Property(x => x.CreatedAt)
           .HasColumnName("CreatedAt")
           .HasColumnType<DateTime>("timestamptz")
           .HasDefaultValueSql("CURRENT_TIMESTAMP");
    builder.Property(x => x.UpdatedAt)
           .HasColumnName("UpdatedAt")
           .HasColumnType<DateTime?>("timestamptz");
    builder.Property(x => x.IdCOD)
           .HasColumnName("IdCOD")
           .HasColumnType<int>("integer");
    builder.Property(x => x.NameCOD)
           .HasColumnName("NameCOD")
           .HasColumnType<string>("text");
    builder.Property(x => x.Telephone)
           .HasColumnName("Telephone")
           .HasColumnType<string?>("text");
    builder.Property(x => x.Email1)
           .HasColumnName("Email1")
           .HasColumnType<string?>("text");
    builder.Property(x => x.Email2)
           .HasColumnName("Email2")
           .HasColumnType<string?>("text");
    builder.Property(x => x.Contact)
           .HasColumnName("Contact")
           .HasColumnType<string?>("text");
    builder.Property(x => x.Description)
           .HasColumnName("Description")
           .HasColumnType<string?>("text");
    builder.Property(x => x.Region)
           .HasColumnName("Region")
           .HasColumnType<string?>("text");
  }
}
