namespace Tests.Models;

public class Person
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public int Age { get; set; }
  public Address Address { get; set; } = new();
  public List<NickName> NickNames { get; set; } = [];
}
