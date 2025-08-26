namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Configuration.ModelBuilders;

public class SPRVlanConfiguration : IEntityTypeConfiguration<SPRVlan>
{
       public void Configure(EntityTypeBuilder<SPRVlan> builder)
       {
              builder.ToTable("SPRVlans");

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
              builder.Property(x => x.UseClient)
                     .HasColumnName("UseClient")
                     .HasColumnType("boolean");
              builder.Property(x => x.UseCOD)
                     .HasColumnName("UseCOD")
                     .HasColumnType("boolean");
              builder.Property(x => x.IdVlan)
                     .HasColumnName("IdVlan")
                     .HasColumnType("integer");
              builder.Property(x => x.IdClient)
                     .HasColumnName("IdClient")
                     .HasColumnType("integer");
       }
}
