namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Users;

/// <summary>
/// Read model for an application account, as listed/managed on the admin Users page.
/// </summary>
public class UserDTO
{
  public Guid Id { get; set; }
  public string Email { get; set; } = string.Empty;
  public string? FullName { get; set; }
  public List<string> Roles { get; set; } = [];
  public bool IsActive { get; set; }
  public DateTime CreatedAtUtc { get; set; }
}
