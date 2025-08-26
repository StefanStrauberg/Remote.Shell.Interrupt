namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Configuration.ModelBuilders;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
       public void Configure(EntityTypeBuilder<Client> builder)
       {
              builder.ToTable("Clients");

              builder.HasKey(x => x.Id);

              builder.HasIndex(x => x.IdClient).IsUnique();

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
              builder.Property(x => x.IdClient)
                     .HasColumnName("IdClient")
                     .HasColumnType("integer")
                     .ValueGeneratedNever();
              builder.Property(x => x.Name)
                     .HasColumnName("Name")
                     .HasColumnType("text");
              builder.Property(x => x.ContactC)
                     .HasColumnName("ContactC")
                     .HasColumnType("text");
              builder.Property(x => x.TelephoneC)
                     .HasColumnName("TelephoneC")
                     .HasColumnType("text");
              builder.Property(x => x.ContactT)
                     .HasColumnName("ContactT")
                     .HasColumnType("text");
              builder.Property(x => x.TelephoneT)
                     .HasColumnName("TelephoneT")
                     .HasColumnType("text");
              builder.Property(x => x.EmailC)
                     .HasColumnName("EmailC")
                     .HasColumnType("text");
              builder.Property(x => x.Working)
                     .HasColumnName("Working")
                     .HasColumnType("boolean");
              builder.Property(x => x.EmailT)
                     .HasColumnName("EmailT")
                     .HasColumnType("text");
              builder.Property(x => x.History)
                     .HasColumnName("History")
                     .HasColumnType("text");
              builder.Property(x => x.AntiDDOS)
                     .HasColumnName("AntiDDOS")
                     .HasColumnType("boolean");
              builder.Property(x => x.Id_COD)
                     .HasColumnName("Id_COD")
                     .HasColumnType("integer")
                     .ValueGeneratedNever();
              builder.Property(x => x.Id_TfPlan)
                     .HasColumnName("Id_TfPlan")
                     .HasColumnType("integer")
                     .ValueGeneratedNever();
              builder.Property(x => x.Dat1)
                     .HasColumnName("Dat1")
                     .HasColumnType("timestamptz");
              builder.Property(x => x.Dat2)
                     .HasColumnName("Dat2")
                     .HasColumnType("timestamptz");
              builder.Property(x => x.Prim1)
                     .HasColumnName("Prim1")
                     .HasColumnType("text");
              builder.Property(x => x.Prim2)
                     .HasColumnName("Prim2")
                     .HasColumnType("text");
              builder.Property(x => x.Nik)
                     .HasColumnName("Nik")
                     .HasColumnType("text");
              builder.Property(x => x.NrDogovor)
                     .HasColumnName("NrDogovor")
                     .HasColumnType("text")
                     .HasDefaultValue("unknown");

              builder.HasOne(x => x.COD)
                     .WithMany()
                     .HasPrincipalKey(c => c.IdCOD)
                     .HasForeignKey(c => c.Id_COD);

              builder.HasOne(x => x.TfPlan)
                     .WithMany()
                     .HasPrincipalKey(t => t.IdTfPlan)
                     .HasForeignKey(c => c.Id_TfPlan);

              builder.HasMany(x => x.SPRVlans)
                     .WithOne()
                     .HasPrincipalKey(c => c.IdClient)
                     .HasForeignKey(v => v.IdClient);
       }
}
