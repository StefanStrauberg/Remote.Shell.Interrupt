using AutoMapper;
using MediatR;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.IGateRep;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.Specification;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.UnOfWrkRep;
using Remote.Shell.Interrupt.Storehouse.Application.DTOs.Gates;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Commands.CreateGate;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Queries.GetGatesByFilter;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Commands.DeleteGate;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Commands.UpdateGate;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Queries.GetGateById;
using Remote.Shell.Interrupt.Storehouse.QueryFilterParser.QueryFilterParsers;

namespace Tests.Application.Features.Gates;

public abstract class GateHandlerTestBase
{
    protected readonly IGateRepository Gates = Substitute.For<IGateRepository>();
    protected readonly IGateUnitOfWork UnitOfWork = Substitute.For<IGateUnitOfWork>();
    protected readonly IGateSpecification Specification = Substitute.For<IGateSpecification>();
    protected readonly IQueryFilterParser Parser = new CommonQueryFilterParser();
    protected readonly IMapper Mapper = Substitute.For<IMapper>();

    protected GateHandlerTestBase()
    {
        UnitOfWork.Gates.Returns(Gates);
        Specification.Clone().Returns(Specification);
    }
}

public class CreateGateCommandHandlerTests : GateHandlerTestBase
{
    readonly CreateGateCommandHandler _handler;
    readonly CreateGateCommand _command = new(new CreateGateDTO
    {
        Name = "gw-1",
        Community = "public",
        IPAddress = "192.168.1.1",
        TypeOfNetworkDevice = "Juniper"
    });

    public CreateGateCommandHandlerTests()
    {
        Mapper.Map<Gate>(Arg.Any<object>()).Returns(ci =>
        {
            var dto = ci.ArgAt<CreateGateDTO>(0);
            return new Gate
            {
                Name = dto.Name,
                Community = dto.Community,
                IPAddress = ConvertStringIPAddressToLong.Handle(dto.IPAddress),
                TypeOfNetworkDevice = Enum.Parse<TypeOfNetworkDevice>(dto.TypeOfNetworkDevice)
            };
        });
        _handler = new CreateGateCommandHandler(UnitOfWork, Specification, Parser, Mapper);
    }

    [Fact]
    public async Task Handle_DuplicateIpAddress_ThrowsEntityAlreadyExists()
    {
        Gates.AnyByQueryAsync(Arg.Any<ISpecification<Gate>>(), Arg.Any<CancellationToken>()).Returns(true);

        var act = async () => await ((ICommandHandler<CreateGateCommand, Unit>)_handler)
            .Handle(_command, CancellationToken.None);

        await act.Should().ThrowAsync<EntityAlreadyExists>();
        Gates.DidNotReceiveWithAnyArgs().InsertOne(default!);
    }

    [Fact]
    public async Task Handle_NoDuplicate_InsertsGateInsideTransactionAndCompletes()
    {
        Gates.AnyByQueryAsync(Arg.Any<ISpecification<Gate>>(), Arg.Any<CancellationToken>()).Returns(false);
        Gate? inserted = null;
        Gates.InsertOne(Arg.Do<Gate>(g => inserted = g));

        await ((ICommandHandler<CreateGateCommand, Unit>)_handler).Handle(_command, CancellationToken.None);

        inserted.Should().NotBeNull();
        inserted!.Name.Should().Be("gw-1");
        inserted.IPAddress.Should().Be(ConvertStringIPAddressToLong.Handle("192.168.1.1"));
        inserted.TypeOfNetworkDevice.Should().Be(TypeOfNetworkDevice.Juniper);
        UnitOfWork.Received().StartTransaction();
        UnitOfWork.Received().Complete();
    }

    [Fact]
    public async Task Handle_DuplicateCheckUsesParsedFilterOnIPAddress()
    {
        // A real specification is used so the parsed criteria are inspectable.
        var realSpec = new GateSpecification();
        var handler = new CreateGateCommandHandler(UnitOfWork, realSpec, Parser, Mapper);
        Gates.AnyByQueryAsync(Arg.Any<ISpecification<Gate>>(), Arg.Any<CancellationToken>()).Returns(false);

        await ((ICommandHandler<CreateGateCommand, Unit>)handler).Handle(_command, CancellationToken.None);

        await Gates.Received().AnyByQueryAsync(
            Arg.Is<ISpecification<Gate>>(s => s.Criterias != null), Arg.Any<CancellationToken>());
    }
}

