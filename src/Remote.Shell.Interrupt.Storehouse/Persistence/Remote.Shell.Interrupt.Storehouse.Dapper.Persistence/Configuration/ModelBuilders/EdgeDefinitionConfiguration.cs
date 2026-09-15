namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Configuration.ModelBuilders;

public class EdgeDefinitionConfiguration : IEntityTypeConfiguration<EdgeDefinition>
{
  public void Configure(EntityTypeBuilder<EdgeDefinition> builder)
  {
    builder.ToTable("WorkflowEdges");

    builder.HasKey(x => x.Id);

    builder.HasIndex(x => x.WorkflowDefinitionId);

    // No default: the caller assigns Id itself, same as NodeDefinition.
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
    builder.Property(x => x.FromNodeId)
           .HasColumnName("FromNodeId")
           .HasColumnType("uuid");
    builder.Property(x => x.ToNodeId)
           .HasColumnName("ToNodeId")
           .HasColumnType("uuid");
    builder.Property(x => x.Condition)
           .HasColumnName("Condition")
           .HasColumnType("text");
    builder.Property(x => x.Priority)
           .HasColumnName("Priority")
           .HasColumnType("integer");
  }
}
