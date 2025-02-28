using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
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
