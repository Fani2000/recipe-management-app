using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using SDK.DTOs;
using SDK.Models;
using SDK.RecipesContext;

namespace RMA.ApiService.Services
{
    public class RecipeService
    {
        private readonly RecipesDbContext _context;
        private readonly IDistributedCache _cache;
        private readonly ILogger<RecipeService> _logger;

        public RecipeService(
            RecipesDbContext context,
            IDistributedCache cache,
            ILogger<RecipeService> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }

        public async Task<List<RecipeDto>> GetRecipesAsync(
            string? tag = null,
            string? search = null,
            int? maxCookingTime = null,
            string? sortBy = null,
            bool ascending = true)
        {
            var query = _context.Recipes
                .Include(r => r.Ingredients)
                .Include(r => r.Steps)
                .Include(r => r.Tags)
                .AsQueryable();

            if (!string.IsNullOrEmpty(tag))
            {
                query = query.Where(r => r.Tags.Any(t => t.Name.ToLower() == tag.ToLower()));
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(r => r.Title.Contains(search) || r.Description.Contains(search));
            }

            if (maxCookingTime.HasValue)
            {
                query = query.Where(r => r.CookingTimeMinutes <= maxCookingTime.Value);
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "title":
                        query = ascending
                            ? query.OrderBy(r => r.Title)
                            : query.OrderByDescending(r => r.Title);
                        break;
                    case "cookingtime":
                        query = ascending
                            ? query.OrderBy(r => r.CookingTimeMinutes)
                            : query.OrderByDescending(r => r.CookingTimeMinutes);
                        break;
                    default:
                        query = ascending
                            ? query.OrderBy(r => r.Id)
                            : query.OrderByDescending(r => r.Id);
                        break;
                }
            }
            else
            {
                query = ascending
                    ? query.OrderBy(r => r.Id)
                    : query.OrderByDescending(r => r.Id);
            }

            var recipes = await query.ToListAsync();

            var recipeDtos = recipes.Select(recipe => new RecipeDto
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Description = recipe.Description,
                CookingTimeMinutes = recipe.CookingTimeMinutes,
                Image = recipe.Image,
                Ingredients = recipe.Ingredients.Select(i => new IngredientDto
                {
                    Id = i.Id,
                    Content = i.Content,
                    Index = i.Index,
                    RecipeId = i.RecipeId
                }).ToList(),
                Steps = recipe.Steps.Select(s => new StepDto
                {
                    Id = s.Id,
                    Content = s.Content,
                    Index = s.Index,
                    RecipeId = s.RecipeId
                }).ToList(),
                Tags = recipe.Tags.Select(t => new TagDto
                {
                    Id = t.Id,
                    Name = t.Name
                }).ToList()
            }).ToList();

