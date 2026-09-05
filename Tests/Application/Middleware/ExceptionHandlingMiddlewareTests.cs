using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Logger;
using Remote.Shell.Interrupt.Storehouse.Application.Middleware;

namespace Tests.Application.Middleware;

public class ExceptionHandlingMiddlewareTests
{
    readonly IAppLogger<ExceptionHandlingMiddleware> _logger =
        Substitute.For<IAppLogger<ExceptionHandlingMiddleware>>();
    readonly ExceptionHandlingMiddleware _middleware;

    public ExceptionHandlingMiddlewareTests()
    {
        _middleware = new ExceptionHandlingMiddleware(_logger);
    }

    static (DefaultHttpContext Context, MemoryStream Body) CreateContext()
    {
        var body = new MemoryStream();
        var context = new DefaultHttpContext();
        context.Response.Body = body;
        return (context, body);
    }

    static async Task<string> ReadBodyAsync(MemoryStream body)
    {
        body.Position = 0;
        using var reader = new StreamReader(body);
        return await reader.ReadToEndAsync();
    }

    [Fact]
    public async Task InvokeAsync_NoException_PassesThroughPipeline()
    {
        var (context, _) = CreateContext();
        var nextCalled = false;
        RequestDelegate next = ctx =>
        {
            nextCalled = true;
            ctx.Response.StatusCode = StatusCodes.Status201Created;
            return Task.CompletedTask;
        };

        await ((IMiddleware)_middleware).InvokeAsync(context, next);

        nextCalled.Should().BeTrue();
        context.Response.StatusCode.Should().Be(StatusCodes.Status201Created);
    }

    [Fact]
    public async Task InvokeAsync_NotFoundException_Returns404WithTitle()
    {
        var (context, body) = CreateContext();
        RequestDelegate next = _ => throw new EntityNotFoundException(typeof(Gate), "Id");

        await ((IMiddleware)_middleware).InvokeAsync(context, next);

        context.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        context.Response.ContentType.Should().Be("application/json");

        var json = await ReadBodyAsync(body);
        json.Should().Contain("\"Status\":404");
        json.Should().Contain("Not Found");
        json.Should().Contain("Gate");
    }

    [Fact]
    public async Task InvokeAsync_BadRequestException_Returns400()
    {
        var (context, body) = CreateContext();
        RequestDelegate next = _ => throw new SNMPBadRequestException("snmp failed");

        await ((IMiddleware)_middleware).InvokeAsync(context, next);

        context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        var json = await ReadBodyAsync(body);
        json.Should().Contain("\"Status\":400");
        json.Should().Contain("Bad Request");
        json.Should().Contain("snmp failed");
    }

    [Fact]
    public async Task InvokeAsync_ValidationException_Returns422WithErrorsDictionary()
    {
        var (context, body) = CreateContext();
        var errors = new Dictionary<string, string[]> { ["Name"] = ["required"] };
        RequestDelegate next = _ => throw new ValidationException(errors);

        await ((IMiddleware)_middleware).InvokeAsync(context, next);

        context.Response.StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
        var json = await ReadBodyAsync(body);
        json.Should().Contain("\"Status\":422");
        json.Should().Contain("required");
    }

    [Fact]
    public async Task InvokeAsync_UnexpectedException_Returns500WithServerErrortitle()
    {
        var (context, body) = CreateContext();
        RequestDelegate next = _ => throw new InvalidOperationException("unexpected");

        await ((IMiddleware)_middleware).InvokeAsync(context, next);

        context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        var json = await ReadBodyAsync(body);
        json.Should().Contain("\"Status\":500");
        json.Should().Contain("Server Error");
        json.Should().Contain("unexpected");
    }

    [Fact]
    public async Task InvokeAsync_Exception_LogsErrorDetails()
    {
        var (context, _) = CreateContext();
        RequestDelegate next = _ => throw new InvalidOperationException("logged");

        await ((IMiddleware)_middleware).InvokeAsync(context, next);

        _logger.Received().LogError(Arg.Is<string>(m => m.Contains("logged")));
    }
}