public class DeleteGateCommandHandlerTests : GateHandlerTestBase
{
    readonly DeleteGateCommandHandler _handler;
    readonly Gate _gate = new() { Id = Guid.NewGuid(), Name = "gw-1" };

    public DeleteGateCommandHandlerTests()
    {
        _handler = new DeleteGateCommandHandler(UnitOfWork, Specification, Parser);
    }

    [Fact]
    public async Task Handle_GateNotFound_ThrowsEntityNotFound()
    {
        Gates.AnyByQueryAsync(Arg.Any<ISpecification<Gate>>(), Arg.Any<CancellationToken>()).Returns(false);

        var act = async () => await ((IRequestHandler<DeleteGateCommand, Unit>)_handler)
            .Handle(new DeleteGateCommand(_gate.Id), CancellationToken.None);

        await act.Should().ThrowAsync<EntityNotFoundException>();
        Gates.DidNotReceiveWithAnyArgs().DeleteOne(default!);
        UnitOfWork.DidNotReceive().Complete();
    }

    [Fact]
    public async Task Handle_GateExists_DeletesAndCompletes()
    {
        Gates.AnyByQueryAsync(Arg.Any<ISpecification<Gate>>(), Arg.Any<CancellationToken>()).Returns(true);
        Gates.GetOneShortAsync(Arg.Any<ISpecification<Gate>>(), Arg.Any<CancellationToken>()).Returns(_gate);

        await ((IRequestHandler<DeleteGateCommand, Unit>)_handler)
            .Handle(new DeleteGateCommand(_gate.Id), CancellationToken.None);

        Gates.Received().DeleteOne(_gate);
        UnitOfWork.Received().Complete();
    }
}

public class UpdateGateCommandHandlerTests : GateHandlerTestBase
{
    readonly UpdateGateCommandHandler _handler;
    readonly Gate _gate = new() { Id = Guid.NewGuid(), Name = "old", Community = "c", IPAddress = 1 };
    readonly UpdateGateDTO _dto = new()
    {
        Id = Guid.NewGuid(),
        Name = "new",
        Community = "new-community",
        IPAddress = "10.0.0.5",
        TypeOfNetworkDevice = "Cisco"
    };

    public UpdateGateCommandHandlerTests()
    {
        Mapper.Map(Arg.Any<UpdateGateDTO>(), Arg.Any<Gate>())
              .Returns(callInfo => callInfo.ArgAt<Gate>(1));
        _handler = new UpdateGateCommandHandler(UnitOfWork, Specification, Parser, Mapper);
    }

    [Fact]
    public async Task Handle_GateNotFound_ThrowsEntityNotFound()
    {
        Gates.AnyByQueryAsync(Arg.Any<ISpecification<Gate>>(), Arg.Any<CancellationToken>()).Returns(false);

        var act = async () => await ((ICommandHandler<UpdateGateCommand, Unit>)_handler)
            .Handle(new UpdateGateCommand(_dto), CancellationToken.None);

        await act.Should().ThrowAsync<EntityNotFoundException>();
        Gates.DidNotReceiveWithAnyArgs().ReplaceOne(default!);
    }

    [Fact]
    public async Task Handle_GateExists_MapsDtoOntoEntityReplacesAndCompletes()
    {
        Gates.AnyByQueryAsync(Arg.Any<ISpecification<Gate>>(), Arg.Any<CancellationToken>()).Returns(true);
        Gates.GetOneShortAsync(Arg.Any<ISpecification<Gate>>(), Arg.Any<CancellationToken>()).Returns(_gate);

        await ((ICommandHandler<UpdateGateCommand, Unit>)_handler)
            .Handle(new UpdateGateCommand(_dto), CancellationToken.None);

        Mapper.Received().Map(_dto, _gate);
        Gates.Received().ReplaceOne(_gate);
        UnitOfWork.Received().Complete();
    }
}

