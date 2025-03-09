using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Prod.Models.Database;
using Prod.Models.Requests;
using Prod.Models.Responses;
using Xunit.Extensions.AssemblyFixture;

namespace Prod.Tests.Intergration;

public class BookTests : IAssemblyFixture<TestContainers>
{
    private readonly TestContainers fixture;
    private readonly AuthenticationHeaderValue authHeader;
    
    public BookTests(TestContainers fixture)
    {
        this.fixture = fixture;
        
        var jsonString = JsonSerializer.Serialize(_requestSignUpData);
        var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
        var response = fixture.Client.PostAsync("/auth/sign-up", content).Result;
        var authResponse = response.Content.ReadFromJsonAsync<AuthResponse>().Result;
        
        authHeader = new AuthenticationHeaderValue("Bearer", authResponse!.Token);
    }
    
    private readonly SignUpRequest _requestSignUpData = new SignUpRequest()
    {
        Name = "Admin",
        Email = "admin@admin.com",
        Password = "admin"
    };

    private async Task SeedZones()
    {
        var openSpaces = new List<OpenZone>()
        {
            new() {Capacity = 1, Description = "", }
        };
    }

    [Fact]
    public async Task ClientCanBookOpenSpace()
    {
        
    }
}