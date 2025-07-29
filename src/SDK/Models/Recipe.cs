using System.ComponentModel.DataAnnotations;

namespace SDK.Models;

public class Recipe
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [StringLength(150, ErrorMessage = "Title can't be longer than 150 characters.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(1000, ErrorMessage = "Description can't be longer than 1000 characters.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Image URL is required.")]
    public string Image { get; set; } = string.Empty;

    [Required(ErrorMessage = "Cooking time is required.")]
    [Range(1, 1440, ErrorMessage = "Cooking time must be between 1 and 1440 minutes.")]
    public int CookingTimeMinutes { get; set; }

    public ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();

    public ICollection<Step> Steps { get; set; } = new List<Step>();

    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
}