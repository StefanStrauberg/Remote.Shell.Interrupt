namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Configuration.ModelBuilders;

public class PortConfiguration : IEntityTypeConfiguration<Port>
{
       public void Configure(EntityTypeBuilder<Port> builder)
       {
              builder.ToTable("Ports");

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
              builder.Property(x => x.InterfaceNumber)
                     .HasColumnName("InterfaceNumber")
                     .HasColumnType("integer");
              builder.Property(x => x.InterfaceName)
                     .HasColumnName("InterfaceName")
                     .HasColumnType("text");
              builder.Property(x => x.InterfaceType)
                     .HasColumnName("InterfaceType")
                     .HasColumnType("integer");
              builder.Property(x => x.InterfaceStatus)
                     .HasColumnName("InterfaceStatus")
                     .HasColumnType("integer");
              builder.Property(x => x.InterfaceSpeed)
                     .HasColumnName("InterfaceSpeed")
                     .HasColumnType("bigint");
              builder.Property(x => x.MACAddress)
                     .HasColumnName("MACAddress")
                     .HasColumnType("text");
              builder.Property(x => x.Description)
                     .HasColumnName("Description")
                     .HasColumnType("text");
              builder.Property(x => x.ParentId)
                     .HasColumnName("ParentId")
                     .HasColumnType("uuid");
              builder.Property(x => x.NetworkDeviceId)
                     .HasColumnName("NetworkDeviceId")
                     .HasColumnType("uuid");

              builder.HasMany(x => x.ARPTableOfInterface)
                     .WithOne()
                     .HasForeignKey(x => x.PortId);
              builder.HasMany(x => x.MACTable)
                     .WithOne()
                     .HasForeignKey(x => x.PortId);
              builder.HasMany(x => x.NetworkTableOfInterface)
                     .WithOne()
                     .HasForeignKey(x => x.PortId);

              builder.HasMany(x => x.AggregatedPorts)
                     .WithOne()
                     .HasForeignKey(x => x.ParentId);
       }
}
