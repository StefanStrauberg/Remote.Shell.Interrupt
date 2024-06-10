using Remote.Shell.Interrupt.BuildingBlocks.Logger;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    var Assembly = typeof(Program).Assembly;

    builder.Services.AddLoggerServices();
    builder.Services.AddCarter();
    builder.Services.AddMediatR(config =>
    {
        config.RegisterServicesFromAssembly(Assembly);
    });
    builder.Services.AddValidatorsFromAssembly(Assembly);
    builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
    builder.Services.AddTransient<ExceptionHandlingMiddleware>();
    builder.Services.AddSNMPCommandExecutorServices();

    builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console());

    var app = builder.Build();

    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.MapCarter();
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
