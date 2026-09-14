using Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Identity;

namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Configuration.ModelBuilders;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
       public void Configure(EntityTypeBuilder<RefreshToken> builder)
       {
              builder.ToTable("RefreshTokens");

              builder.HasKey(x => x.Id);

              builder.HasIndex(x => x.TokenHash).IsUnique();
              builder.HasIndex(x => x.UserId);

              builder.Property(x => x.Id)
                     .HasColumnName("Id")
                     .HasColumnType("uuid")
                     .HasDefaultValueSql("gen_random_uuid()");
              builder.Property(x => x.UserId)
                     .HasColumnName("UserId")
                     .HasColumnType("uuid");
              builder.Property(x => x.TokenHash)
                     .HasColumnName("TokenHash")
                     .HasColumnType("text")
                     .IsRequired();
              builder.Property(x => x.CreatedAtUtc)
                     .HasColumnName("CreatedAtUtc")
                     .HasColumnType("timestamptz");
              builder.Property(x => x.ExpiresAtUtc)
                     .HasColumnName("ExpiresAtUtc")
                     .HasColumnType("timestamptz");
              builder.Property(x => x.RevokedAtUtc)
                     .HasColumnName("RevokedAtUtc")
                     .HasColumnType("timestamptz");
              builder.Property(x => x.ReplacedByTokenHash)
                     .HasColumnName("ReplacedByTokenHash")
                     .HasColumnType("text");

              builder.HasOne<ApplicationUser>()
                     .WithMany()
                     .HasForeignKey(x => x.UserId)
                     .OnDelete(DeleteBehavior.Cascade);
       }
}
