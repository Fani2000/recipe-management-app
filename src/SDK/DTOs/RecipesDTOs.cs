using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SDK.DTOs;

public class RecipeDto
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(150, ErrorMessage = "Title can't be longer than 150 characters.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(1000, ErrorMessage = "Description can't be longer than 1000 characters.")]
    public string Description { get; set; } = string.Empty;

    public string Image { get; set; } = string.Empty;

    [Required(ErrorMessage = "Cooking time is required.")]
    [Range(1, 1440, ErrorMessage = "Cooking time must be between 1 and 1440 minutes.")]
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