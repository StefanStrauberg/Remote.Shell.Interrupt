using AutoMapper;
using MediatR;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.LocBillRep;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.Specification;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.UnOfWrkRep;
using Remote.Shell.Interrupt.Storehouse.Application.DTOs.Organizations;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Commands.DeleteClientsLocalDb;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Commands.UpdateClientsLocalDb;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetClientById;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetClientsByFilter;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetClientsByVlanTag;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetClientsWithChildrenByFilter;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetClientWithChildrenByFilter;
using Remote.Shell.Interrupt.Storehouse.Application.Features.SPRVlans.Queries.GetSPRVlansByFilter;
using Remote.Shell.Interrupt.Storehouse.QueryFilterParser.QueryFilterParsers;

namespace Tests.Application.Features.Organizations;

public class GetClientsByVlanTagQueryHandlerTests
{
    readonly ILocBillUnitOfWork _unitOfWork = Substitute.For<ILocBillUnitOfWork>();
    readonly IClientsRepository _clients = Substitute.For<IClientsRepository>();
    readonly ISPRVlansRepository _sprVlans = Substitute.For<ISPRVlansRepository>();
    readonly IClientSpecification _clientSpec = Substitute.For<IClientSpecification>();
    readonly IQueryFilterParser _parser = new CommonQueryFilterParser();
    readonly IMapper _mapper;

    public GetClientsByVlanTagQueryHandlerTests()
    {
        _unitOfWork.Clients.Returns(_clients);
        _unitOfWork.SPRVlans.Returns(_sprVlans);
        _clientSpec.Clone().Returns(_clientSpec);
        var config = new MapperConfiguration(cfg =>
            cfg.AddProfile(new AssemblyMappingProfile(typeof(DetailClientDTO).Assembly)),
            NullLoggerFactory.Instance);
        _mapper = config.CreateMapper();
    }

    GetSPRVlansByFilterQueryHandler CreateSprVlanHandler()
    {
        var sprSpec = Substitute.For<ISPRVlanSpecification>();
        sprSpec.Clone().Returns(sprSpec);
        return new GetSPRVlansByFilterQueryHandler(_unitOfWork, sprSpec, _parser, _mapper);
    }

