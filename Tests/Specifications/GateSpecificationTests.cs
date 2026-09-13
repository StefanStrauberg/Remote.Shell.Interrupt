namespace Tests.Specifications;

public class GateSpecificationTests
{
    [Fact]
    public void IsAssignableFromGenericSpecification()
    {
        typeof(GateSpecification).Should().BeAssignableTo<GenericSpecification<Gate>>();
    }
}
