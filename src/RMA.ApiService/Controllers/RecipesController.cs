using Microsoft.AspNetCore.Mvc;
using SDK.DTOs;
using RMA.ApiService.Services;

namespace RMA.ApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        private readonly RecipeService _recipeService;

        public RecipesController(RecipeService recipeService)
        {
            _recipeService = recipeService;
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
            var recipes = await _recipeService.GetRecipesAsync(tag, search, maxCookingTime, sortBy, ascending);
            return Ok(recipes);
        }

        // GET: api/Recipes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RecipeDto>> GetRecipe(int id)
        {
            var recipe = await _recipeService.GetRecipeAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }
            return Ok(recipe);
        }

        // POST: api/Recipes
        [HttpPost]
        public async Task<ActionResult<RecipeDto>> PostRecipe(RecipeDto recipeDto)
        {
            var createdRecipe = await _recipeService.CreateRecipeAsync(recipeDto);
            return Ok(createdRecipe);
        }

        // PUT: api/Recipes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRecipe(int id, RecipeDto recipeDto)
        {
            var updated = await _recipeService.UpdateRecipeAsync(id, recipeDto);
            if (!updated)
            {
                return BadRequest();
            }
            return NoContent();
        }

        // DELETE: api/Recipes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            var deleted = await _recipeService.DeleteRecipeAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
