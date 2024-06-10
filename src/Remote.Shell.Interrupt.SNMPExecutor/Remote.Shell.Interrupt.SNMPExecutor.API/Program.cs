Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddCarter();
    builder.Services.AddApplicationServices();
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
