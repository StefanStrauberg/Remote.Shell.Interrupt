namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Configuration.ModelBuilders;

public class NodeDefinitionConfiguration : IEntityTypeConfiguration<NodeDefinition>
{
  public void Configure(EntityTypeBuilder<NodeDefinition> builder)
  {
    builder.ToTable("WorkflowNodes");

    builder.HasKey(x => x.Id);

    builder.HasIndex(x => x.WorkflowDefinitionId);

    // No default: the caller always assigns Id itself so that edges in the same
    // create/update payload can reference it.
    builder.Property(x => x.Id)
           .HasColumnName("Id")
           .HasColumnType("uuid");
    builder.Property(x => x.CreatedAt)
           .HasColumnName("CreatedAt")
           .HasColumnType("timestamptz")
           .HasDefaultValueSql("CURRENT_TIMESTAMP");
    builder.Property(x => x.UpdatedAt)
           .HasColumnName("UpdatedAt")
           .HasColumnType("timestamptz");
    builder.Property(x => x.WorkflowDefinitionId)
           .HasColumnName("WorkflowDefinitionId")
           .HasColumnType("uuid");
    builder.Property(x => x.Type)
           .HasColumnName("Type")
           .HasColumnType("text");
    builder.Property(x => x.Name)
           .HasColumnName("Name")
           .HasColumnType("text");
    builder.Property(x => x.Key)
           .HasColumnName("Key")
           .HasColumnType("text");
    builder.Property(x => x.PositionX)
           .HasColumnName("PositionX")
           .HasColumnType("double precision");
    builder.Property(x => x.PositionY)
           .HasColumnName("PositionY")
           .HasColumnType("double precision");

    builder.Property(x => x.Config)
           .HasColumnName("Config")
           .HasColumnType("jsonb")
           .HasConversion(JsonDictionaryConverter.Converter)
           .Metadata.SetValueComparer(JsonDictionaryConverter.Comparer);
  }
}
