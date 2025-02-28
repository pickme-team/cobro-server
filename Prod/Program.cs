using Microsoft.EntityFrameworkCore;
using Npgsql;
using Prod.Services;

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
