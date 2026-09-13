namespace Tests.Exceptions;

public class ExceptionTests
{
    [Fact]
    public void ValidationException_HasErrorsDictionary()
    {
        var errors = new Dictionary<string, string[]>
        {
            { "Name", ["Name is required"] },
            { "Email", ["Email is invalid"] }
        };
        var ex = new ValidationException(errors);
        ex.ErrorsDictionary.Should().ContainKey("Name");
        ex.ErrorsDictionary.Should().ContainKey("Email");
        ex.ErrorsDictionary["Name"].Should().Contain("Name is required");
    }

    [Fact]
    public void EntityNotFoundException_HasCorrectProperties()
    {
        var ex = new EntityNotFoundException(typeof(Client), "Id");
        ex.Message.Should().Contain("Client");
    }

    [Fact]
    public void EntityAlreadyExists_HasCorrectProperties()
    {
        var ex = new EntityAlreadyExists(typeof(Client), "Name");
        ex.Message.Should().Contain("Client");
    }

    [Fact]
    public void SNMPBadRequestException_HasCorrectProperties()
    {
        var ex = new SNMPBadRequestException("SNMP error");
        ex.Message.Should().Be("SNMP error");
    }

    [Fact]
    public void SNMPBadRequestException_WithInnerException_PreservesInner()
    {
        var inner = new Exception("inner");
        var ex = new SNMPBadRequestException("SNMP error", inner);
        ex.InnerException.Should().Be(inner);
    }

    [Fact]
    public void SNMPBadRequestException_InheritsFromBadRequestException()
    {
        var ex = new SNMPBadRequestException("test");
        ex.Should().BeAssignableTo<BadRequestException>();
    }

    [Fact]
    public void BadRequestException_TitleIsBadRequest()
    {
        var ex = new SNMPBadRequestException("test");
        ex.Title.Should().Be("Bad Request");
    }
}
