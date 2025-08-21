namespace Tests.Models;

public class TestEntity : BaseEntity
{
  public string Name { get; set; } = string.Empty;
  public int Age { get; set; }
  public Address Address { get; set; } = new();
  public List<ChildEntity> Children { get; set; } = [];
}
