namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Configuration.ModelBuilders;

public class WorkflowDefinitionConfiguration : IEntityTypeConfiguration<WorkflowDefinition>
{
  public void Configure(EntityTypeBuilder<WorkflowDefinition> builder)
  {
    builder.ToTable("WorkflowDefinitions");

    builder.HasKey(x => x.Id);

    // Duplicate-check target for CreateWorkflowCommandHandler, mirrors Gate.IPAddress.
    builder.HasIndex(x => x.Name)
           .IsUnique();

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
    builder.Property(x => x.Version)
           .HasColumnName("Version")
           .HasColumnType("integer");
    builder.Property(x => x.Status)
           .HasColumnName("Status")
           .HasColumnType("integer");
    builder.Property(x => x.StartNodeId)
           .HasColumnName("StartNodeId")
           .HasColumnType("uuid");

    // No DB-level FK from EdgeDefinition.FromNodeId/ToNodeId to NodeDefinition.Id, and
    // StartNodeId isn't FK-constrained either: graph-internal consistency (start node
    // exists, edges reference real nodes) is validated at the application layer
    // (BaseWorkflowValidator) - same as the in-memory prototype this ports from.
    builder.HasMany(x => x.Nodes)
           .WithOne()
           .HasForeignKey(x => x.WorkflowDefinitionId)
           .IsRequired()
           .OnDelete(DeleteBehavior.Cascade);

    builder.HasMany(x => x.Edges)
           .WithOne()
           .HasForeignKey(x => x.WorkflowDefinitionId)
           .IsRequired()
           .OnDelete(DeleteBehavior.Cascade);
  }
}
