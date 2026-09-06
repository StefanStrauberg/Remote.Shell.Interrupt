namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Configuration.ModelBuilders;

public class GateConfiguration : IEntityTypeConfiguration<Gate>
{
       public void Configure(EntityTypeBuilder<Gate> builder)
       {
              builder.ToTable("Gates");

              builder.HasKey(x => x.Id);

              // Non-unique for now: promote to a unique index after deduplicating existing rows.
              builder.HasIndex(x => x.IPAddress);

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
              builder.Property(x => x.Name)
                     .HasColumnName("Name")
                     .HasColumnType("text");
              builder.Property(x => x.IPAddress)
                     .HasColumnName("IPAddress")
                     .HasColumnType("bigint");
              builder.Property(x => x.Community)
                     .HasColumnName("Community")
                     .HasColumnType("text");
              builder.Property(x => x.TypeOfNetworkDevice)
                     .HasColumnName("TypeOfNetworkDevice")
                     .HasColumnType("integer");
       }
}
