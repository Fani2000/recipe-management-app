using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using RMA.ApiService.Services;
using SDK.DTOs;
using SDK.Models;
using SDK.RecipesContext;

namespace RMA.Tests.Unit;

public class RecipeServiceTests
{
    private RecipeService CreateService(
        RecipesDbContext? dbContext = null,
        IDistributedCache? cache = null,
        ILogger<RecipeService>? logger = null)
    {
        dbContext ??= new Mock<RecipesDbContext>(new DbContextOptions<RecipesDbContext>()).Object;
        cache ??= new Mock<IDistributedCache>().Object;
        logger ??= new Mock<ILogger<RecipeService>>().Object;
        return new RecipeService(dbContext, cache, logger);
    }

    [Fact]
    public async Task GetRecipesAsync_ReturnsList()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<RecipesDbContext>()
            .UseInMemoryDatabase(databaseName: "GetRecipesAsync_ReturnsList")
            .Options;
        using var context = new RecipesDbContext(options);
        context.Recipes.Add(new Recipe { Title = "R1", Description = "D1", CookingTimeMinutes = 5 });
        context.SaveChanges();

        var service = CreateService(context);

        // Act
        var result = await service.GetRecipesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("R1", result[0].Title);
    }

    [Fact]
    public async Task GetRecipeAsync_ReturnsRecipeDto_WhenExists()
    {
        var options = new DbContextOptionsBuilder<RecipesDbContext>()
            .UseInMemoryDatabase(databaseName: "GetRecipeAsync_ReturnsRecipeDto")
            .Options;
        using var context = new RecipesDbContext(options);
        var recipe = new Recipe { Title = "R2", Description = "D2", CookingTimeMinutes = 10 };
        context.Recipes.Add(recipe);
        context.SaveChanges();

        var service = CreateService(context);

        var result = await service.GetRecipeAsync(recipe.Id);

        Assert.NotNull(result);
        Assert.Equal("R2", result.Title);
    }

    [Fact]
    public async Task CreateRecipeAsync_AddsRecipe()
    {
        var options = new DbContextOptionsBuilder<RecipesDbContext>()
            .UseInMemoryDatabase(databaseName: "CreateRecipeAsync_AddsRecipe")
            .Options;
        using var context = new RecipesDbContext(options);

        var service = CreateService(context);

        var dto = new RecipeDto
        {
            Title = "R3",
            Description = "D3",
            CookingTimeMinutes = 15,
            Ingredients = new List<IngredientDto>(),
            Steps = new List<StepDto>(),
            Tags = new List<TagDto>()
        };

        var result = await service.CreateRecipeAsync(dto);

        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("R3", result.Title);
        Assert.Single(context.Recipes);
    }

    [Fact]
    public async Task UpdateRecipeAsync_UpdatesRecipe()
    {
        var options = new DbContextOptionsBuilder<RecipesDbContext>()
            .UseInMemoryDatabase(databaseName: "UpdateRecipeAsync_UpdatesRecipe")
            .Options;
        using var context = new RecipesDbContext(options);
        var recipe = new Recipe { Title = "Old", Description = "Old", CookingTimeMinutes = 5 };
        context.Recipes.Add(recipe);
        context.SaveChanges();

        var service = CreateService(context);

        var dto = new RecipeDto
        {
            Id = recipe.Id,
            Title = "New",
            Description = "New",
            CookingTimeMinutes = 20,
            Ingredients = new List<IngredientDto>(),
            Steps = new List<StepDto>(),
            Tags = new List<TagDto>()
        };

        var updated = await service.UpdateRecipeAsync(recipe.Id, dto);

        Assert.True(updated);
        var updatedRecipe = context.Recipes.Find(recipe.Id);
        Assert.Equal("New", updatedRecipe.Title);
        Assert.Equal(20, updatedRecipe.CookingTimeMinutes);
    }

    [Fact]
    public async Task DeleteRecipeAsync_RemovesRecipe()
    {
        var options = new DbContextOptionsBuilder<RecipesDbContext>()
            .UseInMemoryDatabase(databaseName: "DeleteRecipeAsync_RemovesRecipe")
            .Options;
        using var context = new RecipesDbContext(options);
        var recipe = new Recipe { Title = "ToDelete", Description = "Del", CookingTimeMinutes = 1 };
        context.Recipes.Add(recipe);
        context.SaveChanges();

        var service = CreateService(context);

        var deleted = await service.DeleteRecipeAsync(recipe.Id);

        Assert.True(deleted);
        Assert.Empty(context.Recipes);
    }
}
