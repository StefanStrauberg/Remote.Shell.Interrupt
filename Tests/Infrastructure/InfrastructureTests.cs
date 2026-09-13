using Microsoft.Extensions.Logging;
using Remote.Shell.Interrupt.Storehouse.AppLogger;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Logger;
using Remote.Shell.Interrupt.Storehouse.Infrastructure.SNMPCommandExecutor;
using Lextm.SharpSnmpLib;

namespace Tests.Infrastructure;

public class SNMPCommandExecutorSubtreeTests
{
    [Theory]
    [InlineData("1.3.6.1.2.1.2.2.1.1", "1.3.6.1.2.1.2.2.1.1", true)]          // exact match
    [InlineData("1.3.6.1.2.1.2.2.1.1.5", "1.3.6.1.2.1.2.2.1.1", true)]        // child arc
    [InlineData("1.3.6.1.2.1.2.2.1.1.5.1", "1.3.6.1.2.1.2.2.1.1", true)]      // deep child
    [InlineData("1.3.6.1.2.1.2.2.1.10.5", "1.3.6.1.2.1.2.2.1.1", false)]      // sibling arc "10"
    [InlineData("1.3.6.1.2.1.2.2.1.2", "1.3.6.1.2.1.2.2.1.1", false)]         // different arc
    [InlineData("1.3.6.1.2.1.2.2.1", "1.3.6.1.2.1.2.2.1.1", false)]           // parent of root
    public void IsWithinSubtree_ComparesOidArcsNotRawStrings(string candidate, string root, bool expected)
    {
        SNMPCommandExecutor.IsWithinSubtree(candidate, root).Should().Be(expected);
    }
}

public class SNMPCommandExecutorHexConversionTests
{
    [Fact]
    public void ConvertSnmpDataToHex_OctetString_ReturnsSpaceSeparatedHex()
    {
        var octetString = new OctetString("text");

        var result = SNMPCommandExecutor.ConvertSnmpDataToHex(octetString);

        result.Should().Be("74 65 78 74");
    }

    [Fact]
    public void ConvertSnmpDataToHex_EmptyOctetString_ReturnsEmptyString()
    {
        var result = SNMPCommandExecutor.ConvertSnmpDataToHex(new OctetString(Array.Empty<byte>()));

        result.Should().BeEmpty();
    }

    [Fact]
    public void ConvertSnmpDataToHex_NonOctetStringData_ThrowsArgumentException()
    {
        var act = () => SNMPCommandExecutor.ConvertSnmpDataToHex(new Integer32(42));

        act.Should().Throw<ArgumentException>()
           .WithMessage("Provided ISnmpData is not an OctetString.");
    }
}

/// <summary>
/// Minimal ILogger implementation capturing emitted log levels,
/// used because ILogger.Log is generic and awkward to verify with NSubstitute matchers.
/// </summary>
internal sealed class RecordingLogger : ILogger
{
    public string Category { get; init; } = string.Empty;
    public List<(LogLevel Level, string Category)> Entries { get; } = [];

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel,
                            EventId eventId,
                            TState state,
                            Exception? exception,
                            Func<TState, Exception?, string> formatter)
        => Entries.Add((logLevel, Category));
}

public class GenericAppLoggerTests
{
    sealed class Marker;


    [Fact]
    public void LogMethods_WrapUnderlyingLoggerAtAllLevels()
    {
        var factory = Substitute.For<ILoggerFactory>();
        var inner = new RecordingLogger();
        factory.CreateLogger(typeof(Marker)).Returns(inner);
        var logger = (IAppLogger<Marker>)new AppLogger<Marker>(factory);

        logger.LogInformation("value {Value}", 5);
        logger.LogWarning("warning {Value}", 1);
        logger.LogError("error {Value}", 1);

        inner.Entries.Select(e => e.Level)
                     .Should().ContainInOrder(LogLevel.Information, LogLevel.Warning, LogLevel.Error);
    }
}

public class NonGenericAppLoggerTests
{
    [Fact]
    public void LogMethods_CreateCategoryLoggerPerClassName()
    {
        var factory = Substitute.For<ILoggerFactory>();
        var inner = new RecordingLogger { Category = "MyClass" };
        factory.CreateLogger("MyClass").Returns(inner);
        var logger = new AppLogger(factory);

        logger.LogInformation("MyClass", "message {Value}", 1);
        logger.LogWarning("MyClass", "warning");
        logger.LogError("MyClass", "error");

        factory.Received(3).CreateLogger("MyClass");
        inner.Entries.Select(e => e.Level)
                     .Should().ContainInOrder(LogLevel.Information, LogLevel.Warning, LogLevel.Error);
    }

    [Fact]
    public void LogInformation_UsesProvidedClassName()
    {
        var factory = Substitute.For<ILoggerFactory>();
        var inner = new RecordingLogger();
        factory.CreateLogger(Arg.Any<string>()).Returns(inner);
        var logger = new AppLogger(factory);

        logger.LogInformation("CategoryA", "message");

        factory.Received().CreateLogger("CategoryA");
    }
}
