using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Prod.Models.Database;
using Prod.Models.Requests;
using Prod.Models.Responses;
using Xunit.Extensions.AssemblyFixture;

namespace Prod.Tests.Intergration;

public class AuthTests(TestContainers fixture) : IAssemblyFixture<TestContainers>
{
    private readonly SignUpRequest _requestSignUpData = new SignUpRequest()
    {
        Name = "Admin",
        Email = "admin@admin.com",
        Password = "admin"
    };

    private readonly SignUpRequest _requestSignUpEnterpriseData = new SignUpRequest()
    {
        Name = "Admin",
        Email = "admin@",
        Password = "admin"
    };

    [Fact]
    public async Task DefaultUserCanSignUpWithValidDataAndGetsRoleClient()
    {
        var jsonString = JsonSerializer.Serialize(_requestSignUpData);
        var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
        var response = await fixture.Client.PostAsync("/auth/sign-up", content);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();

        Assert.NotNull(authResponse);

        var token = authResponse.Token;

        fixture.Client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var responseGet = await fixture.Client.GetAsync("user");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var user = await responseGet.Content.ReadFromJsonAsync<User>();

        Assert.NotNull(user);

        Assert.Equal(_requestSignUpData.Name, user.Name);
        Assert.Equal(_requestSignUpData.Email, user.Email);
        Assert.Equal(Role.CLIENT, user.Role);
    }
}