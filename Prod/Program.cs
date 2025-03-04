using System.Configuration;
using System.Net;
using System.Reflection;
using AspNetCore.Yandex.ObjectStorage.Extensions;
using Blazored.LocalStorage;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Microsoft.OpenApi.Models;
using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Prod.Components;
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
        .Where(File.Exists);
    foreach (var xmlDocPath in referencedProjectsXmlDocPaths)
    {
        options.IncludeXmlComments(xmlDocPath);
    }

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. <br/>
                      Enter 'Bearer' [space] and then your token in the text input below. <br/>
                      Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            []
        }
    });
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
        options
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
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

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.GrafanaLoki("http://loki:3100", new List<LokiLabel>
    {
        new LokiLabel { Key = "app", Value = "webapi" }
    }, propertiesAsLabels: new[] { "app" })
    .Enrich.FromLogContext()
    .CreateLogger();

var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Services.AddSingleton(logger);
services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = builder.Configuration.GetSection("Redis:Configuration").Value;
    return ConnectionMultiplexer.Connect(configuration);
});

builder.Logging.AddSerilog(logger).AddOpenTelemetry();

builder.Host.UseSerilog();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

services.AddBlazoredLocalStorage();

services.AddSingleton<IJwtService, JwtService>();
services.ConfigureOptions<JwtBearerOptionsConfiguration>();
services.AddAuthorization(o => o.AddPolicy("Admin", policy => policy.RequireClaim("admin", true.ToString())));
services.AddAuthentication().AddJwtBearer();
services.AddYandexObjectStorage(builder.Configuration);

services.AddHostedService<LateService>();

services.AddScoped<IQrCodeService, QrCodeService>();
services.AddScoped<IAuthService, AuthService>();
services.AddScoped<IUserService, UserService>();
services.AddScoped<IBookService, BookService>();
services.AddScoped<IOfficeZoneSeatsService, OfficeZoneSeatsService>();
services.AddScoped<IZoneService, ZoneService>();
services.AddScoped<IDecorationService, DecorationService>();
services.AddScoped<IRequestService, RequestService>();
services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.MapPrometheusScrapingEndpoint();
app.UseSerilogRequestLogging();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ProdContext>();
    context.Database.EnsureCreated();
}

app.Run();
