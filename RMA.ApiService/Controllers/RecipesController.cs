using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using SDK.DTOs;
using SDK.Models;
using SDK.RecipesContext;

namespace RMA.ApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        private readonly RecipesDbContext _context;
        private readonly IDistributedCache _cache;
        private readonly ILogger<RecipesController> _logger;

        public RecipesController(
            RecipesDbContext context,
            IDistributedCache cache,
            ILogger<RecipesController> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }

        // GET: api/Recipes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecipeDto>>> GetRecipes(
            [FromQuery] string? tag = null,
            [FromQuery] string? search = null,
            [FromQuery] int? maxCookingTime = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool ascending = true)
        {
            var query = _context.Recipes
                .Include(r => r.Ingredients)
                .Include(r => r.Steps)
                .Include(r => r.Tags)
                .AsQueryable();

            // Apply filters
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

            // Apply sorting
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
            
            // Map to DTOs
            var recipeDtos = recipes.Select(recipe => new RecipeDto
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Description = recipe.Description,
                CookingTimeMinutes = recipe.CookingTimeMinutes,
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

        // GET: api/Recipes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RecipeDto>> GetRecipe(int id)
        {
            var recipe = await _context.Recipes
                .Include(r => r.Ingredients)
                .Include(r => r.Steps)
                .Include(r => r.Tags)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (recipe == null)
            {
                return NotFound();
            }

            // Map to DTO
            var recipeDto = new RecipeDto
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Description = recipe.Description,
                CookingTimeMinutes = recipe.CookingTimeMinutes,
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

        // POST: api/Recipes
        [HttpPost]
        public async Task<ActionResult<RecipeDto>> PostRecipe(RecipeDto recipeDto)
        {
            // Map DTO to entity
            var recipe = new Recipe
            {
                Title = recipeDto.Title,
                Description = recipeDto.Description,
                CookingTimeMinutes = recipeDto.CookingTimeMinutes,
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

            // Handle tags
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

            // Invalidate cache
            await InvalidateCaches(recipe);

            // Return the created entity as DTO
            recipeDto.Id = recipe.Id;
            return CreatedAtAction(nameof(GetRecipe), new { id = recipe.Id }, recipeDto);
        }

        // PUT: api/Recipes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRecipe(int id, RecipeDto recipeDto)
        {
            if (id != recipeDto.Id)
            {
                return BadRequest();
            }

            var existingRecipe = await _context.Recipes
                .Include(r => r.Ingredients)
                .Include(r => r.Steps)
                .Include(r => r.Tags)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (existingRecipe == null)
            {
                return NotFound();
            }

            // Store old recipe for cache invalidation
            var oldRecipe = new Recipe
            {
                Id = existingRecipe.Id,
                Title = existingRecipe.Title,
                Description = existingRecipe.Description,
                CookingTimeMinutes = existingRecipe.CookingTimeMinutes,
                Tags = new List<Tag>(existingRecipe.Tags)
            };

            // Update existing recipe
            existingRecipe.Title = recipeDto.Title;
            existingRecipe.Description = recipeDto.Description;
            existingRecipe.CookingTimeMinutes = recipeDto.CookingTimeMinutes;

            // Update ingredients
            _context.Ingredients.RemoveRange(existingRecipe.Ingredients);
            existingRecipe.Ingredients = recipeDto.Ingredients.Select(i => new Ingredient
            {
                Content = i.Content,
                Index = i.Index,
                RecipeId = existingRecipe.Id
            }).ToList();

            // Update steps
            _context.Steps.RemoveRange(existingRecipe.Steps);
            existingRecipe.Steps = recipeDto.Steps.Select(s => new Step
            {
                Content = s.Content,
                Index = s.Index,
                RecipeId = existingRecipe.Id
            }).ToList();

            // Update tags
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
                
                // Invalidate cache
                await InvalidateCaches(existingRecipe, oldRecipe);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await RecipeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Recipes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            var recipe = await _context.Recipes
                .Include(r => r.Tags)
                .FirstOrDefaultAsync(r => r.Id == id);
                
            if (recipe == null)
            {
                return NotFound();
            }

            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();
            
            // Invalidate cache
            await InvalidateCaches(recipe);

            return NoContent();
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
            // Invalidate all recipes cache
            await _cache.RemoveAsync("recipes_all");
            
            // Invalidate tag-specific caches for both old and new recipe
            var allTags = new HashSet<string>();
            
            // Add tags from new recipe
            foreach (var tag in recipe.Tags)
            {
                allTags.Add(tag.Name.ToLower());
            }
            
            // Add tags from old recipe if it exists
            if (oldRecipe != null)
            {
                foreach (var tag in oldRecipe.Tags)
                {
                    allTags.Add(tag.Name.ToLower());
                }
            }
            
            // Invalidate cache for each tag
            foreach (var tag in allTags)
            {
                var cacheKey = BuildCacheKey("recipes", new Dictionary<string, string?> { { "tag", tag } });
                await _cache.RemoveAsync(cacheKey);
            }
            
            // Invalidate specific recipe cache
            await _cache.RemoveAsync($"recipe_{recipe.Id}");
        }
    }
}