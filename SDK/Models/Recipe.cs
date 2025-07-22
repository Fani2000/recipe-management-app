namespace SDK.Models;

public class Recipe
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    public int CookingTimeMinutes { get; set; }

    public ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
    public ICollection<Step> Steps { get; set; } = new List<Step>();
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
}

