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
        .Where(assembly => assembly.Name != null && assembly.Name.StartsWith("My.Example.Project", StringComparison.InvariantCultureIgnoreCase))
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
builder.Logging.AddSerilog().AddOpenTelemetry();

services.AddSingleton<IJwtService, JwtService>();
services.ConfigureOptions<JwtBearerOptionsConfiguration>();
services.AddAuthorization();
services.AddAuthentication().AddJwtBearer();

services.AddScoped<IAuthService, AuthService>();
services.AddScoped<IUserService, UserService>();
services.AddScoped<IBookService, BookService>();
services.AddScoped<IPlaceService, PlaceService>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPrometheusScrapingEndpoint();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ProdContext>();
    context.Database.EnsureCreated();
}

app.Run();
