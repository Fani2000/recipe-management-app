namespace SDK.Models;

public class Step
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public int Index { get; set; }

    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; } = default!;
}

