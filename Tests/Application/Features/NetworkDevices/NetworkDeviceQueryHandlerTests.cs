using AutoMapper;
using MediatR;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.CQRS;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.NetDevRep;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.SNMPRep;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.Specification;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.UnOfWrkRep;
using Remote.Shell.Interrupt.Storehouse.Application.DTOs.NetworkDevices;
using Remote.Shell.Interrupt.Storehouse.Application.DTOs.Organizations;
using Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Commands.DeleteNetworkDeviceById;
using Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Commands.DeleteNetworkDevices;
using Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetNetworkDeviceById;
using Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetNetworkDeviceByVlanTag;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetClientsByVlanTag;
using Remote.Shell.Interrupt.Storehouse.Domain.SNMP;
using Remote.Shell.Interrupt.Storehouse.QueryFilterParser.QueryFilterParsers;

namespace Tests.Application.Features.NetworkDevices;

public class DeleteNetworkDeviceByIdCommandHandlerTests
{
    readonly INetDevUnitOfWork _unitOfWork = Substitute.For<INetDevUnitOfWork>();
    readonly INetworkDeviceRepository _devices = Substitute.For<INetworkDeviceRepository>();
    readonly INetworkDeviceSpecification _specification = Substitute.For<INetworkDeviceSpecification>();
    readonly IQueryFilterParser _parser = new CommonQueryFilterParser();

    public DeleteNetworkDeviceByIdCommandHandlerTests()
    {
        _unitOfWork.NetworkDevices.Returns(_devices);
        _specification.Clone().Returns(_specification);
    }

    [Fact]
    public async Task Handle_DeviceNotFound_ThrowsEntityNotFound()
    {
        _devices.AnyByQueryAsync(Arg.Any<ISpecification<NetworkDevice>>(), Arg.Any<CancellationToken>())
                .Returns(false);
        var handler = new DeleteNetworkDeviceByIdCommandHandler(_unitOfWork, _specification, _parser);

        var act = async () => await ((IRequestHandler<DeleteNetworkDeviceByIdCommand, Unit>)handler)
            .Handle(new DeleteNetworkDeviceByIdCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<EntityNotFoundException>();
        _devices.DidNotReceiveWithAnyArgs().DeleteOneWithChildren(default!);
    }

    [Fact]
    public async Task Handle_DeviceFound_DeletesWithChildrenAndCompletes()
    {
        var device = new NetworkDevice { Id = Guid.NewGuid() };
        _devices.AnyByQueryAsync(Arg.Any<ISpecification<NetworkDevice>>(), Arg.Any<CancellationToken>())
                .Returns(true);
        _devices.GetOneWithChildrenAsync(Arg.Any<ISpecification<NetworkDevice>>(), Arg.Any<CancellationToken>())
                .Returns(device);
        var handler = new DeleteNetworkDeviceByIdCommandHandler(_unitOfWork, _specification, _parser);

        await ((IRequestHandler<DeleteNetworkDeviceByIdCommand, Unit>)handler)
            .Handle(new DeleteNetworkDeviceByIdCommand(device.Id), CancellationToken.None);

        _devices.Received().DeleteOneWithChildren(device);
        _unitOfWork.Received().Complete();
    }
}

public class DeleteAllNetworkDevicesCommandHandlerTests
{
    readonly INetDevUnitOfWork _unitOfWork = Substitute.For<INetDevUnitOfWork>();
    readonly INetworkDeviceRepository _devices = Substitute.For<INetworkDeviceRepository>();
    readonly INetworkDeviceSpecification _specification = Substitute.For<INetworkDeviceSpecification>();
    readonly IQueryFilterParser _parser = new CommonQueryFilterParser();

    public DeleteAllNetworkDevicesCommandHandlerTests()
    {
        _unitOfWork.NetworkDevices.Returns(_devices);
        _specification.Clone().Returns(_specification);
    }

    [Fact]
    public async Task Handle_MultipleDevices_DeletesEachIndividually()
    {
        var device1 = new NetworkDevice { Id = Guid.NewGuid() };
        var device2 = new NetworkDevice { Id = Guid.NewGuid() };
        _devices.GetAllAsync(Arg.Any<CancellationToken>()).Returns([device1, device2]);
        _devices.AnyByQueryAsync(Arg.Any<ISpecification<NetworkDevice>>(), Arg.Any<CancellationToken>())
                .Returns(true);
        _devices.GetOneWithChildrenAsync(Arg.Any<ISpecification<NetworkDevice>>(), Arg.Any<CancellationToken>())
                .Returns(device1, device2);

        var deleted = new List<NetworkDevice>();
        _devices.DeleteOneWithChildren(Arg.Do<NetworkDevice>(d => deleted.Add(d)));

        var handler = new DeleteAllNetworkDevicesCommandHandler(_unitOfWork, _specification, _parser);
        await ((IRequestHandler<DeleteAllNetworkDevicesCommand, Unit>)handler)
            .Handle(new DeleteAllNetworkDevicesCommand(), CancellationToken.None);

        deleted.Should().HaveCount(2);
        _unitOfWork.Received(2).Complete();
    }

