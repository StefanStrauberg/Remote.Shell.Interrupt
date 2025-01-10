namespace Remote.Shell.Interrupt.Storehouse.Domain.Organization;

public class SPRVlanL : BaseEntity
{
  public int IdVlan { get; set; }
  public Guid IdClient { get; set; }
  public bool UseClient { get; set; }
  public bool UseCOD { get; set; }
}