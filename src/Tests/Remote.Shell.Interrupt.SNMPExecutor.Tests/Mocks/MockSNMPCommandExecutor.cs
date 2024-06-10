namespace Remote.Shell.Interrupt.SNMPExecutor.Tests.Mocks;

internal static class MockSNMPCommandExecutor
{
  internal static Mock<ISNMPCommandExecutor> GetSNMPCommandExecutor()
  {
    var mockRepo = new Mock<ISNMPCommandExecutor>();

    mockRepo.Setup(x => x.WalkCommand(It.IsAny<string>(),
                                      It.IsAny<string>(),
                                      It.IsAny<string>(),
                                      It.IsAny<CancellationToken>()))
      .ReturnsAsync((string host,
                     string community,
                     string oid,
                     CancellationToken cancellationToken) =>
      {
        return [];
      });

    mockRepo.Setup(x => x.GetCommand(It.IsAny<string>(),
                                     It.IsAny<string>(),
                                     It.IsAny<string>(),
                                     It.IsAny<CancellationToken>()))
      .ReturnsAsync((string host,
                     string community,
                     string oid,
                     CancellationToken cancellationToken) =>
      {
        return [];
      });

    return mockRepo;
  }
}