public class GetGateByIdQueryHandlerTests : GateHandlerTestBase
{
    [Fact]
    public async Task Handle_GateNotFound_ThrowsEntityNotFound()
    {
        var handler = new GetGateByIdQueryHandler(UnitOfWork, Specification, Parser, Mapper);
        Gates.AnyByQueryAsync(Arg.Any<ISpecification<Gate>>(), Arg.Any<CancellationToken>()).Returns(false);

        var act = async () => await ((IRequestHandler<GetGateByIdQuery, GateDTO>)handler).Handle(new GetGateByIdQuery(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task Handle_GateExists_ReturnsMappedDtoWithDottedIpAddress()
    {
        var mapperConfig = new MapperConfiguration(cfg =>
            cfg.AddProfile(new AssemblyMappingProfile(typeof(GateDTO).Assembly)),
            NullLoggerFactory.Instance);
        var realMapper = mapperConfig.CreateMapper();
        var handler = new GetGateByIdQueryHandler(UnitOfWork, Specification, Parser, realMapper);
        var gate = new Gate
        {
            Id = Guid.NewGuid(),
            Name = "gw",
            Community = "public",
            IPAddress = 3232235777,
            TypeOfNetworkDevice = TypeOfNetworkDevice.Huawei
        };
        Gates.AnyByQueryAsync(Arg.Any<ISpecification<Gate>>(), Arg.Any<CancellationToken>()).Returns(true);
        Gates.GetOneShortAsync(Arg.Any<ISpecification<Gate>>(), Arg.Any<CancellationToken>()).Returns(gate);

        var result = await ((IRequestHandler<GetGateByIdQuery, GateDTO>)handler).Handle(new GetGateByIdQuery(gate.Id), CancellationToken.None);

        result.Id.Should().Be(gate.Id);
        result.Name.Should().Be("gw");
        result.IPAddress.Should().Be("192.168.1.1");
        result.TypeOfNetworkDevice.Should().Be("Huawei");
    }
}

public class GetGatesByFilterQueryHandlerTests : GateHandlerTestBase
{
    readonly GetGatesByFilterQueryHandler _handler;
    readonly List<Gate> _gates =
    [
        new Gate { Id = Guid.NewGuid(), Name = "gw-1" },
        new Gate { Id = Guid.NewGuid(), Name = "gw-2" }
    ];

    public GetGatesByFilterQueryHandlerTests()
    {
        Mapper.Map<IEnumerable<GateDTO>>(Arg.Any<object>()).Returns(
            [new GateDTO { Name = "gw-1" }, new GateDTO { Name = "gw-2" }]);
        _handler = new GetGatesByFilterQueryHandler(UnitOfWork, Specification, Parser, Mapper);
    }

    [Fact]
    public async Task Handle_Paginated_CountsAndReturnsPagedList()
    {
        Gates.GetManyShortAsync(Arg.Any<ISpecification<Gate>>(), Arg.Any<CancellationToken>())
             .Returns(_gates);
        Gates.GetCountAsync(Arg.Any<ISpecification<Gate>>(), Arg.Any<CancellationToken>()).Returns(25);
        var query = new GetGatesByFilterQuery(new RequestParameters { PageNumber = 1, PageSize = 10 });

        var result = await _handler.Handle(query, CancellationToken.None);

        result.TotalCount.Should().Be(25);
        result.TotalPages.Should().Be(3);
        result.CurrentPage.Should().Be(1);
        result.Should().HaveCount(2);
        await Gates.Received().GetCountAsync(Arg.Any<ISpecification<Gate>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NonPaginated_DoesNotRunCountQueryAndReturnsSinglePage()
    {
        Gates.GetManyShortAsync(Arg.Any<ISpecification<Gate>>(), Arg.Any<CancellationToken>())
             .Returns(_gates);
        var query = new GetGatesByFilterQuery(new RequestParameters());

        var result = await _handler.Handle(query, CancellationToken.None);

        result.TotalCount.Should().Be(2);
        result.TotalPages.Should().Be(1);
        result.CurrentPage.Should().Be(1);
        await Gates.DidNotReceive().GetCountAsync(Arg.Any<ISpecification<Gate>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_EmptyResult_ReturnsEmptyPagedList()
    {
        Gates.GetManyShortAsync(Arg.Any<ISpecification<Gate>>(), Arg.Any<CancellationToken>())
             .Returns([]);

        var result = await _handler.Handle(
            new GetGatesByFilterQuery(new RequestParameters { PageNumber = 1, PageSize = 10 }),
            CancellationToken.None);

        result.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }
}
