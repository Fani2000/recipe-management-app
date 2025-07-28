using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// Add Redis cache
var cache = builder.AddRedis("cache");

// Add PostgreSQL database
var postgres = builder.AddPostgres("postgres");

// Only add pgAdmin in development environment
if (builder.Environment.IsDevelopment())
{
    postgres.WithPgAdmin();

    Console.WriteLine("pgAdmin added for development environment at port 5050");
}
else
{
    Console.WriteLine("pgAdmin not added in production environment");
}

// Add API service with dependencies
var apiService = builder
    .AddProject<Projects.RMA_ApiService>("apiservice")
    .WithReference(cache)
    .WithReference(postgres)
    .WithEndpoint()
    .WaitFor(postgres)
    .WaitFor(cache);

var frontend = builder.AddNpmApp("frontend", "../RMA.Frontend", "dev")
    .WithReference(apiService)
    .WithHttpEndpoint(5173, 5173, null, null, false)
    // .With("services__apiservice__http__0", "http://localhost:5450")
    .WithExternalHttpEndpoints()
    .WaitFor(apiService)
    .PublishAsDockerFile();

builder.Build().Run();
