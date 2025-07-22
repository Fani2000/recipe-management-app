using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SDK.Models;

namespace SDK.Data
{
    public static class RecipeDbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider,
            ILogger<RecipesDbContext.RecipesDbContext> logger)
        {
            await using var scope = serviceProvider.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<RecipesDbContext.RecipesDbContext>();

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

        private static async Task SeedData(RecipesDbContext.RecipesDbContext context)
        {
            // Seed Tags
            var vegetarianTag = new Tag { Id = 1, Name = "Vegetarian" };
            var quickTag = new Tag { Id = 2, Name = "Quick" };
            var italianTag = new Tag { Id = 3, Name = "Italian" };
            var asianTag = new Tag { Id = 4, Name = "Asian" };
            var dessertTag = new Tag { Id = 5, Name = "Dessert" };
            var breakfastTag = new Tag { Id = 6, Name = "Breakfast" };
            var dinnerTag = new Tag { Id = 7, Name = "Dinner" };
            var healthyTag = new Tag { Id = 8, Name = "Healthy" };

            await context.Tags.AddRangeAsync(
                vegetarianTag, quickTag, italianTag, asianTag,
                dessertTag, breakfastTag, dinnerTag, healthyTag
            );

            // Seed Recipes
            var carbonara = new Recipe
            {
                Id = 1,
                Title = "Spaghetti Carbonara",
                CookingTimeMinutes = 30
            };

            var stirFry = new Recipe
            {
                Id = 2,
                Title = "Vegetable Stir Fry",
                CookingTimeMinutes = 20
            };

            var cookies = new Recipe
            {
                Id = 3,
                Title = "Chocolate Chip Cookies",
                CookingTimeMinutes = 45
            };

            var avocadoToast = new Recipe
            {
                Id = 4,
                Title = "Avocado Toast",
                CookingTimeMinutes = 15
            };

            // Add tags to recipes
            carbonara.Tags.Add(italianTag);
            carbonara.Tags.Add(dinnerTag);

            stirFry.Tags.Add(vegetarianTag);
            stirFry.Tags.Add(quickTag);
            stirFry.Tags.Add(asianTag);
            stirFry.Tags.Add(healthyTag);

            cookies.Tags.Add(dessertTag);

            avocadoToast.Tags.Add(vegetarianTag);
            avocadoToast.Tags.Add(quickTag);
            avocadoToast.Tags.Add(breakfastTag);
            avocadoToast.Tags.Add(healthyTag);

            await context.Recipes.AddRangeAsync(carbonara, stirFry, cookies, avocadoToast);

            // Seed Ingredients for Spaghetti Carbonara
            await context.Ingredients.AddRangeAsync(
                new Ingredient { Id = 1, Content = "200g spaghetti", Index = 0, Recipe = carbonara },
                new Ingredient { Id = 2, Content = "100g pancetta or guanciale", Index = 1, Recipe = carbonara },
                new Ingredient { Id = 3, Content = "2 large eggs", Index = 2, Recipe = carbonara },
                new Ingredient
                    { Id = 4, Content = "50g Pecorino Romano cheese, grated", Index = 3, Recipe = carbonara },
                new Ingredient { Id = 5, Content = "50g Parmesan cheese, grated", Index = 4, Recipe = carbonara },
                new Ingredient { Id = 6, Content = "Black pepper, freshly ground", Index = 5, Recipe = carbonara },
                new Ingredient { Id = 7, Content = "Salt to taste", Index = 6, Recipe = carbonara }
            );

            // Seed Ingredients for Vegetable Stir Fry
            await context.Ingredients.AddRangeAsync(
                new Ingredient { Id = 8, Content = "2 tbsp vegetable oil", Index = 0, Recipe = stirFry },
                new Ingredient { Id = 9, Content = "1 bell pepper, sliced", Index = 1, Recipe = stirFry },
                new Ingredient { Id = 10, Content = "1 carrot, julienned", Index = 2, Recipe = stirFry },
                new Ingredient { Id = 11, Content = "1 cup broccoli florets", Index = 3, Recipe = stirFry },
                new Ingredient { Id = 12, Content = "1 cup snap peas", Index = 4, Recipe = stirFry },
                new Ingredient { Id = 13, Content = "2 cloves garlic, minced", Index = 5, Recipe = stirFry },
                new Ingredient { Id = 14, Content = "1 tbsp ginger, grated", Index = 6, Recipe = stirFry },
                new Ingredient { Id = 15, Content = "3 tbsp soy sauce", Index = 7, Recipe = stirFry },
                new Ingredient { Id = 16, Content = "1 tbsp sesame oil", Index = 8, Recipe = stirFry }
            );

            // Seed Ingredients for Chocolate Chip Cookies
            await context.Ingredients.AddRangeAsync(
                new Ingredient { Id = 17, Content = "2 1/4 cups all-purpose flour", Index = 0, Recipe = cookies },
                new Ingredient { Id = 18, Content = "1 tsp baking soda", Index = 1, Recipe = cookies },
                new Ingredient { Id = 19, Content = "1 tsp salt", Index = 2, Recipe = cookies },
                new Ingredient { Id = 20, Content = "1 cup unsalted butter, softened", Index = 3, Recipe = cookies },
                new Ingredient { Id = 21, Content = "3/4 cup granulated sugar", Index = 4, Recipe = cookies },
                new Ingredient { Id = 22, Content = "3/4 cup packed brown sugar", Index = 5, Recipe = cookies },
                new Ingredient { Id = 23, Content = "2 large eggs", Index = 6, Recipe = cookies },
                new Ingredient { Id = 24, Content = "2 tsp vanilla extract", Index = 7, Recipe = cookies },
                new Ingredient { Id = 25, Content = "2 cups chocolate chips", Index = 8, Recipe = cookies }
            );

            // Seed Ingredients for Avocado Toast
            await context.Ingredients.AddRangeAsync(
                new Ingredient { Id = 26, Content = "2 slices of whole grain bread", Index = 0, Recipe = avocadoToast },
                new Ingredient { Id = 27, Content = "1 ripe avocado", Index = 1, Recipe = avocadoToast },
                new Ingredient { Id = 28, Content = "1/2 lemon, juiced", Index = 2, Recipe = avocadoToast },
                new Ingredient { Id = 29, Content = "Salt and pepper to taste", Index = 3, Recipe = avocadoToast },
                new Ingredient { Id = 30, Content = "Red pepper flakes (optional)", Index = 4, Recipe = avocadoToast },
                new Ingredient { Id = 31, Content = "2 eggs (optional)", Index = 5, Recipe = avocadoToast }
            );

            // Seed Steps for Spaghetti Carbonara
            await context.Steps.AddRangeAsync(
                new Step
                {
                    Id = 1,
                    Content =
                        "Bring a large pot of salted water to boil and cook spaghetti according to package instructions until al dente.",
                    Index = 0, Recipe = carbonara
                },
                new Step
                {
                    Id = 2,
                    Content =
                        "While pasta is cooking, heat a large skillet over medium heat and cook pancetta until crispy, about 5-7 minutes.",
                    Index = 1, Recipe = carbonara
                },
                new Step
                {
                    Id = 3, Content = "In a bowl, whisk together eggs, grated cheeses, and plenty of black pepper.",
                    Index = 2, Recipe = carbonara
                },
                new Step
                {
                    Id = 4, Content = "When pasta is done, reserve 1/2 cup of pasta water, then drain.", Index = 3,
                    Recipe = carbonara
                },
                new Step
                {
                    Id = 5,
                    Content = "Working quickly, add hot pasta to the skillet with pancetta, toss to coat in the fat.",
                    Index = 4, Recipe = carbonara
                },
                new Step
                {
                    Id = 6,
                    Content =
                        "Remove skillet from heat and quickly pour in egg mixture, stirring constantly to coat the pasta and create a creamy sauce. Add a splash of pasta water if needed to loosen.",
                    Index = 5, Recipe = carbonara
                },
                new Step
                {
                    Id = 7, Content = "Serve immediately with extra grated cheese and black pepper on top.", Index = 6,
                    Recipe = carbonara
                }
            );

            // Seed Steps for Vegetable Stir Fry
            await context.Steps.AddRangeAsync(
                new Step
                {
                    Id = 8, Content = "Heat vegetable oil in a large wok or skillet over high heat.", Index = 0,
                    Recipe = stirFry
                },
                new Step
                {
                    Id = 9, Content = "Add garlic and ginger, stir fry for 30 seconds until fragrant.", Index = 1,
                    Recipe = stirFry
                },
                new Step
                {
                    Id = 10, Content = "Add carrots and broccoli, stir fry for 2 minutes.", Index = 2, Recipe = stirFry
                },
                new Step
                {
                    Id = 11,
                    Content =
                        "Add bell pepper and snap peas, continue stir frying for another 2-3 minutes until vegetables are crisp-tender.",
                    Index = 3, Recipe = stirFry
                },
                new Step
                {
                    Id = 12, Content = "Pour in soy sauce and toss to coat all vegetables.", Index = 4, Recipe = stirFry
                },
                new Step
                {
                    Id = 13, Content = "Remove from heat, drizzle with sesame oil, and serve immediately.", Index = 5,
                    Recipe = stirFry
                }
            );

            // Seed Steps for Chocolate Chip Cookies
            await context.Steps.AddRangeAsync(
                new Step
                {
                    Id = 14, Content = "Preheat oven to 375°F (190°C). Line baking sheets with parchment paper.",
                    Index = 0, Recipe = cookies
                },
                new Step
                {
                    Id = 15, Content = "In a small bowl, whisk together flour, baking soda, and salt.", Index = 1,
                    Recipe = cookies
                },
                new Step
                {
                    Id = 16, Content = "In a large bowl, beat butter and both sugars until creamy and pale.", Index = 2,
                    Recipe = cookies
                },
                new Step
                {
                    Id = 17,
                    Content = "Add eggs one at a time, mixing well after each addition. Stir in vanilla extract.",
                    Index = 3, Recipe = cookies
                },
                new Step
                {
                    Id = 18, Content = "Gradually add flour mixture to the butter mixture, mixing just until combined.",
                    Index = 4, Recipe = cookies
                },
                new Step { Id = 19, Content = "Fold in chocolate chips.", Index = 5, Recipe = cookies },
                new Step
                {
                    Id = 20,
                    Content =
                        "Drop tablespoon-sized balls of dough onto prepared baking sheets, spacing them about 2 inches apart.",
                    Index = 6, Recipe = cookies
                },
                new Step
                {
                    Id = 21, Content = "Bake for 9-11 minutes until edges are golden but centers are still soft.",
                    Index = 7, Recipe = cookies
                },
                new Step
                {
                    Id = 22,
                    Content =
                        "Let cool on baking sheets for 5 minutes, then transfer to wire racks to cool completely.",
                    Index = 8, Recipe = cookies
                }
            );

            // Seed Steps for Avocado Toast
            await context.Steps.AddRangeAsync(
                new Step
                {
                    Id = 23, Content = "Toast the bread slices until golden brown and crispy.", Index = 0,
                    Recipe = avocadoToast
                },
                new Step
                {
                    Id = 24, Content = "Cut the avocado in half, remove the pit, and scoop the flesh into a bowl.",
                    Index = 1, Recipe = avocadoToast
                },
                new Step
                {
                    Id = 25,
                    Content =
                        "Add lemon juice, salt, and pepper to the avocado and mash with a fork to desired consistency.",
                    Index = 2, Recipe = avocadoToast
                },
                new Step
                {
                    Id = 26, Content = "Spread the mashed avocado evenly on the toast slices.", Index = 3,
                    Recipe = avocadoToast
                },
                new Step
                {
                    Id = 27, Content = "Sprinkle with red pepper flakes if desired.", Index = 4, Recipe = avocadoToast
                },
                new Step
                {
                    Id = 28, Content = "For an optional protein boost, top with fried or poached eggs.", Index = 5,
                    Recipe = avocadoToast
                }
            );

            await context.SaveChangesAsync();
        }
    }
}