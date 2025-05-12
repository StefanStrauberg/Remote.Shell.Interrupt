namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Configuration.ModelBuilders;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
  public void Configure(EntityTypeBuilder<Client> builder)
  {
    builder.ToTable("Clients");
    
    builder.HasKey(x => x.IdClient);

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
    builder.Property(x => x.IdClient)
           .HasColumnName("IdClient")
           .HasColumnType<int>("integer");
    builder.Property(x => x.Name)
           .HasColumnName("Name")
           .HasColumnType<string>("text");
    builder.Property(x => x.ContactC)
           .HasColumnName("ContactC")
           .HasColumnType<string?>("text");
    builder.Property(x => x.TelephoneC)
           .HasColumnName("TelephoneC")
           .HasColumnType<string?>("text");
    builder.Property(x => x.ContactT)
           .HasColumnName("ContactT")
           .HasColumnType<string?>("text");
    builder.Property(x => x.TelephoneT)
           .HasColumnName("TelephoneT")
           .HasColumnType<string?>("text");
    builder.Property(x => x.EmailC)
           .HasColumnName("EmailC")
           .HasColumnType<string?>("text");
    builder.Property(x => x.Working)
           .HasColumnName("Working")
           .HasColumnType<bool>("boolean");
    builder.Property(x => x.EmailT)
           .HasColumnName("EmailT")
           .HasColumnType<string?>("text");
    builder.Property(x => x.History)
           .HasColumnName("History")
           .HasColumnType<string?>("text");
    builder.Property(x => x.AntiDDOS)
           .HasColumnName("AntiDDOS")
           .HasColumnType<bool>("boolean");
    builder.Property(x => x.Id_COD)
           .HasColumnName("Id_COD")
           .HasColumnType<int>("integer");
    builder.Property(x => x.Id_TfPlan)
           .HasColumnName("Id_TfPlan")
           .HasColumnType<int?>("integer");
    builder.Property(x => x.Dat1)
           .HasColumnName("Dat1")
           .HasColumnType<DateTime?>("timestamptz");
    builder.Property(x => x.Dat2)
           .HasColumnName("Dat2")
           .HasColumnType<DateTime?>("timestamptz");
    builder.Property(x => x.Prim1)
           .HasColumnName("Prim1")
           .HasColumnType<string?>("text");
    builder.Property(x => x.Prim2)
           .HasColumnName("Prim2")
           .HasColumnType<string?>("text");
    builder.Property(x => x.Nik)
           .HasColumnName("Nik")
           .HasColumnType<string?>("text");
    builder.Property(x => x.NrDogovor)
           .HasColumnName("NrDogovor")
           .HasColumnType<string>("text")
           .HasDefaultValue("unknown");

    builder.HasOne<COD>(x => x.COD)
           .WithMany()
           .HasForeignKey(x => x.Id_COD);

    builder.HasOne<TfPlan>(x => x.TfPlan)
           .WithMany()
           .HasForeignKey(x => x.Id_TfPlan);

    builder.HasMany<SPRVlan>(x => x.SPRVlans)
           .WithOne()
           .HasForeignKey(x => x.IdClient);
  }
}