    [Fact]
    public async Task Handle_NonPositiveVlanTag_ThrowsBadRequest()
    {
        var handler = new GetClientsByVlanTagQueryHandler(_unitOfWork, _clientSpec, _parser, _mapper,
                                                          CreateSprVlanHandler());

        var act = async () => await ((IQueryHandler<GetClientsByVlanTagQuery, IEnumerable<DetailClientDTO>>)handler)
            .Handle(new GetClientsByVlanTagQuery(0), CancellationToken.None);

        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task Handle_ClientsFoundPerVlan_ReturnsDistinctMappedClients()
    {
        var vlans = new List<SPRVlan>
        {
            new() { IdVlan = 100, IdClient = 5 },
            new() { IdVlan = 100, IdClient = 7 }
        };
        _sprVlans.GetManyShortAsync(Arg.Any<ISpecification<SPRVlan>>(), Arg.Any<CancellationToken>())
                 .Returns(vlans);

        var client5 = new Client
        {
            Id = Guid.NewGuid(),
            IdClient = 5,
            Name = "Alpha",
            NrDogovor = "D-5",
            COD = new COD { IdCOD = 1, NameCOD = "DC-1" },
            TfPlan = new TfPlan { IdTfPlan = 2, NameTfPlan = "Basic" },
            SPRVlans = [new SPRVlan { IdVlan = 100, IdClient = 5 }]
        };
        _clients.GetOneWithChildrenAsync(Arg.Any<ISpecification<Client>>(), Arg.Any<CancellationToken>())
                .Returns(client5, (Client?)null!);

        var handler = new GetClientsByVlanTagQueryHandler(_unitOfWork, _clientSpec, _parser, _mapper,
                                                          CreateSprVlanHandler());
        var result = await ((IQueryHandler<GetClientsByVlanTagQuery, IEnumerable<DetailClientDTO>>)handler)
            .Handle(new GetClientsByVlanTagQuery(100), CancellationToken.None);

        var clients = result.ToList();
        clients.Should().HaveCount(1);
        clients[0].Name.Should().Be("Alpha");
        clients[0].COD.Should().NotBeNull();
        clients[0].COD.NameCOD.Should().Be("DC-1");
        clients[0].SPRVlans.Should().ContainSingle(v => v.IdVlan == 100);
    }

    [Fact]
    public async Task Handle_SameClientFromMultipleVlanRows_Deduplicates()
    {
        var vlans = new List<SPRVlan>
        {
            new() { IdVlan = 100, IdClient = 5 },
            new() { IdVlan = 101, IdClient = 5 }
        };
        _sprVlans.GetManyShortAsync(Arg.Any<ISpecification<SPRVlan>>(), Arg.Any<CancellationToken>())
                 .Returns(vlans);
        var client = new Client { IdClient = 5, Name = "Same" };
        _clients.GetOneWithChildrenAsync(Arg.Any<ISpecification<Client>>(), Arg.Any<CancellationToken>())
                .Returns(client);

        var handler = new GetClientsByVlanTagQueryHandler(_unitOfWork, _clientSpec, _parser, _mapper,
                                                          CreateSprVlanHandler());
        var result = await ((IQueryHandler<GetClientsByVlanTagQuery, IEnumerable<DetailClientDTO>>)handler)
            .Handle(new GetClientsByVlanTagQuery(100), CancellationToken.None);

        result.Should().ContainSingle(c => c.Name == "Same");
    }
}

public class LocalDbSyncCommandHandlerTests
{
    readonly ILocBillUnitOfWork _local = Substitute.For<ILocBillUnitOfWork>();
    readonly IRemBillUnitOfWork _remote = Substitute.For<IRemBillUnitOfWork>();
    readonly IClientsRepository _clients = Substitute.For<IClientsRepository>();
    readonly ICODRepository _cods = Substitute.For<ICODRepository>();
    readonly ITfPlanRepository _tfPlans = Substitute.For<ITfPlanRepository>();
    readonly ISPRVlansRepository _sprVlans = Substitute.For<ISPRVlansRepository>();

    public LocalDbSyncCommandHandlerTests()
    {
        _local.Clients.Returns(_clients);
        _local.CODs.Returns(_cods);
        _local.TfPlans.Returns(_tfPlans);
        _local.SPRVlans.Returns(_sprVlans);
    }

    [Fact]
    public async Task DeleteClientsLocalDb_WithData_DeletesAllTablesAndCompletes()
    {
        _tfPlans.GetAllAsync(Arg.Any<CancellationToken>()).Returns([new TfPlan()]);
        _sprVlans.GetAllAsync(Arg.Any<CancellationToken>()).Returns([new SPRVlan()]);
        _clients.GetAllAsync(Arg.Any<CancellationToken>()).Returns([new Client()]);
        _cods.GetAllAsync(Arg.Any<CancellationToken>()).Returns([new COD()]);

        var handler = new DeleteClientsLocalDbCommandHandler(_local);
        await ((ICommandHandler<DeleteClientsLocalDbCommand, Unit>)handler)
            .Handle(new DeleteClientsLocalDbCommand(), CancellationToken.None);

        _tfPlans.Received().DeleteMany(Arg.Any<IEnumerable<TfPlan>>());
        _sprVlans.Received().DeleteMany(Arg.Any<IEnumerable<SPRVlan>>());
        _clients.Received().DeleteMany(Arg.Any<IEnumerable<Client>>());
        _cods.Received().DeleteMany(Arg.Any<IEnumerable<COD>>());
        _local.Received().Complete();
    }