            return recipeDtos;
        }

        public async Task<RecipeDto?> GetRecipeAsync(int id)
        {
            var recipe = await _context.Recipes
                .Include(r => r.Ingredients)
                .Include(r => r.Steps)
                .Include(r => r.Tags)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (recipe == null)
            {
                return null;
            }

            var recipeDto = new RecipeDto
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Description = recipe.Description,
                CookingTimeMinutes = recipe.CookingTimeMinutes,
                Image = recipe.Image,
                Ingredients = recipe.Ingredients.Select(i => new IngredientDto
                {
                    Id = i.Id,
                    Content = i.Content,
                    Index = i.Index,
                    RecipeId = i.RecipeId
                }).ToList(),
                Steps = recipe.Steps.Select(s => new StepDto
                {
                    Id = s.Id,
                    Content = s.Content,
                    Index = s.Index,
                    RecipeId = s.RecipeId
                }).ToList(),
                Tags = recipe.Tags.Select(t => new TagDto
                {
                    Id = t.Id,
                    Name = t.Name
                }).ToList()
            };

            return recipeDto;
        }

        public async Task<RecipeDto> CreateRecipeAsync(RecipeDto recipeDto)
        {
            var recipe = new Recipe
            {
                Title = recipeDto.Title,
                Description = recipeDto.Description,
                CookingTimeMinutes = recipeDto.CookingTimeMinutes,
                Image = recipeDto.Image,
                Ingredients = recipeDto.Ingredients.Select(i => new Ingredient
                {
                    Content = i.Content,
                    Index = i.Index
                }).ToList(),
                Steps = recipeDto.Steps.Select(s => new Step
                {
                    Content = s.Content,
                    Index = s.Index
                }).ToList()
            };

            foreach (var tagDto in recipeDto.Tags)
            {
                var existingTag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagDto.Name);
                if (existingTag == null)
                {
                    recipe.Tags.Add(new Tag { Name = tagDto.Name });
                }
                else
                {
                    recipe.Tags.Add(existingTag);
                }
            }

            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();

            await InvalidateCaches(recipe);

            recipeDto.Id = recipe.Id;
            return recipeDto;
        }

        public async Task<bool> UpdateRecipeAsync(int id, RecipeDto recipeDto)
        {
            if (id != recipeDto.Id)
            {
                return false;
            }

            var existingRecipe = await _context.Recipes
                .Include(r => r.Ingredients)
                .Include(r => r.Steps)
                .Include(r => r.Tags)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (existingRecipe == null)
            {
                return false;
            }

            var oldRecipe = new Recipe
            {
                Id = existingRecipe.Id,
                Title = existingRecipe.Title,
                Description = existingRecipe.Description,
                Image = existingRecipe.Image,
                CookingTimeMinutes = existingRecipe.CookingTimeMinutes,
                Tags = new List<Tag>(existingRecipe.Tags)
            };

            existingRecipe.Title = recipeDto.Title;
            existingRecipe.Description = recipeDto.Description;
            existingRecipe.Image = recipeDto.Image;
            existingRecipe.CookingTimeMinutes = recipeDto.CookingTimeMinutes;

            _context.Ingredients.RemoveRange(existingRecipe.Ingredients);
            existingRecipe.Ingredients = recipeDto.Ingredients.Select(i => new Ingredient
            {
                Content = i.Content,
                Index = i.Index,
                RecipeId = existingRecipe.Id
            }).ToList();

            _context.Steps.RemoveRange(existingRecipe.Steps);
            existingRecipe.Steps = recipeDto.Steps.Select(s => new Step
            {
                Content = s.Content,
                Index = s.Index,
                RecipeId = existingRecipe.Id
            }).ToList();

            existingRecipe.Tags.Clear();
            foreach (var tagDto in recipeDto.Tags)
            {
                var existingTag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagDto.Name);
                if (existingTag == null)
                {
                    existingRecipe.Tags.Add(new Tag { Name = tagDto.Name });
                }
                else
                {
                    existingRecipe.Tags.Add(existingTag);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                await InvalidateCaches(existingRecipe, oldRecipe);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await RecipeExists(id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }

            return true;
        }

        public async Task<bool> DeleteRecipeAsync(int id)
        {
            var recipe = await _context.Recipes
                .Include(r => r.Tags)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (recipe == null)
            {
                return false;
            }

            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();

            await InvalidateCaches(recipe);

            return true;
        }

        private async Task<bool> RecipeExists(int id)
        {
            return await _context.Recipes.AnyAsync(e => e.Id == id);
        }

        private string BuildCacheKey(string prefix, Dictionary<string, string?> parameters)
        {
            var paramString = string.Join("_", parameters
                .Where(p => p.Value != null)
                .OrderBy(p => p.Key)
                .Select(p => $"{p.Key}:{p.Value}"));

            return $"{prefix}_{paramString}";
        }

        private async Task InvalidateCaches(Recipe recipe, Recipe? oldRecipe = null)
        {
            await _cache.RemoveAsync("recipes_all");

            var allTags = new HashSet<string>();

            foreach (var tag in recipe.Tags)
            {
                allTags.Add(tag.Name.ToLower());
            }

            if (oldRecipe != null)
            {
                foreach (var tag in oldRecipe.Tags)
                {
                    allTags.Add(tag.Name.ToLower());
                }
            }

            foreach (var tag in allTags)
            {
                var cacheKey = BuildCacheKey("recipes", new Dictionary<string, string?> { { "tag", tag } });
                await _cache.RemoveAsync(cacheKey);
            }

            await _cache.RemoveAsync($"recipe_{recipe.Id}");
        }
    }
}
