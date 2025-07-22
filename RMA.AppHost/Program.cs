using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// Add Redis cache
var cache = builder.AddRedis("cache");

// Add PostgreSQL database
var postgres = builder.AddPostgres("postgres");

// Only add pgAdmin in development environment
if (builder.Environment.IsDevelopment())
{
    postgres.WithPgAdmin()
        .AddDatabase("recipes");

    Console.WriteLine("pgAdmin added for development environment at port 5050");
}
else
{
    postgres.AddDatabase("recipes");
    Console.WriteLine("pgAdmin not added in production environment");
}

// Add API service with dependencies
var apiService = builder.AddProject<Projects.RMA_ApiService>("apiservice")
    .WithReference(cache)
    .WithReference(postgres)
    .WaitFor(postgres)
    .WaitFor(cache);


builder.Build().Run();