    [Fact]
    public async Task DeleteClientsLocalDb_EmptyTables_DoesNotComplete()
    {
        _tfPlans.GetAllAsync(Arg.Any<CancellationToken>()).Returns([]);
        _sprVlans.GetAllAsync(Arg.Any<CancellationToken>()).Returns([]);
        _clients.GetAllAsync(Arg.Any<CancellationToken>()).Returns([]);
        _cods.GetAllAsync(Arg.Any<CancellationToken>()).Returns([]);

        var handler = new DeleteClientsLocalDbCommandHandler(_local);
        await ((ICommandHandler<DeleteClientsLocalDbCommand, Unit>)handler)
            .Handle(new DeleteClientsLocalDbCommand(), CancellationToken.None);

        _local.DidNotReceive().Complete();
    }

    [Fact]
    public async Task UpdateClientsLocalDb_RemoteData_InsertsCleanedEntitiesAndCompletes()
    {
        var remoteClients = Substitute.For<IRemoteClientsRepository>();
        var remoteCods = Substitute.For<IRemoteCODRepository>();
        var remoteTfPlans = Substitute.For<IRemoteTfPlanRepository>();
        var remoteSprVlans = Substitute.For<IRemoteSPRVlansRepository>();
        _remote.RemoteClients.Returns(remoteClients);
        _remote.RemoteCODs.Returns(remoteCods);
        _remote.RemoteTfPlans.Returns(remoteTfPlans);
        _remote.RemoteSPRVlans.Returns(remoteSprVlans);

        remoteClients.GetAllAsync(Arg.Any<CancellationToken>()).Returns(
        [
            new RemoteClient
            {
                IdClient = 1,
                Name = "Alpha  ",
                NrDogovor = "D-1 ",
                History = "log\0",
                Dat1 = new DateTime(2024, 1, 2, 3, 4, 5, DateTimeKind.Unspecified),
                ContactC = "c",
                TelephoneC = "t",
                ContactT = "ct",
                TelephoneT = "tt",
                EmailC = "e",
                EmailT = "et",
                Id_COD = 11,
                Id_TfPlan = 22
            }
        ]);
        remoteCods.GetAllAsync(Arg.Any<CancellationToken>()).Returns(
            [new RemoteCOD { IdCOD = 11, NameCOD = "DC " }]);
        remoteTfPlans.GetAllAsync(Arg.Any<CancellationToken>()).Returns(
            [new RemoteTfPlan { IdTfPlan = 22, NameTfPlan = "Basic ", DescTfPlan = "d\0" }]);
        remoteSprVlans.GetAllAsync(Arg.Any<CancellationToken>()).Returns(
            [new RemoteSPRVlan { IdVlan = 100, IdClient = 1, UseClient = true, UseCOD = false }]);

        _clients.GetAllAsync(Arg.Any<CancellationToken>()).Returns([]);
        _cods.GetAllAsync(Arg.Any<CancellationToken>()).Returns([]);
        _tfPlans.GetAllAsync(Arg.Any<CancellationToken>()).Returns([]);
        _sprVlans.GetAllAsync(Arg.Any<CancellationToken>()).Returns([]);

        var insertedClients = new List<Client>();
        var insertedCods = new List<COD>();
        var insertedTfPlans = new List<TfPlan>();
        var insertedSprVlans = new List<SPRVlan>();
        _clients.InsertMany(Arg.Do<IEnumerable<Client>>(x => insertedClients.AddRange(x)));
        _cods.InsertMany(Arg.Do<IEnumerable<COD>>(x => insertedCods.AddRange(x)));
        _tfPlans.InsertMany(Arg.Do<IEnumerable<TfPlan>>(x => insertedTfPlans.AddRange(x)));
        _sprVlans.InsertMany(Arg.Do<IEnumerable<SPRVlan>>(x => insertedSprVlans.AddRange(x)));

        var handler = new UpdateClientsLocalDbCommandHandler(_local, _remote);
        await ((ICommandHandler<UpdateClientsLocalDbCommand, Unit>)handler)
            .Handle(new UpdateClientsLocalDbCommand(), CancellationToken.None);

        insertedClients.Should().ContainSingle();
        var client = insertedClients[0];
        client.Name.Should().Be("Alpha");
        client.NrDogovor.Should().Be("D-1");
        client.History.Should().Be("log");
        client.Dat1!.Value.Kind.Should().Be(DateTimeKind.Utc);
        client.Id_COD.Should().Be(11);
        client.Id_TfPlan.Should().Be(22);

        insertedCods.Should().ContainSingle(c => c.IdCOD == 11 && c.NameCOD == "DC");
        insertedTfPlans.Should().ContainSingle(t => t.IdTfPlan == 22 && t.DescTfPlan == "d");
        insertedSprVlans.Should().ContainSingle(v => v.IdVlan == 100 && v.UseClient);
        _local.Received().Complete();
    }

