using System.Text.Json.Serialization;

namespace SDK.Models;

public class Ingredient
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public int Index { get; set; }

    public int RecipeId { get; set; }
    
    [JsonIgnore]
    public Recipe Recipe { get; set; } = default!;
}

