using System.Net.Http.Json;
using SocketheadCleanArch.API.Models;
using SocketheadCleanArch.Tests;
using Assert = Xunit.Assert;

namespace SocketheadCleanArch.xUnit.Tests;

public class ApiTests(ApiAppFactory factory) : IClassFixture<ApiAppFactory>
{
    private readonly HttpClient Client = factory.CreateClient();

    [Theory]
    [InlineData("/test/ping")]
    public async Task Get_Ping_ShouldReturnPong(string url)
    {
        HttpResponseMessage response = await Client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
        Assert.NotNull(apiResponse);
        Assert.Equal("Pong.", apiResponse.Data);
    }

    [Fact]
    public async Task Login_ShouldReturnToken()
    {
        HttpResponseMessage response = await Client
            .PostAsJsonAsync("Auth/login", new AuthRequest
            {
                Email = "admin@foo.com", 
                Password = "Admin@123!" 
            });
        
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>();
        
        Assert.NotNull(apiResponse);
        Assert.NotNull(apiResponse.Data);
        Assert.True(apiResponse.Data.AccessToken.Length > 10);
    }
}
