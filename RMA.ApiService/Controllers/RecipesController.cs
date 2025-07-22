using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using SDK.Models;
using SDK.RecipesDbContext;
using System.Text.Json;

namespace RMA.ApiService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RecipesController : ControllerBase
{
    private readonly RecipesDbContext _context;
    private readonly IDistributedCache _cache;
    private readonly ILogger<RecipesController> _logger;
    private readonly DistributedCacheEntryOptions _cacheOptions;

    public RecipesController(
        RecipesDbContext context,
        IDistributedCache cache,
        ILogger<RecipesController> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
        _cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        };
    }

    // GET: api/recipes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Recipe>>> GetRecipes(
        [FromQuery] string? tag = null,
        [FromQuery] string? search = null,
        [FromQuery] int? maxCookingTime = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool ascending = true)
    {
        // Build a cache key based on all query parameters
        var cacheKey = BuildCacheKey("recipes", new Dictionary<string, string?>
        {
            { "tag", tag },
            { "search", search },
            { "maxCookingTime", maxCookingTime?.ToString() },
            { "sortBy", sortBy },
            { "ascending", ascending.ToString() }
        });
        
        // Try to get from cache first
        var cachedData = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedData))
        {
            _logger.LogInformation("Recipes retrieved from cache with key: {CacheKey}", cacheKey);
            return Ok(JsonSerializer.Deserialize<List<Recipe>>(cachedData, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }));
        }
        
        // If not in cache, query from database
        var query = _context.Recipes
            .Include(r => r.Tags)
            .Include(r => r.Ingredients)
            .AsQueryable();

        // Apply tag filter if provided
        if (!string.IsNullOrEmpty(tag))
        {
            query = query.Where(r => r.Tags.Any(t => t.Name.ToLower() == tag.ToLower()));
        }

        // Apply search filter if provided
        if (!string.IsNullOrEmpty(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(r => 
                r.Title.ToLower().Contains(searchLower) || 
                r.Ingredients.Any(i => i.Content.ToLower().Contains(searchLower)));
        }

        // Apply cooking time filter if provided
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
            // Default sorting by ID if no sort specified
            query = ascending 
                ? query.OrderBy(r => r.Id) 
                : query.OrderByDescending(r => r.Id);
        }

        var recipes = await query.ToListAsync();
        
        // Store in cache for future requests
        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(recipes),
            _cacheOptions);
            
        return recipes;
    }
    
    // GET: api/recipes/search
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Recipe>>> SearchRecipes([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest("Search query cannot be empty");
        }
        
        var cacheKey = $"search-{query.ToLower().Trim()}";
        
        // Try to get from cache first
        var cachedData = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedData))
        {
            _logger.LogInformation("Search results retrieved from cache for query: {Query}", query);
            return Ok(JsonSerializer.Deserialize<List<Recipe>>(cachedData,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }));
        }
        
        // If not in cache, perform search
        var searchLower = query.ToLower().Trim();
        var results = await _context.Recipes
            .Include(r => r.Tags)
            .Include(r => r.Ingredients)
            .Where(r => 
                r.Title.ToLower().Contains(searchLower) || 
                r.Ingredients.Any(i => i.Content.ToLower().Contains(searchLower)) ||
                r.Tags.Any(t => t.Name.ToLower().Contains(searchLower)))
            .ToListAsync();
            
        // Store in cache for future requests
        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(results),
            _cacheOptions);
            
        return results;
    }

    // GET: api/recipes/tags
    [HttpGet("tags")]
    public async Task<ActionResult<IEnumerable<Tag>>> GetAllTags()
    {
        var cacheKey = "all-tags";
        
        // Try to get from cache first
        var cachedData = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedData))
        {
            _logger.LogInformation("Tags retrieved from cache");
            return Ok(JsonSerializer.Deserialize<List<Tag>>(cachedData,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }));
        }
        
        // If not in cache, get from database
        var tags = await _context.Tags
            .OrderBy(t => t.Name)
            .ToListAsync();
            
        // Store in cache for future requests
        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(tags),
            _cacheOptions);
            
        return tags;
    }

    // GET: api/recipes/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Recipe>> GetRecipe(int id)
    {
        var cacheKey = $"recipe-{id}";
        
        // Try to get from cache first
        var cachedData = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedData))
        {
            _logger.LogInformation("Recipe {Id} retrieved from cache", id);
            return Ok(JsonSerializer.Deserialize<Recipe>(cachedData,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }));
        }
        
        // If not in cache, get from database with related data
        var recipe = await _context.Recipes
            .Include(r => r.Tags)
            .Include(r => r.Ingredients.OrderBy(i => i.Index))
            .Include(r => r.Steps.OrderBy(s => s.Index))
            .FirstOrDefaultAsync(r => r.Id == id);

        if (recipe == null)
            return NotFound();
            
        // Store in cache for future requests
        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(recipe),
            _cacheOptions);

        return recipe;
    }

    // POST: api/recipes
    [HttpPost]
    public async Task<ActionResult<Recipe>> PostRecipe(Recipe recipe)
    {
        // Set indices for ordered collections if not already set
        if (recipe.Ingredients != null)
        {
            int index = 0;
            foreach (var ingredient in recipe.Ingredients)
            {
                ingredient.Index = index++;
            }
        }
        
        if (recipe.Steps != null)
        {
            int index = 0;
            foreach (var step in recipe.Steps)
            {
                step.Index = index++;
            }
        }
        
        _context.Recipes.Add(recipe);
        await _context.SaveChangesAsync();
        
        // Invalidate relevant caches
        await InvalidateCaches(recipe);

        return CreatedAtAction(nameof(GetRecipe), new { id = recipe.Id }, recipe);
    }

    // PUT: api/recipes/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutRecipe(int id, Recipe recipe)
    {
        if (id != recipe.Id)
            return BadRequest();

        // Get the existing recipe to check its tags
        var existingRecipe = await _context.Recipes
            .Include(r => r.Tags)
            .Include(r => r.Ingredients)
            .Include(r => r.Steps)
            .FirstOrDefaultAsync(r => r.Id == id);
            
        if (existingRecipe == null)
            return NotFound();
            
        // Set indices for ordered collections if not already set
        if (recipe.Ingredients != null)
        {
            int index = 0;
            foreach (var ingredient in recipe.Ingredients)
            {
                ingredient.Index = index++;
            }
        }
        
        if (recipe.Steps != null)
        {
            int index = 0;
            foreach (var step in recipe.Steps)
            {
                step.Index = index++;
            }
        }
        
        // Update the recipe and its related entities
        _context.Entry(existingRecipe).State = EntityState.Detached;
        _context.Entry(recipe).State = EntityState.Modified;
        
        try
        {
            await _context.SaveChangesAsync();
            
            // Invalidate relevant caches
            await InvalidateCaches(recipe, existingRecipe);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await RecipeExists(id))
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }

    // DELETE: api/recipes/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRecipe(int id)
    {
        var recipe = await _context.Recipes
            .Include(r => r.Tags)
            .FirstOrDefaultAsync(r => r.Id == id);
            
        if (recipe == null)
            return NotFound();

        _context.Recipes.Remove(recipe);
        await _context.SaveChangesAsync();
        
        // Invalidate relevant caches
        await InvalidateCaches(recipe);

        return NoContent();
    }
    
    // Helper methods
    private async Task<bool> RecipeExists(int id)
    {
        return await _context.Recipes.AnyAsync(e => e.Id == id);
    }
    
    private string BuildCacheKey(string prefix, Dictionary<string, string?> parameters)
    {
        var filteredParams = parameters
            .Where(p => !string.IsNullOrEmpty(p.Value))
            .OrderBy(p => p.Key)
            .Select(p => $"{p.Key}={p.Value}");
            
        var paramString = string.Join("-", filteredParams);
        return string.IsNullOrEmpty(paramString) ? prefix : $"{prefix}-{paramString}";
    }
    
    private async Task InvalidateCaches(Recipe recipe, Recipe? oldRecipe = null)
    {
        // List of cache keys to invalidate
        var keysToInvalidate = new HashSet<string>();
        
        // Add specific recipe cache
        keysToInvalidate.Add($"recipe-{recipe.Id}");
        
        // Add all-recipes and all-tags caches
        keysToInvalidate.Add("all-recipes");
        keysToInvalidate.Add("all-tags");
        
        // Add tag-specific caches
        if (recipe.Tags != null)
        {
            foreach (var tag in recipe.Tags)
            {
                keysToInvalidate.Add($"recipes-tag-{tag.Name.ToLower()}");
            }
        }
        
        // Add old recipe's tag-specific caches
        if (oldRecipe?.Tags != null)
        {
            foreach (var tag in oldRecipe.Tags)
            {
                keysToInvalidate.Add($"recipes-tag-{tag.Name.ToLower()}");
            }
        }
        
        // Invalidate any search-related caches
        // This is a simplified approach - for a production app you might want 
        // to be more selective about which search caches to invalidate
        var searchCacheKeys = await _cache.GetAsync("search-cache-keys");
        if (searchCacheKeys != null)
        {
            var searchKeys = JsonSerializer.Deserialize<List<string>>(searchCacheKeys);
            if (searchKeys != null)
            {
                foreach (var key in searchKeys)
                {
                    keysToInvalidate.Add(key);
                }
            }
        }
        
        // Invalidate all identified cache keys
        foreach (var key in keysToInvalidate)
        {
            await _cache.RemoveAsync(key);
        }
    }
}