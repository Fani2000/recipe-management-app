using SDK.Models;
using Xunit;

namespace RMA.Tests.Unit;

public class ModelTests
{
    [Fact]
    public void Recipe_CanSetAndGetProperties()
    {
        var recipe = new Recipe
        {
            Id = 1,
            Title = "Test Recipe",
            Description = "Test Description",
            Image = "test.jpg",
            CookingTimeMinutes = 30
        };

        Assert.Equal(1, recipe.Id);
        Assert.Equal("Test Recipe", recipe.Title);
        Assert.Equal("Test Description", recipe.Description);
        Assert.Equal("test.jpg", recipe.Image);
        Assert.Equal(30, recipe.CookingTimeMinutes);
        Assert.Empty(recipe.Ingredients);
        Assert.Empty(recipe.Steps);
        Assert.Empty(recipe.Tags);
    }

    [Fact]
    public void Ingredient_CanSetAndGetProperties()
    {
        var ingredient = new Ingredient
        {
            Id = 2,
            Content = "Flour",
            Index = 0,
            RecipeId = 1
        };

        Assert.Equal(2, ingredient.Id);
        Assert.Equal("Flour", ingredient.Content);
        Assert.Equal(0, ingredient.Index);
        Assert.Equal(1, ingredient.RecipeId);
    }

    [Fact]
    public void Step_CanSetAndGetProperties()
    {
        var step = new Step
        {
            Id = 3,
            Content = "Mix ingredients",
            Index = 1,
            RecipeId = 1
        };

        Assert.Equal(3, step.Id);
        Assert.Equal("Mix ingredients", step.Content);
        Assert.Equal(1, step.Index);
        Assert.Equal(1, step.RecipeId);
    }

    [Fact]
    public void Recipe_Ingredient_Step_Collections_Work()
    {
        var recipe = new Recipe { Title = "Test" };
        var ingredient = new Ingredient { Content = "Sugar" };
        var step = new Step { Content = "Stir" };

        recipe.Ingredients.Add(ingredient);
        recipe.Steps.Add(step);

        Assert.Single(recipe.Ingredients);
        Assert.Single(recipe.Steps);
        Assert.Equal("Sugar", recipe.Ingredients.First().Content);
        Assert.Equal("Stir", recipe.Steps.First().Content);
    }
}
