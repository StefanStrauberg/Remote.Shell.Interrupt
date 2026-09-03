namespace Tests.Specifications;

public class SPRVlanSpecificationTests
{
    [Fact]
    public void IsAssignableFromGenericSpecification()
    {
        typeof(SPRVlanSpecification).Should().BeAssignableTo<GenericSpecification<SPRVlan>>();
    }
}
