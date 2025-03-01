using System.Reflection;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Prod.Exceptions;
using Prod.Services;
using Serilog;
using Serilog.Sinks.Grafana.Loki;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddControllers(o => o.Filters.Add<ExceptionFilter>());

services.AddEndpointsApiExplorer();
services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "1.0.0",
        Title = "so",
    });
    var executingAssembly = Assembly.GetExecutingAssembly();
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{executingAssembly.GetName().Name}.xml"));

    var referencedProjectsXmlDocPaths = executingAssembly.GetReferencedAssemblies()
        .Where(assembly => assembly.Name != null &&
                           assembly.Name.StartsWith("My.Example.Project", StringComparison.InvariantCultureIgnoreCase))
        .Select(assembly => Path.Combine(AppContext.BaseDirectory, $"{assembly.Name}.xml"))
        .Where(path => File.Exists(path));
    foreach (var xmlDocPath in referencedProjectsXmlDocPaths)
    {
        options.IncludeXmlComments(xmlDocPath);
    }
});

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

services.AddMetrics();
services.AddOpenTelemetry()
    .WithMetrics(options =>
    {
        options.AddPrometheusExporter();
        options.AddRuntimeInstrumentation()
            .AddMeter("Microsoft.AspNetCore.Hosting")
            .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
            .AddMeter("System.Net.Http")
            .AddMeter("Prod");
    })
    .WithTracing(options =>
    {
        options.AddHttpClientInstrumentation();
        options.AddAspNetCoreInstrumentation();
    });

services.AddHttpLogging(o =>
    o.LoggingFields = HttpLoggingFields.RequestMethod
                      | HttpLoggingFields.RequestPath
                      | HttpLoggingFields.ResponseStatusCode
                      | HttpLoggingFields.Duration);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.GrafanaLoki("http://localhost:3100", new List<LokiLabel>
    {
        new LokiLabel { Key = "app", Value = "webapi" }
    }, propertiesAsLabels: new[] { "app" })
    .Enrich.FromLogContext()
    .CreateLogger();

var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Services.AddSingleton(logger);
builder.Logging.AddSerilog(logger).AddOpenTelemetry();

builder.Host.UseSerilog();

builder.Services.AddRazorPages();

services.AddSingleton<IJwtService, JwtService>();
services.ConfigureOptions<JwtBearerOptionsConfiguration>();
services.AddAuthorization(o => o.AddPolicy("Admin", policy => policy.RequireClaim("admin", true.ToString())));
services.AddAuthentication().AddJwtBearer();

services.AddScoped<IAuthService, AuthService>();
services.AddScoped<IUserService, UserService>();
services.AddScoped<IBookService, BookService>();
services.AddScoped<IPlaceService, PlaceService>();
services.AddScoped<IOfficeZoneSeatsService, OfficeZoneSeatsService>();
services.AddScoped<IZoneService, ZoneService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapRazorPages();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPrometheusScrapingEndpoint();
app.UseHttpLogging();
app.UseSerilogRequestLogging();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ProdContext>();
    context.Database.EnsureCreated();
}

app.Run();
