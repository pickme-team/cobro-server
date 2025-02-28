using Xunit.Extensions.AssemblyFixture;

namespace Prod.Tests.Intergration;

public class PingTests(TestContainers fixture) : IAssemblyFixture<TestContainers>
{
    [Fact]
    public async Task PingTest()
    {
        var response = await fixture.Client.GetAsync("ping");
        Assert.True(response.IsSuccessStatusCode, "/ping was not successful");
    }
}