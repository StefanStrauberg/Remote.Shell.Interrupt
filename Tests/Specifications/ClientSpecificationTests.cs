namespace Tests.Specifications;

public class ClientSpecificationTests
{
    [Fact]
    public void IsAssignableFromGenericSpecification()
    {
        typeof(ClientSpecification).Should().BeAssignableTo<GenericSpecification<Client>>();
    }
}
