Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddApplicationServices();
    builder.Services.AddLoggerServices();
    builder.Services.AddSNMPCommandExecutorServices();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console());

    var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapControllers();
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
