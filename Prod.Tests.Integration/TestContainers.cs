using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Net.Mime;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using DotNet.Testcontainers.Networks;
using Microsoft.Extensions.Logging;
using Npgsql;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Testcontainers.PostgreSql;

namespace Prod.Tests.Intergration;

public class TestContainers : IAsyncLifetime
{
    private const string pgImage = "postgres:latest";

    private static readonly CommonDirectoryPath Solution = CommonDirectoryPath.GetSolutionDirectory();
    private static readonly StringLogger Logger = new StringLogger();
    private static IReadOnlyDictionary<string, string> Envs => ExtractEnvironmentVariables();

    private static readonly INetwork Network =
        new NetworkBuilder()
            .WithDriver(NetworkDriver.Bridge)
            .Build();

    public Uri BaseAddress { get; private set; } = null!;
    public HttpClient Client { get; private set; } = null!;

    private static readonly IFutureDockerImage image =
        new ImageFromDockerfileBuilder()
            .WithDockerfileDirectory(Solution, string.Empty)
            .WithDockerfile("./Prod/Dockerfile")
            .WithName("prod-test")
            .WithCleanUp(false)
            .WithDeleteIfExists(true)
            .WithLogger(Logger)
            .Build();

    private static readonly PostgreSqlContainer _dbContainer =
        new PostgreSqlBuilder()
            .WithImage(pgImage)
            .WithPortBinding(8059, 5432)
            .WithEnvironment(Envs)
            .WithNetwork(Network)
            .WithNetworkAliases("postgres")
            .Build();

    private static readonly IContainer _appContainer =
        new ContainerBuilder()
            .WithImage(image)
            .WithNetwork(Network)
            .WithPortBinding(8080, true)
            .WithEnvironment(Envs)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(8080))
            .DependsOn(_dbContainer)
            .Build();

    private static IReadOnlyDictionary<string, string> ExtractEnvironmentVariables()
    {
        var path = Path.Combine(Solution.DirectoryPath, "Prod/.env");
        var lines = File.ReadAllLines(path);

        var dict = new Dictionary<string, string>();
        foreach (var line in lines)
        {
            var kayValue = line.Split('=');
            dict[kayValue[0]] = kayValue[1];
        }

        return dict;
    }

    private async Task RunAsync()
    {
        try
        {
            var imageTask = image.CreateAsync();
            var dbTask = _dbContainer.StartAsync();

            await Task.WhenAll(imageTask, dbTask);
        }
        catch (Exception ex)
        {
            var errors = Logger.Data;
            var msg = $"Errors:\n{errors}";
            throw new Exception(msg, ex);
        }
    }

    public async Task InitializeAsync()
    {
        try
        {
            await RunAsync();

            await _appContainer.StartAsync();
            Client = new HttpClient();
            BaseAddress = new Uri($"http://{_appContainer.Hostname}:{_appContainer.GetMappedPublicPort(8080)}");
            Client.BaseAddress = BaseAddress;
        }
        finally
        {
            if (_appContainer.State != TestcontainersStates.Undefined)
            {
                var (stdout, stderr) = _appContainer.GetLogsAsync().Result;
                if (!string.IsNullOrEmpty(stderr))
                {
                    throw new Exception(stderr);
                }
            }
        }
    }

    public async Task DisposeAsync()
    {
        await _appContainer.DisposeAsync();
        await _dbContainer.DisposeAsync();
    }
}