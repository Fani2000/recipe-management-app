using System.ComponentModel.DataAnnotations;

namespace SDK.Models;

public class Tag
{
    [Required]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;

    public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
}

