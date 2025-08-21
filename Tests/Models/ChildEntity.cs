namespace Tests.Models;

public class ChildEntity
{
  public string ChildName { get; set; } = string.Empty;
  public List<ToyEntity> Toys { get; set; } = [];
}
