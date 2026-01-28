using FastEndpoints;
using FastEndpoints.Swagger;
using kudos.backend.webapi.infrastructure;

// Load environment variables from .env file
// This is necessary to ensure that the connection string and other settings are available
DotNetEnv.Env.Load();

IConfiguration configuration;

var builder = WebApplication.CreateBuilder(args);
configuration = builder.Configuration;
var environment = builder.Environment;

// Configure dependency injection container
builder.Services
    .AddSwaggerGen()
    .AddEndpointsApiExplorer()
    .ConfigurePolicy()
    .ConfigureCors(configuration)
    .ConfigureIdentityServerClient(configuration)
    .ConfigureUnitOfWork(configuration)
    .ConfigureAutoMapper()
    .ConfigureValidators()
    .ConfigureDependencyInjections(environment)
    .AddLogging()
    .AddAuthorization()
    .AddFastEndpoints(o =>
    {
        // Scan application assembly for ICommandHandler implementations
        o.Assemblies = [
            typeof(kudos.backend.application.usecases.books.GetManyAndCountBooksUseCase).Assembly
        ];
    })
    .SwaggerDocument();

var app = builder.Build();

// Register all Commands and Handlers from the application assembly
app.Services.RegisterCommandsFromAssembly(typeof(kudos.backend.application.usecases.books.GetManyAndCountBooksUseCase).Assembly);

app.MapGet("/", () => Results.Redirect("/swagger"));

app.UseCors("CorsPolicy")
    .UseHttpsRedirection()
    .UseRouting()
    .UseAuthentication()
    .UseAuthorization()
    .UseFastEndpoints()
    .UseSwagger()
    .UseSwaggerUI(opt =>
{
    opt.DefaultModelsExpandDepth(-1); // Hide schemas by default
    opt.DisplayRequestDuration();
    opt.EnableTryItOutByDefault();
});

await app.RunAsync();

// Make Program accessible for integration tests
public partial class Program { }