    [Fact]
    public async Task Handle_NoDevices_DoesNothing()
    {
        _devices.GetAllAsync(Arg.Any<CancellationToken>()).Returns([]);

        var handler = new DeleteAllNetworkDevicesCommandHandler(_unitOfWork, _specification, _parser);
        await ((IRequestHandler<DeleteAllNetworkDevicesCommand, Unit>)handler)
            .Handle(new DeleteAllNetworkDevicesCommand(), CancellationToken.None);

        _devices.DidNotReceiveWithAnyArgs().DeleteOneWithChildren(default!);
        _unitOfWork.DidNotReceive().Complete();
    }
}

public class GetNetworkDeviceByIdQueryHandlerTests
{
    readonly INetDevUnitOfWork _unitOfWork = Substitute.For<INetDevUnitOfWork>();
    readonly INetworkDeviceRepository _devices = Substitute.For<INetworkDeviceRepository>();
    readonly INetworkDeviceSpecification _specification = Substitute.For<INetworkDeviceSpecification>();
    readonly IQueryFilterParser _parser = new CommonQueryFilterParser();
    readonly IMapper _mapper;

    public GetNetworkDeviceByIdQueryHandlerTests()
    {
        _unitOfWork.NetworkDevices.Returns(_devices);
        _specification.Clone().Returns(_specification);
        _specification.AddInclude(Arg.Any<Expression<Func<NetworkDevice, object>>>()).Returns(_specification);
        _specification.AddInclude(Arg.Any<Expression<Func<NetworkDevice, List<Port>>>>()).Returns(_specification);
        _specification.AddThenInclude(Arg.Any<Expression<Func<Port, IEnumerable<ARPEntity>>>>()).Returns(_specification);
        _specification.AddThenInclude(Arg.Any<Expression<Func<Port, IEnumerable<MACEntity>>>>()).Returns(_specification);
        _specification.AddThenInclude(Arg.Any<Expression<Func<Port, IEnumerable<TerminatedNetworkEntity>>>>()).Returns(_specification);
        _specification.AddThenInclude(Arg.Any<Expression<Func<Port, IEnumerable<VLAN>>>>()).Returns(_specification);

        var config = new MapperConfiguration(cfg =>
            cfg.AddProfile(new AssemblyMappingProfile(typeof(NetworkDeviceDTO).Assembly)),
            NullLoggerFactory.Instance);
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task Handle_DeviceNotFound_ThrowsEntityNotFound()
    {
        _devices.AnyByQueryAsync(Arg.Any<ISpecification<NetworkDevice>>(), Arg.Any<CancellationToken>())
                .Returns(false);
        var handler = new GetNetworkDeviceByIdQueryHandler(_unitOfWork, _specification, _parser, _mapper);

        var act = async () => await handler.Handle(
            new GetNetworkDeviceByIdQuery(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task Handle_ChildPortsAreAggregatedIntoParentsAndRemovedFromTopLevel()
    {
        var parent = new Port { Id = Guid.NewGuid(), InterfaceName = "ae0" };
        var child1 = new Port { Id = Guid.NewGuid(), InterfaceName = "xe-0/0/0", ParentId = parent.Id };
        var child2 = new Port { Id = Guid.NewGuid(), InterfaceName = "xe-0/0/1", ParentId = parent.Id };
        var device = new NetworkDevice
        {
            Id = Guid.NewGuid(),
            NetworkDeviceName = "gw",
            Host = 3232235777,
            PortsOfNetworkDevice = [parent, child1, child2]
        };
        _devices.AnyByQueryAsync(Arg.Any<ISpecification<NetworkDevice>>(), Arg.Any<CancellationToken>())
                .Returns(true);
        _devices.GetOneWithChildrenAsync(Arg.Any<ISpecification<NetworkDevice>>(), Arg.Any<CancellationToken>())
                .Returns(device);
        var handler = new GetNetworkDeviceByIdQueryHandler(_unitOfWork, _specification, _parser, _mapper);

        var result = await handler.Handle(new GetNetworkDeviceByIdQuery(device.Id), CancellationToken.None);

        result.NetworkDeviceName.Should().Be("gw");
        result.Host.Should().Be("192.168.1.1");
        result.PortsOfNetworkDevice.Should().ContainSingle(p => p.InterfaceName == "ae0");
        result.PortsOfNetworkDevice[0].AggregatedPorts.Should().HaveCount(2);
        result.PortsOfNetworkDevice[0].AggregatedPorts.Should().Contain(p => p.InterfaceName == "xe-0/0/0");
    }

    [Fact]
    public async Task Handle_OrphanChildPortWithMissingParent_IsSkippedWithoutThrowing()
    {
        var orphan = new Port { Id = Guid.NewGuid(), InterfaceName = "xe-0/0/9", ParentId = Guid.NewGuid() };
        var device = new NetworkDevice
        {
            Id = Guid.NewGuid(),
            PortsOfNetworkDevice = [orphan]
        };
        _devices.AnyByQueryAsync(Arg.Any<ISpecification<NetworkDevice>>(), Arg.Any<CancellationToken>())
                .Returns(true);
        _devices.GetOneWithChildrenAsync(Arg.Any<ISpecification<NetworkDevice>>(), Arg.Any<CancellationToken>())
                .Returns(device);
        var handler = new GetNetworkDeviceByIdQueryHandler(_unitOfWork, _specification, _parser, _mapper);

        var result = await handler.Handle(new GetNetworkDeviceByIdQuery(device.Id), CancellationToken.None);

        result.PortsOfNetworkDevice.Should().ContainSingle(p => p.InterfaceName == "xe-0/0/9");
    }
}

public class GetCompoundDataByVlanTagQueryHandlerTests
{
    readonly INetDevUnitOfWork _unitOfWork = Substitute.For<INetDevUnitOfWork>();
    readonly INetworkDeviceRepository _devices = Substitute.For<INetworkDeviceRepository>();
    readonly INetworkDeviceSpecification _specification = Substitute.For<INetworkDeviceSpecification>();
    readonly IQueryFilterParser _parser = new CommonQueryFilterParser();
    readonly IMapper _mapper;
    readonly IQueryHandler<GetClientsByVlanTagQuery, IEnumerable<DetailClientDTO>> _clientsHandler =
        Substitute.For<IQueryHandler<GetClientsByVlanTagQuery, IEnumerable<DetailClientDTO>>>();

    public GetCompoundDataByVlanTagQueryHandlerTests()
    {
        _unitOfWork.NetworkDevices.Returns(_devices);
        _specification.Clone().Returns(_specification);
        _specification.AddInclude(Arg.Any<Expression<Func<NetworkDevice, List<Port>>>>()).Returns(_specification);
        _specification.AddThenInclude(Arg.Any<Expression<Func<Port, IEnumerable<VLAN>>>>()).Returns(_specification);
        _specification.AddFilter(Arg.Any<Expression<Func<NetworkDevice, bool>>>()).Returns(_specification);

        var config = new MapperConfiguration(cfg =>
            cfg.AddProfile(new AssemblyMappingProfile(typeof(NetworkDeviceDTO).Assembly)),
            NullLoggerFactory.Instance);
        _mapper = config.CreateMapper();
    }

    GetCompoundDataByVlanTagQueryHandler CreateHandler()
        => new(_unitOfWork, _specification, _parser, _mapper, _clientsHandler);

    [Fact]
    public async Task Handle_NonPositiveVlanTag_ThrowsBadRequest()
    {
        var act = async () => await ((IQueryHandler<GetCompoundDataByVlanTagQuery, CompoundObjectDTO>)CreateHandler())
            .Handle(new GetCompoundDataByVlanTagQuery(-1), CancellationToken.None);

        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task Handle_FiltersPortsDevicesAndVlansByClientVlanTags()
    {
        var detailClient = new DetailClientDTO
        {
            Name = "Alpha",
            SPRVlans = [new SPRVlanDTO { IdVlan = 100 }]
        };
        _clientsHandler.Handle(Arg.Any<GetClientsByVlanTagQuery>(), Arg.Any<CancellationToken>())
                       .Returns((IEnumerable<DetailClientDTO>)[detailClient]);

        var vlan100 = new VLAN { Id = Guid.NewGuid(), VLANTag = 100, VLANName = "CLIENTS" };
        var vlan200 = new VLAN { Id = Guid.NewGuid(), VLANTag = 200, VLANName = "OTHER" };

        var aggregatedChild = new Port { Id = Guid.NewGuid(), InterfaceName = "xe-0/0/0.0" };
        aggregatedChild.VLANs.Add(vlan100);

        var matchingPort = new Port { Id = Guid.NewGuid(), InterfaceName = "xe-0/0/2" };
        aggregatedChild.ParentId = matchingPort.Id;
        matchingPort.VLANs.AddRange([vlan100, vlan200]);

        var untaggedPort = new Port { Id = Guid.NewGuid(), InterfaceName = "xe-0/0/3" };

        var goodDevice = new NetworkDevice
        {
            Id = Guid.NewGuid(),
            NetworkDeviceName = "gw-1",
            PortsOfNetworkDevice = [aggregatedChild, matchingPort, untaggedPort]
        };
        var badDevice = new NetworkDevice
        {
            Id = Guid.NewGuid(),
            NetworkDeviceName = "gw-2",
            PortsOfNetworkDevice = [untaggedPort]
        };
        _devices.GetManyWithChildrenAsync(Arg.Any<ISpecification<NetworkDevice>>(), Arg.Any<CancellationToken>())
                .Returns([goodDevice, badDevice]);

        var result = await ((IQueryHandler<GetCompoundDataByVlanTagQuery, CompoundObjectDTO>)CreateHandler())
            .Handle(new GetCompoundDataByVlanTagQuery(100), CancellationToken.None);

        result.Clients.Should().ContainSingle(c => c.Name == "Alpha");

        var devices = result.NetworkDevices.ToList();
        devices.Should().ContainSingle(d => d.NetworkDeviceName == "gw-1");

        var ports = devices[0].PortsOfNetworkDevice;
        ports.Should().ContainSingle(p => p.InterfaceName == "xe-0/0/2");
        ports[0].VLANs.Should().ContainSingle(v => v.VLANTag == 100);
        ports[0].AggregatedPorts.Should().ContainSingle(p => p.InterfaceName == "xe-0/0/0.0");
    }
}

public class SnmpExecutorHandlerTests
{
    readonly ISNMPCommandExecutor _executor = Substitute.For<ISNMPCommandExecutor>();

    [Fact]
    public async Task SnmpGetCommandHandler_ForwardsRequestToExecutor()
    {
        var response = new SNMPResponse { OID = "1.3.6.1.2.1.1.1.0", Data = "TestOS" };
        _executor.GetCommand("10.0.0.1", "public", "1.3.6.1.2.1.1.1.0", Arg.Any<CancellationToken>(), false)
                 .Returns(response);
        var handler = new SNMPGetCommandHandler(_executor);

        var result = await ((IRequestHandler<SNMPGetCommand, SNMPResponse>)handler)
            .Handle(new SNMPGetCommand("10.0.0.1", "public", "1.3.6.1.2.1.1.1.0"), CancellationToken.None);

        result.Should().BeSameAs(response);
    }

    [Fact]
    public async Task SnmpWalkCommandHandler_ForwardsRequestToExecutor()
    {
        var responses = new List<SNMPResponse> { new() { OID = "1.3.6.1", Data = "1" } };
        _executor.WalkCommand("10.0.0.1", "private", "1.3.6.1", Arg.Any<CancellationToken>(), false, 20)
                 .Returns(responses);
        var handler = new SNMPWalkCommandHandler(_executor);

        var result = await ((IRequestHandler<SNMPWalkCommand, IEnumerable<SNMPResponse>>)handler)
            .Handle(new SNMPWalkCommand("10.0.0.1", "private", "1.3.6.1"), CancellationToken.None);

        result.Should().BeSameAs(responses);
    }

    [Fact]
    public async Task SnmpGetCommandHandler_ExecutorThrows_PropagatesException()
    {
        _executor.GetCommand(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(),
                             Arg.Any<CancellationToken>(), Arg.Any<bool>())
                 .Returns((Func<NSubstitute.Core.CallInfo, SNMPResponse>)(ci => throw new SNMPBadRequestException("timeout")));
        var handler = new SNMPGetCommandHandler(_executor);

        var act = async () => await ((IRequestHandler<SNMPGetCommand, SNMPResponse>)handler)
            .Handle(new SNMPGetCommand("10.0.0.1", "public", "1.3.6.1.2.1.1.1.0"), CancellationToken.None);

        await act.Should().ThrowAsync<SNMPBadRequestException>().WithMessage("timeout");
    }
}
