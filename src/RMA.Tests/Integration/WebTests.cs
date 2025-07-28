using System.Diagnostics;
using System.Net.Http.Json;
using RMA.Tests.Utils;
using SDK.DTOs;
using Xunit.Abstractions;

namespace RMA.Tests;

public class WebTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public WebTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task GetWebResourceRootReturnsOkStatusCode()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.RMA_AppHost>();
        await using var app = await appHost.BuildAsync();
        var service = app.Services.GetRequiredService<ResourceNotificationService>();
        await app.StartAsync();

        var httpClient = app.CreateHttpClient("apiservice");
        await service.WaitForResourceAsync("apiservice", KnownResourceStates.Running)
            .WaitAsync(TimeSpan.FromSeconds(30));

        var response = await httpClient.GetAsync("/api/Recipes");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        _testOutputHelper.WriteLine("✅ [GET /api/Recipes] passed - Response OK");
    }

    [Fact]
    public async Task CanCreateAndRetrieveRecipe_Success()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.RMA_AppHost>();
        await using var app = await appHost.BuildAsync();
        await app.StartAsync();

        var httpClient = app.CreateHttpClient("apiservice");

        var newRecipe = new RecipeDto
        {
            Title = "Benchmark Pasta",
            Description = "A delicious benchmark test",
            CookingTimeMinutes = 15,
            Image = "",
            Ingredients = new List<IngredientDto> {
                new() { Content = "Noodles", Index = 0 }
            },
            Steps = new List<StepDto> {
                new() { Content = "Boil water", Index = 0 }
            },
            Tags = new List<TagDto> {
                new() { Name = "Test" }
            }
        };

        var postResponse = await httpClient.PostAsJsonAsync("/api/Recipes", newRecipe);
        postResponse.EnsureSuccessStatusCode();

        var createdId = await TestUtils.GetJsonStringPropertyAsync(postResponse, "id");
        var title = await TestUtils.GetJsonStringPropertyAsync(postResponse, "title");

        Assert.Equal("Benchmark Pasta", title);
        Assert.False(string.IsNullOrWhiteSpace(createdId));
        _testOutputHelper.WriteLine($"✅ Created recipe with ID: {createdId}");

        var getResponse = await httpClient.GetAsync($"/api/Recipes/{createdId}");
        getResponse.EnsureSuccessStatusCode();

        var ingredients = await TestUtils.GetJsonArrayAsync<List<IngredientDto>>(getResponse, "ingredients");
        var steps = await TestUtils.GetJsonArrayAsync<List<StepDto>>(getResponse, "steps");
        var tags = await TestUtils.GetJsonArrayAsync<List<TagDto>>(getResponse, "tags");

        Assert.Single(ingredients);
        Assert.Single(steps);
        Assert.Single(tags);

        _testOutputHelper.WriteLine("✅ Retrieval and structure validation passed.");
    }

    [Theory]
    [InlineData("", 10)]        // Missing title
    [InlineData("Valid", 0)]    // Invalid time
    [InlineData(null, -1)]      // Null title, negative time
    public async Task CreateRecipe_ShouldFailValidation(string? title, int time)
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.RMA_AppHost>();
        await using var app = await appHost.BuildAsync();
        await app.StartAsync();

        var httpClient = app.CreateHttpClient("apiservice");

        var recipe = new RecipeDto
        {
            Title = title!,
            Description = "Bad data",
            CookingTimeMinutes = time,
            Ingredients = new(),
            Steps = new(),
            Tags = new()
        };

        var response = await httpClient.PostAsJsonAsync("/api/Recipes", recipe);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        _testOutputHelper.WriteLine($"✅ Validation failed as expected for input [Title: {title}, Time: {time}]");
    }

    [Fact]
    public async Task Benchmark_GetAllRecipes_500Times()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.RMA_AppHost>();
        await using var app = await appHost.BuildAsync();
        await app.StartAsync();

        var httpClient = app.CreateHttpClient("apiservice");

        var sw = new Stopwatch();
        sw.Start();

        for (int i = 0; i < 500; i++)
        {
            var response = await httpClient.GetAsync("/api/Recipes");
            response.EnsureSuccessStatusCode();
        }

        sw.Stop();
        double avgMs = sw.Elapsed.TotalMilliseconds / 500;

        _testOutputHelper.WriteLine("\n🧪 Benchmark: 500 sequential GETs to /api/Recipes");
        _testOutputHelper.WriteLine($"🕒 Total: {sw.Elapsed.TotalMilliseconds:F2} ms");
        _testOutputHelper.WriteLine($"📊 Avg latency per request: {avgMs:F2} ms\n");
    }
}
