using System.Net.Http.Json;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SocketheadCleanArch.API.Models;
using TUnit.Core.Interfaces;
using Assert = TUnit.Assertions.Assert;

namespace SocketheadCleanArch.Tests.TUnit;

public class TestHooks
{
    private static int counter = 1;
    private static readonly Lock mutex = new();
    
    [BeforeEvery(Test)]
    public static void BeforeTest(TestContext testContext)
    {
        lock (mutex)
            Console.WriteLine($"Started [{counter++}]: {testContext.TestDetails.TestName}");
    }
    
    [AfterEvery(Test)]
    public static void AfterTest(TestContext testContext)
    {
        lock (mutex)
            Console.WriteLine($"Completed: {testContext.TestDetails.TestName}");
    }
}

[ClassDataSource<ApiAppFactoryTUnit>(Shared = SharedType.PerAssembly)]
public class ApiTests(ApiAppFactoryTUnit factory)
{
    
    [Test]
    public async Task Ping_ShouldReturnPong()
    {
        HttpResponseMessage response = await factory.Client.GetAsync("/test/ping");
        response.EnsureSuccessStatusCode();
        ApiResponse<string>? apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
        await Assert.That(apiResponse).IsNotNull();
        await Assert.That(apiResponse!.Data).IsEqualTo("Pong.");
    }
 
    /*
    [Test]
    [DependsOn(nameof(Ping_ShouldReturnPong))]
    public async Task Login_ShouldReturnToken()
    {
        //using var scope = factory.Services.CreateScope();
        //var service = scope.ServiceProvider.GetRequiredService<IMyService>();
        
        HttpResponseMessage response = await factory.Client
            .PostAsJsonAsync("Auth/login", new AuthRequest
            {
                Email = "user1@foo.com", 
                Password = "SocketheadRocks1!" 
            });
        
        response.EnsureSuccessStatusCode();
        ApiResponse<AuthResponse>? apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>();
        await Assert.That(apiResponse).IsNotNull();
        await Assert.That(apiResponse!.Data!.AccessToken.Length).IsGreaterThan(10);
        
        factory.BearerToken = apiResponse.Data?.AccessToken;
    }

    [Test]
    [DependsOn(nameof(Login_ShouldReturnToken))]
    public async Task TestPingAuthorized_ShouldReturnPong()
    {
        string result = await factory.GetAsync<string>("/test/ping-authorized");
        await Assert.That(result).IsEqualTo("Authorized Pong.");
    }
        */


    [Test]
    //[DependsOn(nameof(TestPingAuthorized_ShouldReturnPong))]
    public async Task MyTest()
    {
        int a = 1;
        int b = a + a;
        await Assert.That(b).IsEqualTo(2);
    }
}

public class ApiAppFactoryTUnit : ApiAppFactory, IAsyncInitializer
{
    private HttpClient? _client;
    public HttpClient Client => _client!;
    public string? BearerToken { get; set; }

    public Task InitializeAsync()
    {
        _client = CreateClient();
        return Task.CompletedTask;
    }
    
    public async Task<TResult> GetAsync<TResult>(string url)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        if (BearerToken != null)
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", BearerToken);
        HttpResponseMessage response = await Client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        ApiResponse<TResult>? apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<TResult>>();
        await Assert.That(apiResponse).IsNotNull();
        return apiResponse!.Data!;
    }
    
    public async Task<TResult> PostAsync<TResult, TPost>(string url, TPost o)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        if (BearerToken != null)
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", BearerToken);

        string json = JsonConvert.SerializeObject(o);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", BearerToken);
        try
        {
            HttpResponseMessage response = await Client.PostAsync(url,  content);
            response.EnsureSuccessStatusCode();
            ApiResponse<TResult>? apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<TResult>>();
            await Assert.That(apiResponse).IsNotNull();
            return apiResponse!.Data!;
        }
        finally
        {
            Client.DefaultRequestHeaders.Authorization = null;
        }
    }
}