namespace Remote.Shell.Interrupt.Storehouse.Domain.Organization;

public class Organization : BaseEntity
{
  public int IdClient { get; set; }
  public string Name { get; set; } = string.Empty;
  public string Contact { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public string TPlan { get; set; } = string.Empty;
  public int[] VLANTags { get; set; } = [];
}
