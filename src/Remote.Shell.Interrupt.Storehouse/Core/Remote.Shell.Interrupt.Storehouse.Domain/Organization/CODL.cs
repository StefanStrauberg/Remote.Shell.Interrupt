namespace Remote.Shell.Interrupt.Storehouse.Domain.Organization;

public class CODL : BaseEntity
{
  public int IdCOD { get; set; }
  public string NameCOD { get; set; } = string.Empty;
  public string? Telephone { get; set; } = string.Empty;
  public string? Email1 { get; set; } = string.Empty;
  public string? Email2 { get; set; } = string.Empty;
  public string? Contact { get; set; } = string.Empty;
  public string? Description { get; set; } = string.Empty;
  public string? Region { get; set; } = string.Empty;
}
