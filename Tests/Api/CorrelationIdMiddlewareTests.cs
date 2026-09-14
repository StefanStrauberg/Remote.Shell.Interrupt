using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Remote.Shell.Interrupt.Storehouse.API.Middleware;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Tests.Api;

public class CorrelationIdMiddlewareTests
{
    readonly CorrelationIdMiddleware _middleware = new();

    // A bare DefaultHttpContext doesn't wire OnStarting callbacks to anything - there is
    // no real server underneath to fire them when the response actually starts. This fake
    // captures the callback the middleware registers so the test can trigger it explicitly,
    // the way Kestrel would just before writing the first response byte.
    sealed class FakeHttpResponseFeature : HttpResponseFeature
    {
        public Func<Task>? OnStartingCallback;

        public override void OnStarting(Func<object, Task> callback, object state)
            => OnStartingCallback = () => callback(state);
    }

    static (DefaultHttpContext Context, FakeHttpResponseFeature ResponseFeature) CreateContext()
    {
        var context = new DefaultHttpContext();
        var responseFeature = new FakeHttpResponseFeature();
        context.Features.Set<IHttpResponseFeature>(responseFeature);
        context.Response.Body = new MemoryStream();
        return (context, responseFeature);
    }

    static async Task<string?> RunAndCaptureResponseHeaderAsync(
        CorrelationIdMiddleware middleware,
        DefaultHttpContext context,
        FakeHttpResponseFeature responseFeature,
        RequestDelegate? next = null)
    {
        next ??= _ => Task.CompletedTask;
        await ((IMiddleware)middleware).InvokeAsync(context, next);

        if (responseFeature.OnStartingCallback is not null)
            await responseFeature.OnStartingCallback();

        return context.Response.Headers[CorrelationIdMiddleware.HeaderName].FirstOrDefault();
    }

    [Fact]
    public async Task InvokeAsync_NoInboundHeader_GeneratesAndEchoesBackAGuid()
    {
        var (context, responseFeature) = CreateContext();

        var correlationId = await RunAndCaptureResponseHeaderAsync(_middleware, context, responseFeature);

        correlationId.Should().NotBeNullOrWhiteSpace();
        Guid.TryParse(correlationId, out _).Should().BeTrue();
    }

    [Fact]
    public async Task InvokeAsync_InboundHeaderPresent_ReusesItVerbatim()
    {
        var (context, responseFeature) = CreateContext();
        context.Request.Headers[CorrelationIdMiddleware.HeaderName] = "client-supplied-id-123";

        var correlationId = await RunAndCaptureResponseHeaderAsync(_middleware, context, responseFeature);

        correlationId.Should().Be("client-supplied-id-123");
    }

    [Fact]
    public async Task InvokeAsync_CallsNext()
    {
        var (context, responseFeature) = CreateContext();
        var nextCalled = false;
        RequestDelegate next = _ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        await RunAndCaptureResponseHeaderAsync(_middleware, context, responseFeature, next);

        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task InvokeAsync_PushesCorrelationIdOntoSerilogLogContext_SoDownstreamLogsCarryIt()
    {
        var events = new List<LogEvent>();
        var testLogger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Sink(new CapturingSink(events.Add))
            .CreateLogger();

        var (context, responseFeature) = CreateContext();

        RequestDelegate next = _ =>
        {
            testLogger.Information("inside the pipeline");
            return Task.CompletedTask;
        };

        var correlationId = await RunAndCaptureResponseHeaderAsync(_middleware, context, responseFeature, next);

        events.Should().ContainSingle();
        events[0].Properties.Should().ContainKey("CorrelationId");
        events[0].Properties["CorrelationId"].ToString().Should().Contain(correlationId!);
    }

    [Fact]
    public async Task InvokeAsync_LogContextScope_DoesNotLeakAfterRequestCompletes()
    {
        var events = new List<LogEvent>();
        var testLogger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Sink(new CapturingSink(events.Add))
            .CreateLogger();

        var (context, responseFeature) = CreateContext();
        await RunAndCaptureResponseHeaderAsync(_middleware, context, responseFeature);

        testLogger.Information("after the pipeline finished");

        events.Should().ContainSingle();
        events[0].Properties.Should().NotContainKey("CorrelationId");
    }

    sealed class CapturingSink(Action<LogEvent> capture) : ILogEventSink
    {
        public void Emit(LogEvent logEvent) => capture(logEvent);
    }
}
