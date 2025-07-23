using System.Collections.Generic;

namespace SDK.DTOs;

public class RecipeDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public int CookingTimeMinutes { get; set; }
    public List<IngredientDto> Ingredients { get; set; } = new();
    public List<StepDto> Steps { get; set; } = new();
    public List<TagDto> Tags { get; set; } = new();
}

public class IngredientDto
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public int Index { get; set; }
    public int RecipeId { get; set; }
}

public class StepDto
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public int Index { get; set; }
    public int RecipeId { get; set; }
}

public class TagDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}