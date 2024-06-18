namespace Remote.Shell.Interrupt.SNMPExecutor.Tests.SNMP;

[TestFixture]
public class SNMPGetCommandHandlerTests
{
    readonly Mock<ISNMPCommandExecutor> _mockRepo;
    readonly SNMPGetCommand _request;
    readonly SNMPGetCommandHandler _handler;

    public SNMPGetCommandHandlerTests()
        => (_mockRepo, _request, _handler)
        = (_mockRepo = MockSNMPCommandExecutor.GetSNMPCommandExecutor(),
           _request = new SNMPGetCommand("192.168.1.1", "public", "1.3.6.1.2.1.1.1.0"),
           _handler = new SNMPGetCommandHandler(_mockRepo.Object));

    [Test]
    public async Task Handle_ValidRequest_ReturnsJObject()
    {
        // Act
        var result = await (_handler as IRequestHandler<SNMPGetCommand, Info>).Handle(_request, default);

        // Assert
        result.ShouldNotBeNull();
    }
}