    [Fact]
    public async Task UpdateClientsLocalDb_LocalDataPresent_DeletesExistingRecords()
    {
        var remoteClients = Substitute.For<IRemoteClientsRepository>();
        var remoteCods = Substitute.For<IRemoteCODRepository>();
        var remoteTfPlans = Substitute.For<IRemoteTfPlanRepository>();
        var remoteSprVlans = Substitute.For<IRemoteSPRVlansRepository>();
        _remote.RemoteClients.Returns(remoteClients);
        _remote.RemoteCODs.Returns(remoteCods);
        _remote.RemoteTfPlans.Returns(remoteTfPlans);
        _remote.RemoteSPRVlans.Returns(remoteSprVlans);
        remoteClients.GetAllAsync(Arg.Any<CancellationToken>()).Returns([]);
        remoteCods.GetAllAsync(Arg.Any<CancellationToken>()).Returns([]);
        remoteTfPlans.GetAllAsync(Arg.Any<CancellationToken>()).Returns([]);
        remoteSprVlans.GetAllAsync(Arg.Any<CancellationToken>()).Returns([]);

        _clients.GetAllAsync(Arg.Any<CancellationToken>()).Returns([new Client()]);
        _cods.GetAllAsync(Arg.Any<CancellationToken>()).Returns([new COD()]);
        _tfPlans.GetAllAsync(Arg.Any<CancellationToken>()).Returns([new TfPlan()]);
        _sprVlans.GetAllAsync(Arg.Any<CancellationToken>()).Returns([new SPRVlan()]);

        var handler = new UpdateClientsLocalDbCommandHandler(_local, _remote);
        await ((ICommandHandler<UpdateClientsLocalDbCommand, Unit>)handler)
            .Handle(new UpdateClientsLocalDbCommand(), CancellationToken.None);

        _clients.Received().DeleteMany(Arg.Any<IEnumerable<Client>>());
        _cods.Received().DeleteMany(Arg.Any<IEnumerable<COD>>());
        _tfPlans.Received().DeleteMany(Arg.Any<IEnumerable<TfPlan>>());
        _sprVlans.Received().DeleteMany(Arg.Any<IEnumerable<SPRVlan>>());
        _local.Received().Complete();
    }
}

public class OrganizationsDelegatingQueryHandlerTests
{
    readonly ILocBillUnitOfWork _unitOfWork = Substitute.For<ILocBillUnitOfWork>();
    readonly IClientsRepository _clients = Substitute.For<IClientsRepository>();
    readonly IClientSpecification _clientSpec = Substitute.For<IClientSpecification>();
    readonly IQueryFilterParser _parser = new CommonQueryFilterParser();
    readonly IMapper _mapper = Substitute.For<IMapper>();

    public OrganizationsDelegatingQueryHandlerTests()
    {
        _unitOfWork.Clients.Returns(_clients);
        _clientSpec.Clone().Returns(_clientSpec);
    }

