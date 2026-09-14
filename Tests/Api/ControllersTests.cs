using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Remote.Shell.Interrupt.Storehouse.API.Controllers;
using Remote.Shell.Interrupt.Storehouse.API.Entities;
using Remote.Shell.Interrupt.Storehouse.Application.DTOs.Gates;
using Remote.Shell.Interrupt.Storehouse.Application.DTOs.NetworkDevices;
using Remote.Shell.Interrupt.Storehouse.Application.DTOs.Organizations;
using Remote.Shell.Interrupt.Storehouse.Application.DTOs.TfPlans;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Commands.CreateGate;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Commands.DeleteGate;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Commands.UpdateGate;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Queries.GetGateById;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Queries.GetGatesByFilter;
using Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Commands.CreateNetworkDevice;
using Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Commands.DeleteNetworkDeviceById;
using Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Commands.DeleteNetworkDevices;
using Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetNetworkDeviceByVlanTag;
using Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetNetworkDevicesByFilter;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Commands.DeleteClientsLocalDb;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Commands.UpdateClientsLocalDb;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetClientsByFilter;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetClientById;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetClientsByVlanTag;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetClientsWithChildrenByFilter;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetClientWithChildrenByFilter;
using Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetNetworkDeviceById;
using Remote.Shell.Interrupt.Storehouse.Application.Features.SNMPExecutor.Commands.SNMPGet;
using Remote.Shell.Interrupt.Storehouse.Application.Features.SNMPExecutor.Commands.SNMPWalk;
using Remote.Shell.Interrupt.Storehouse.Application.Features.SPRVlans.Queries.GetSPRVlansByFilter;
using Remote.Shell.Interrupt.Storehouse.Application.Features.TfPlans.Queries.GetTfPlansByFilter;
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

    [Fact]
    public async Task GetClientsWithChildrenByFilter_SetsPaginationHeaderAndReturnsOk()
    {
        var paged = PagedList<DetailClientDTO>.Create([new DetailClientDTO { Name = "Alpha" }],
                                                       3,
                                                       new PaginationContext(1, 10));
        _sender.Send(Arg.Any<GetClientsWithChildrenByFilterQuery>(), Arg.Any<CancellationToken>())
               .Returns(paged);

        var result = await _controller.GetClientsWithChildrenByFilter(new RequestParameters(), CancellationToken.None);

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeSameAs(paged);
        _controller.Response.Headers["X-Pagination"].ToString().Should().Contain("\"TotalCount\":3");
        await _sender.Received().Send(Arg.Any<GetClientsWithChildrenByFilterQuery>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetClientWithChildrenByFilter_DispatchesQueryAndReturnsOk()
    {
        var dto = new DetailClientDTO { Name = "Alpha" };
        _sender.Send(Arg.Any<GetClientWithChildrenByFilterQuery>(), Arg.Any<CancellationToken>()).Returns(dto);

        var result = await _controller.GetClientWithChildrenByFilter(new RequestParameters(), CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>();
        await _sender.Received().Send(Arg.Any<GetClientWithChildrenByFilterQuery>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetClientsByVlanTag_DispatchesQueryWithRouteVlanTag()
    {
        var clients = new List<DetailClientDTO> { new() { Name = "Alpha" } };
        _sender.Send(Arg.Any<GetClientsByVlanTagQuery>(), Arg.Any<CancellationToken>()).Returns(clients);

        var result = await _controller.GetClientsByVlanTag(100, CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>();
        await _sender.Received().Send(
            Arg.Is<GetClientsByVlanTagQuery>(q => q.VlanTag == 100), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateClientsLocalDb_DispatchesCommandAndReturnsOk()
    {
        _sender.Send(Arg.Any<UpdateClientsLocalDbCommand>(), Arg.Any<CancellationToken>()).Returns(Unit.Value);

        var result = await _controller.UpdateClientsLocalDb(CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>();
        await _sender.Received().Send(Arg.Any<UpdateClientsLocalDbCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteClientsLocalDb_DispatchesCommandAndReturnsOk()
    {
        _sender.Send(Arg.Any<DeleteClientsLocalDbCommand>(), Arg.Any<CancellationToken>()).Returns(Unit.Value);

        var result = await _controller.DeleteClientsLocalDb(CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>();
        await _sender.Received().Send(Arg.Any<DeleteClientsLocalDbCommand>(), Arg.Any<CancellationToken>());
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

    [Fact]
    public async Task GetGatesByFilter_SetsPaginationHeaderAndReturnsOk()
    {
        var paged = PagedList<GateDTO>.Create([new GateDTO { Name = "gw" }], 7, new PaginationContext(1, 10));
        _sender.Send(Arg.Any<GetGatesByFilterQuery>(), Arg.Any<CancellationToken>()).Returns(paged);

        var result = await _controller.GetGatesByFilter(new RequestParameters(), CancellationToken.None);

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeSameAs(paged);
        _controller.Response.Headers["X-Pagination"].ToString().Should().Contain("\"TotalCount\":7");
        await _sender.Received().Send(Arg.Any<GetGatesByFilterQuery>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetGateById_DispatchesQueryAndReturnsOk()
    {
        var id = Guid.NewGuid();
        _sender.Send(Arg.Any<GetGateByIdQuery>(), Arg.Any<CancellationToken>()).Returns(new GateDTO { Name = "gw" });

        var result = await _controller.GetGateById(id, CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>();
        await _sender.Received().Send(Arg.Is<GetGateByIdQuery>(q => q.Id == id), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateGate_DispatchesCommandWithDto()
    {
        var dto = new UpdateGateDTO
        {
            Id = Guid.NewGuid(),
            Name = "gw-updated",
            Community = "public",
            IPAddress = "10.0.0.2",
            TypeOfNetworkDevice = "Cisco"
        };
        UpdateGateCommand? received = null;
        _sender.Send(Arg.Do<UpdateGateCommand>(c => received = c), Arg.Any<CancellationToken>())
               .Returns(Unit.Value);

        var result = await _controller.UpdateGate(dto, CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>();
        received.Should().NotBeNull();
        received!.UpdateGateDTO.Name.Should().Be("gw-updated");
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

    [Fact]
    public async Task GetNetworkDevicesByFilter_SetsPaginationHeaderAndReturnsOk()
    {
        var paged = PagedList<NetworkDeviceDTO>.Create([new NetworkDeviceDTO { NetworkDeviceName = "gw" }],
                                                        4,
                                                        new PaginationContext(1, 10));
        _sender.Send(Arg.Any<GetNetworkDevicesByFilterQuery>(), Arg.Any<CancellationToken>()).Returns(paged);

        var result = await _controller.GetNetworkDevicesByFilter(new RequestParameters(), CancellationToken.None);

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeSameAs(paged);
        _controller.Response.Headers["X-Pagination"].ToString().Should().Contain("\"TotalCount\":4");
        await _sender.Received().Send(Arg.Any<GetNetworkDevicesByFilterQuery>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetNetworkDevicesByVlanTag_DispatchesQueryWithRouteVlanTag()
    {
        var compound = new CompoundObjectDTO();
        _sender.Send(Arg.Any<GetCompoundDataByVlanTagQuery>(), Arg.Any<CancellationToken>()).Returns(compound);

        var result = await _controller.GetNetworkDevicesByVlanTag(100, CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>();
        await _sender.Received().Send(
            Arg.Is<GetCompoundDataByVlanTagQuery>(q => q.VlanTag == 100), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateNetworkDevice_DispatchesCommand()
    {
        var command = new CreateNetworkDeviceCommand("10.0.0.1", "public", "Cisco");
        _sender.Send(Arg.Any<CreateNetworkDeviceCommand>(), Arg.Any<CancellationToken>()).Returns(Unit.Value);

        var result = await _controller.CreateNetworkDevice(command, CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>();
        await _sender.Received().Send(command, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteNetworkDeviceById_DispatchesCommandWithRouteId()
    {
        var id = Guid.NewGuid();
        _sender.Send(Arg.Any<DeleteNetworkDeviceByIdCommand>(), Arg.Any<CancellationToken>()).Returns(Unit.Value);

        var result = await _controller.DeleteNetworkDeviceById(id, CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>();
        await _sender.Received().Send(
            Arg.Is<DeleteNetworkDeviceByIdCommand>(c => c.Id == id), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteNetworkDevices_DispatchesCommandAndReturnsOk()
    {
        _sender.Send(Arg.Any<DeleteAllNetworkDevicesCommand>(), Arg.Any<CancellationToken>()).Returns(Unit.Value);

        var result = await _controller.DeleteNetworkDevices(CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>();
        await _sender.Received().Send(Arg.Any<DeleteAllNetworkDevicesCommand>(), Arg.Any<CancellationToken>());
    }
}

public class SPRVlansControllerTests
{
    readonly ISender _sender = Substitute.For<ISender>();
    readonly SPRVlansController _controller;

    public SPRVlansControllerTests()
    {
        _controller = new SPRVlansController(_sender)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };
    }

    [Fact]
    public async Task GetSPRVlansByFilter_SetsPaginationHeaderAndReturnsOk()
    {
        var paged = PagedList<SPRVlanDTO>.Create([new SPRVlanDTO { IdVlan = 100 }], 9, new PaginationContext(1, 10));
        _sender.Send(Arg.Any<GetSPRVlansByFilterQuery>(), Arg.Any<CancellationToken>()).Returns(paged);

        var result = await _controller.GetSPRVlansByFilter(new RequestParameters(), CancellationToken.None);

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeSameAs(paged);
        _controller.Response.Headers["X-Pagination"].ToString().Should().Contain("\"TotalCount\":9");
        await _sender.Received().Send(Arg.Any<GetSPRVlansByFilterQuery>(), Arg.Any<CancellationToken>());
    }
}

public class TfPlansControllerTests
{
    readonly ISender _sender = Substitute.For<ISender>();
    readonly TfPlansController _controller;

    public TfPlansControllerTests()
    {
        _controller = new TfPlansController(_sender)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };
    }

    [Fact]
    public async Task GetTfPlansByFilter_SetsPaginationHeaderAndReturnsOk()
    {
        var paged = PagedList<TfPlanDTO>.Create([new TfPlanDTO { NameTfPlan = "Gold" }], 2, new PaginationContext(1, 10));
        _sender.Send(Arg.Any<GetTfPlansByFilterQuery>(), Arg.Any<CancellationToken>()).Returns(paged);

        var result = await _controller.GetTfPlansByFilter(new RequestParameters(), CancellationToken.None);

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeSameAs(paged);
        _controller.Response.Headers["X-Pagination"].ToString().Should().Contain("\"TotalCount\":2");
        await _sender.Received().Send(Arg.Any<GetTfPlansByFilterQuery>(), Arg.Any<CancellationToken>());
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
