using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Remote.Shell.Interrupt.Storehouse.API.Controllers;
using Remote.Shell.Interrupt.Storehouse.API.Entities;
using Remote.Shell.Interrupt.Storehouse.Application.DTOs.Gates;
using Remote.Shell.Interrupt.Storehouse.Application.DTOs.NetworkDevices;
using Remote.Shell.Interrupt.Storehouse.Application.DTOs.Organizations;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Commands.CreateGate;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Commands.DeleteGate;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetClientsByFilter;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetClientById;
using Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetNetworkDeviceById;
using Remote.Shell.Interrupt.Storehouse.Application.Features.SNMPExecutor.Commands.SNMPGet;
using Remote.Shell.Interrupt.Storehouse.Application.Features.SNMPExecutor.Commands.SNMPWalk;
using Remote.Shell.Interrupt.Storehouse.Application.Models.Request;
using Remote.Shell.Interrupt.Storehouse.Application.Models.Response;
using Remote.Shell.Interrupt.Storehouse.Domain.SNMP;

namespace Tests.Api;

public class ClientsControllerTests
{
    readonly ISender _sender = Substitute.For<ISender>();
    readonly ClientsController _controller;

    public ClientsControllerTests()
    {
        _controller = new ClientsController(_sender)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };
    }

    [Fact]
    public async Task GetClientsByFilter_SetsPaginationHeaderAndReturnsOk()
    {
        var paged = PagedList<ShortClientDTO>.Create([new ShortClientDTO { Name = "Alpha" }],
                                                     12,
                                                     new PaginationContext(2, 5));
        _sender.Send(Arg.Any<GetClientsByFilterQuery>(), Arg.Any<CancellationToken>())
               .Returns(paged);

        var result = await _controller.GetClientsByFilter(new RequestParameters(), CancellationToken.None);

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeSameAs(paged);
        _controller.Response.Headers["X-Pagination"].ToString().Should().Contain("\"TotalCount\":12");
        _controller.Response.Headers["X-Pagination"].ToString().Should().Contain("\"CurrentPage\":2");
        await _sender.Received().Send(Arg.Any<GetClientsByFilterQuery>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetClientById_DispatchesQueryAndReturnsOk()
    {
        var id = Guid.NewGuid();
        var dto = new DetailClientDTO { Name = "Alpha" };
        _sender.Send(Arg.Any<GetClientByIdQuery>(), Arg.Any<CancellationToken>()).Returns(dto);

        var result = await _controller.GetClientById(id, CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>();
        await _sender.Received().Send(
            Arg.Is<GetClientByIdQuery>(q => q.Id == id), Arg.Any<CancellationToken>());
    }
}

public class GatesControllerTests
{
    readonly ISender _sender = Substitute.For<ISender>();
    readonly GatesController _controller;

    public GatesControllerTests()
    {
        _controller = new GatesController(_sender)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };
    }

    [Fact]
    public async Task CreateGate_DispatchesCommandWithDto()
    {
        var dto = new CreateGateDTO { Name = "gw", Community = "public", IPAddress = "10.0.0.1", TypeOfNetworkDevice = "Cisco" };
        CreateGateCommand? received = null;
        _sender.Send(Arg.Do<CreateGateCommand>(c => received = c), Arg.Any<CancellationToken>())
               .Returns(Unit.Value);

        var result = await _controller.CreateGate(dto, CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>();
        received.Should().NotBeNull();
        received!.GateDto.Name.Should().Be("gw");
    }

    [Fact]
    public async Task DeleteGateById_DispatchesCommandWithRouteId()
    {
        var id = Guid.NewGuid();
        _sender.Send(Arg.Any<DeleteGateCommand>(), Arg.Any<CancellationToken>()).Returns(Unit.Value);

        var result = await _controller.DeleteGateById(id, CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>();
        await _sender.Received().Send(Arg.Is<DeleteGateCommand>(c => c.Id == id), Arg.Any<CancellationToken>());
    }
}

public class NetworkDevicesControllerTests
{
    readonly ISender _sender = Substitute.For<ISender>();
    readonly NetworkDevicesController _controller;

    public NetworkDevicesControllerTests()
    {
        _controller = new NetworkDevicesController(_sender)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };
    }

    [Fact]
    public async Task GetNetworkDeviceById_DispatchesQuery()
    {
        var id = Guid.NewGuid();
        _sender.Send(Arg.Any<GetNetworkDeviceByIdQuery>(), Arg.Any<CancellationToken>())
               .Returns(new NetworkDeviceDTO { NetworkDeviceName = "gw" });

        var result = await _controller.GetNetworkDeviceById(id, CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>();
        await _sender.Received().Send(Arg.Is<GetNetworkDeviceByIdQuery>(q => q.Id == id), Arg.Any<CancellationToken>());
    }
}

public class SNMPExecutorControllerTests
{
    readonly ISender _sender = Substitute.For<ISender>();
    readonly SNMPExecutorController _controller;

    public SNMPExecutorControllerTests()
    {
        _controller = new SNMPExecutorController(_sender)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };
    }

    [Fact]
    public async Task Get_DispatchesSnmpGetCommand()
    {
        var command = new SNMPGetCommand("10.0.0.1", "public", "1.3.6.1.2.1.1.1.0");
        var response = new SNMPResponse { OID = "1.3.6.1.2.1.1.1.0", Data = "OS" };
        _sender.Send(Arg.Any<SNMPGetCommand>(), Arg.Any<CancellationToken>()).Returns(response);

        var result = await _controller.Get(command, CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>();
        await _sender.Received().Send(command, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Walk_DispatchesSnmpWalkCommand()
    {
        var command = new SNMPWalkCommand("10.0.0.1", "public", "1.3.6.1");
        _sender.Send(Arg.Any<SNMPWalkCommand>(), Arg.Any<CancellationToken>())
               .Returns([new SNMPResponse { OID = "1.3.6.1.1", Data = "1" }]);

        var result = await _controller.Walk(command, CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>();
        await _sender.Received().Send(command, Arg.Any<CancellationToken>());
    }
}

public class BuggyControllerTests
{
    readonly BuggyController _controller = new(Substitute.For<ISender>());

    [Fact]
    public void GetNotFound_Returns404Payload()
    {
        var result = _controller.GetNotFound();

        var objectResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(404);
        objectResult.Value.Should().BeOfType<ApiErrorResponse>()
                    .Which.Status.Should().Be(404);
    }

    [Fact]
    public void GetBadRequest_Returns400Payload()
    {
        var result = _controller.GetBadRequest();

        var objectResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(400);
        objectResult.Value.Should().BeOfType<ApiErrorResponse>()
                    .Which.Status.Should().Be(400);
    }

    [Fact]
    public void GetUnauthorized_Returns401Payload()
    {
        var result = _controller.GetUnauthorized();

        var objectResult = result.Should().BeOfType<UnauthorizedObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(401);
        objectResult.Value.Should().BeOfType<ApiErrorResponse>()
                    .Which.Status.Should().Be(401);
    }

    [Fact]
    public void GetServerError_ThrowsUnhandledException()
    {
        var act = () => _controller.GetServerError();

        act.Should().Throw<Exception>().WithMessage("Testing a server error");
    }
}