    [Fact]
    public async Task GetClientsByFilter_DelegatesToShortQueryAndCount()
    {
        var clients = new List<Client> { new() { Name = "A" }, new() { Name = "B" } };
        _clients.GetManyShortAsync(Arg.Any<ISpecification<Client>>(), Arg.Any<CancellationToken>())
                .Returns(clients);
        _clients.GetCountAsync(Arg.Any<ISpecification<Client>>(), Arg.Any<CancellationToken>()).Returns(9);
        _mapper.Map<IEnumerable<ShortClientDTO>>(Arg.Any<object>())
               .Returns([new ShortClientDTO(), new ShortClientDTO()]);

        var handler = new GetClientsByFilterQueryHandler(_unitOfWork, _clientSpec, _parser, _mapper);
        var result = await handler.Handle(
            new GetClientsByFilterQuery(new RequestParameters { PageNumber = 1, PageSize = 10 }),
            CancellationToken.None);

        result.TotalCount.Should().Be(9);
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetClientsWithChildrenByFilter_FetchesClientsWithRelations()
    {
        var clients = new List<Client> { new() { Name = "A" } };
        _clients.GetManyWithChildrenAsync(Arg.Any<ISpecification<Client>>(), Arg.Any<CancellationToken>())
                .Returns(clients);
        _clients.GetCountAsync(Arg.Any<ISpecification<Client>>(), Arg.Any<CancellationToken>()).Returns(1);
        _mapper.Map<IEnumerable<DetailClientDTO>>(Arg.Any<object>()).Returns([new DetailClientDTO()]);

        var handler = new GetClientsWithChildrenByFilterQueryHandler(_unitOfWork, _clientSpec, _parser, _mapper);
        var result = await handler.Handle(
            new GetClientsWithChildrenByFilterQuery(new RequestParameters { PageNumber = 1, PageSize = 10 }),
            CancellationToken.None);

        result.Should().HaveCount(1);
        await _clients.Received().GetManyWithChildrenAsync(Arg.Any<ISpecification<Client>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetClientById_NotFound_ThrowsEntityNotFound()
    {
        _clients.AnyByQueryAsync(Arg.Any<ISpecification<Client>>(), Arg.Any<CancellationToken>()).Returns(false);
        var handler = new GetClientByIdQueryHandler(_unitOfWork, _clientSpec, _parser, _mapper);

        var act = async () => await ((IRequestHandler<GetClientByIdQuery, DetailClientDTO>)handler).Handle(new GetClientByIdQuery(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task GetClientById_Found_MapsAndReturnsDetailDto()
    {
        var client = new Client { Id = Guid.NewGuid(), Name = "Alpha" };
        _clients.AnyByQueryAsync(Arg.Any<ISpecification<Client>>(), Arg.Any<CancellationToken>()).Returns(true);
        _clients.GetOneWithChildrenAsync(Arg.Any<ISpecification<Client>>(), Arg.Any<CancellationToken>())
                .Returns(client);
        _mapper.Map<DetailClientDTO>(Arg.Any<Client>()).Returns(new DetailClientDTO { Name = "Alpha" });

        var handler = new GetClientByIdQueryHandler(_unitOfWork, _clientSpec, _parser, _mapper);
        var result = await ((IRequestHandler<GetClientByIdQuery, DetailClientDTO>)handler).Handle(new GetClientByIdQuery(client.Id), CancellationToken.None);

        result.Name.Should().Be("Alpha");
    }

    [Fact]
    public async Task GetClientWithChildrenByFilter_NotFound_ThrowsEntityNotFound()
    {
        _clients.AnyByQueryAsync(Arg.Any<ISpecification<Client>>(), Arg.Any<CancellationToken>()).Returns(false);
        var handler = new GetClientWithChildrenByFilterHandler(_unitOfWork, _clientSpec, _parser, _mapper);

        var act = async () => await handler.Handle(
            new GetClientWithChildrenByFilterQuery(new RequestParameters()), CancellationToken.None);

        await act.Should().ThrowAsync<EntityNotFoundException>();
    }
}
