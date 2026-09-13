namespace Tests.Specifications;

public class TfPlanSpecificationTests
{
    [Fact]
    public void IsAssignableFromGenericSpecification()
    {
        typeof(TfPlanSpecification).Should().BeAssignableTo<GenericSpecification<TfPlan>>();
    }
}
