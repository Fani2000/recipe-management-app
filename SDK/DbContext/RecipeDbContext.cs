using Microsoft.EntityFrameworkCore;
using SDK.Models;

namespace SDK.RecipesDbContext;

public class RecipesDbContext : DbContext
{
    public RecipesDbContext(DbContextOptions<RecipesDbContext> options)
        : base(options) { }

    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<Step> Steps => Set<Step>();
    public DbSet<Tag> Tags => Set<Tag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Recipe>()
            .HasMany(r => r.Ingredients)
            .WithOne(i => i.Recipe)
            .HasForeignKey(i => i.RecipeId);

        modelBuilder.Entity<Recipe>()
            .HasMany(r => r.Steps)
            .WithOne(s => s.Recipe)
            .HasForeignKey(s => s.RecipeId);

        modelBuilder.Entity<Recipe>()
            .HasMany(r => r.Tags)
            .WithMany(t => t.Recipes)
            .UsingEntity("RecipeTags",
                l => l.HasOne(typeof(Tag)).WithMany().HasForeignKey("TagId"),
                r => r.HasOne(typeof(Recipe)).WithMany().HasForeignKey("RecipeId"),
                j => j.HasKey("RecipeId", "TagId")
            );
    }
}