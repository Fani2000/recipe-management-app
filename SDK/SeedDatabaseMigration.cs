using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SDK.Models;
using SDK.RecipesContext;

namespace SDK.Data
{
    public static class RecipeDbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider,
            ILogger<RecipesDbContext> logger)
        {
            await using var scope = serviceProvider.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<RecipesDbContext>();

            try
            {
                // Ensure database is created
                await context.Database.MigrateAsync();

                // Only seed if the database is empty (no recipes)
                if (!context.Recipes.Any())
                {
                    await SeedData(context);
                    logger.LogInformation("Seed data was successfully applied");
                }
                else
                {
                    logger.LogInformation("Skipped seed data as the database already contains recipes");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database");
                throw;
            }
        }

        public async static Task SeedData(RecipesDbContext context)
        {
            // Only seed if no recipes exist
            if (await context.Recipes.AnyAsync())
                return;

            // Create tags
            var tags = new List<Tag>
            {
                new Tag { Name = "Breakfast" },
                new Tag { Name = "Lunch" },
                new Tag { Name = "Dinner" },
                new Tag { Name = "Vegetarian" },
                new Tag { Name = "Vegan" },
                new Tag { Name = "Gluten-Free" },
                new Tag { Name = "Quick Meals" },
                new Tag { Name = "Dessert" },
                new Tag { Name = "Italian" },
                new Tag { Name = "Mexican" },
                new Tag { Name = "Asian" }
            };

            await context.Tags.AddRangeAsync(tags);
            await context.SaveChangesAsync();

            // Create recipes
            var recipes = new List<Recipe>
            {
                new Recipe
                {
                    Title = "Scrambled Eggs",
                    Description =
                        "A simple and delicious breakfast classic made with fluffy eggs. Ready in minutes and perfect for busy mornings.",
                    CookingTimeMinutes = 10,
                    Ingredients = new List<Ingredient>
                    {
                        new Ingredient { Content = "4 eggs", Index = 0 },
                        new Ingredient { Content = "2 tbsp milk", Index = 1 },
                        new Ingredient { Content = "Salt and pepper to taste", Index = 2 },
                        new Ingredient { Content = "1 tbsp butter", Index = 3 }
                    },
                    Steps = new List<Step>
                    {
                        new Step { Content = "Whisk eggs and milk together in a bowl", Index = 0 },
                        new Step { Content = "Season with salt and pepper", Index = 1 },
                        new Step { Content = "Melt butter in a non-stick pan over medium heat", Index = 2 },
                        new Step
                        {
                            Content = "Pour in egg mixture and stir gently until cooked but still moist", Index = 3
                        }
                    },
                    Tags = new List<Tag> { tags[0], tags[6] } // Breakfast, Quick Meals
                },

                new Recipe
                {
                    Title = "Spaghetti Carbonara",
                    Description =
                        "A classic Italian pasta dish with a creamy sauce made from eggs, cheese, pancetta, and black pepper. Rich, savory, and comforting.",
                    CookingTimeMinutes = 25,
                    Ingredients = new List<Ingredient>
                    {
                        new Ingredient { Content = "400g spaghetti", Index = 0 },
                        new Ingredient { Content = "200g pancetta or guanciale, diced", Index = 1 },
                        new Ingredient { Content = "3 large eggs", Index = 2 },
                        new Ingredient { Content = "75g Pecorino Romano, grated", Index = 3 },
                        new Ingredient { Content = "50g Parmesan, grated", Index = 4 },
                        new Ingredient { Content = "Freshly ground black pepper", Index = 5 },
                        new Ingredient { Content = "Salt to taste", Index = 6 }
                    },
                    Steps = new List<Step>
                    {
                        new Step
                        {
                            Content = "Cook spaghetti in salted water according to package instructions", Index = 0
                        },
                        new Step { Content = "While pasta cooks, fry pancetta in a large pan until crispy", Index = 1 },
                        new Step { Content = "In a bowl, whisk eggs and grated cheeses together", Index = 2 },
                        new Step { Content = "Drain pasta, reserving a cup of cooking water", Index = 3 },
                        new Step { Content = "Add hot pasta to pancetta, remove from heat", Index = 4 },
                        new Step
                        {
                            Content = "Quickly stir in egg and cheese mixture, thinning with pasta water if needed",
                            Index = 5
                        },
                        new Step { Content = "Season with black pepper and serve immediately", Index = 6 }
                    },
                    Tags = new List<Tag> { tags[2], tags[8] } // Dinner, Italian
                },

                new Recipe
                {
                    Title = "Vegetable Stir-Fry",
                    Description =
                        "A colorful and healthy stir-fry loaded with fresh vegetables and a flavorful sauce. Customizable and perfect for using up produce in your fridge.",
                    CookingTimeMinutes = 20,
                    Ingredients = new List<Ingredient>
                    {
                        new Ingredient { Content = "1 bell pepper, sliced", Index = 0 },
                        new Ingredient { Content = "1 carrot, julienned", Index = 1 },
                        new Ingredient { Content = "1 broccoli head, cut into florets", Index = 2 },
                        new Ingredient { Content = "1 onion, sliced", Index = 3 },
                        new Ingredient { Content = "2 cloves garlic, minced", Index = 4 },
                        new Ingredient { Content = "2 tbsp soy sauce", Index = 5 },
                        new Ingredient { Content = "1 tbsp sesame oil", Index = 6 },
                        new Ingredient { Content = "1 tsp ginger, grated", Index = 7 }
                    },
                    Steps = new List<Step>
                    {
                        new Step { Content = "Heat sesame oil in a large wok or skillet", Index = 0 },
                        new Step { Content = "Add garlic and ginger, sauté for 30 seconds", Index = 1 },
                        new Step { Content = "Add onions and stir-fry for 1 minute", Index = 2 },
                        new Step { Content = "Add remaining vegetables and stir-fry for 5-7 minutes", Index = 3 },
                        new Step { Content = "Add soy sauce and toss to combine", Index = 4 },
                        new Step { Content = "Serve hot, optionally over rice", Index = 5 }
                    },
                    Tags = new List<Tag>
                        { tags[3], tags[4], tags[6], tags[10] } // Vegetarian, Vegan, Quick Meals, Asian
                },

                new Recipe
                {
                    Title = "Chocolate Chip Cookies",
                    Description =
                        "Classic homemade cookies with the perfect balance of chewy centers and crisp edges. Packed with chocolate chips for a timeless dessert favorite.",
                    CookingTimeMinutes = 30,
                    Ingredients = new List<Ingredient>
                    {
                        new Ingredient { Content = "225g butter, softened", Index = 0 },
                        new Ingredient { Content = "150g granulated sugar", Index = 1 },
                        new Ingredient { Content = "275g brown sugar", Index = 2 },
                        new Ingredient { Content = "2 eggs", Index = 3 },
                        new Ingredient { Content = "2 tsp vanilla extract", Index = 4 },
                        new Ingredient { Content = "375g all-purpose flour", Index = 5 },
                        new Ingredient { Content = "1 tsp baking soda", Index = 6 },
                        new Ingredient { Content = "1 tsp salt", Index = 7 },
                        new Ingredient { Content = "350g chocolate chips", Index = 8 }
                    },
                    Steps = new List<Step>
                    {
                        new Step { Content = "Preheat oven to 190°C (375°F)", Index = 0 },
                        new Step { Content = "Cream together butter and sugars until light and fluffy", Index = 1 },
                        new Step { Content = "Beat in eggs one at a time, then stir in vanilla", Index = 2 },
                        new Step
                        {
                            Content = "Combine flour, baking soda, and salt; gradually add to creamed mixture",
                            Index = 3
                        },
                        new Step { Content = "Fold in chocolate chips", Index = 4 },
                        new Step
                        {
                            Content = "Drop rounded tablespoons of dough onto ungreased baking sheets", Index = 5
                        },
                        new Step { Content = "Bake for 9-11 minutes or until golden brown", Index = 6 },
                        new Step
                        {
                            Content = "Cool on baking sheet for 2 minutes, then transfer to wire racks", Index = 7
                        }
                    },
                    Tags = new List<Tag> { tags[7] } // Dessert
                },

                new Recipe
                {
                    Title = "Guacamole",
                    Description =
                        "Fresh and zesty homemade guacamole made with ripe avocados, lime, and cilantro. Perfect as a dip or topping for Mexican dishes.",
                    CookingTimeMinutes = 15,
                    Ingredients = new List<Ingredient>
                    {
                        new Ingredient { Content = "3 ripe avocados", Index = 0 },
                        new Ingredient { Content = "1 lime, juiced", Index = 1 },
                        new Ingredient { Content = "1 small onion, finely diced", Index = 2 },
                        new Ingredient { Content = "1 tomato, diced", Index = 3 },
                        new Ingredient { Content = "2 tbsp cilantro, chopped", Index = 4 },
                        new Ingredient { Content = "1 clove garlic, minced", Index = 5 },
                        new Ingredient { Content = "Salt and pepper to taste", Index = 6 },
                        new Ingredient { Content = "Optional: 1 jalapeño, seeded and minced", Index = 7 }
                    },
                    Steps = new List<Step>
                    {
                        new Step
                        {
                            Content = "Cut avocados in half, remove pits, and scoop flesh into a bowl", Index = 0
                        },
                        new Step { Content = "Mash avocados with a fork to desired consistency", Index = 1 },
                        new Step { Content = "Add lime juice and mix well", Index = 2 },
                        new Step { Content = "Stir in onion, tomato, cilantro, and garlic", Index = 3 },
                        new Step { Content = "Season with salt and pepper", Index = 4 },
                        new Step { Content = "Add jalapeño if desired for extra heat", Index = 5 },
                        new Step
                        {
                            Content = "Serve immediately or cover tightly with plastic wrap and refrigerate", Index = 6
                        }
                    },
                    Tags = new List<Tag>
                    {
                        tags[3], tags[4], tags[5], tags[6], tags[9]
                    } // Vegetarian, Vegan, Gluten-Free, Quick Meals, Mexican
                }
            };

            await context.Recipes.AddRangeAsync(recipes);
            await context.SaveChangesAsync();
        }
    }
}