namespace Tests.Specifications;

public class NetworkDeviceSpecificationTests
{
    [Fact]
    public void IsAssignableFromGenericSpecification()
    {
        typeof(NetworkDeviceSpecification).Should().BeAssignableTo<GenericSpecification<NetworkDevice>>();
    }
}
