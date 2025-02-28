using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using OpenTelemetry.Metrics;
using Prod.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddControllers();

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddDbContext<ProdContext>(o =>
    o.UseNpgsql(new NpgsqlConnectionStringBuilder
        {
            Host = "postgres",
            Port = 5432,
            Database = builder.Configuration["POSTGRES_DB"],
            Username = builder.Configuration["POSTGRES_USER"],
            Password = builder.Configuration["POSTGRES_PASSWORD"]
        }.ConnectionString,
        options => options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

services.AddOpenTelemetry()
    .WithMetrics(options =>
    {
        options.AddAspNetCoreInstrumentation();
        options.AddHttpClientInstrumentation();
        options.AddMeter("Microsoft.AspNetCore.Hosting");
        options.AddMeter("Microsoft.AspNetCore.Server.Kestrel");
    });

services.AddHttpLogging(o =>
    o.LoggingFields = HttpLoggingFields.RequestMethod
                      | HttpLoggingFields.RequestPath
                      | HttpLoggingFields.ResponseStatusCode
                      | HttpLoggingFields.Duration);
builder.Logging.AddSerilog().AddOpenTelemetry();

services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ProdContext>();
    context.Database.EnsureCreated();
}

app.Run();
