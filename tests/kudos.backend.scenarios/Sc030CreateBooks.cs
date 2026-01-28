using kudos.backend.domain.entities;
using kudos.backend.domain.interfaces.repositories;

namespace kudos.backend.scenarios;

/// <summary>
/// Scenario to create books with authors for testing.
/// Used in: GetManyAndCountBooks tests, search and pagination tests.
/// </summary>
public class Sc030CreateBooks(IUnitOfWork uoW) : IScenario
{
    private readonly IUnitOfWork _uoW = uoW;

    /// <summary>
    /// Get the scenario file name used to store in the file system.
    /// </summary>
    public string ScenarioFileName => "CreateBooks";

    /// <summary>
    /// Requires Sc020CreateAuthors to be pre-loaded.
    /// </summary>
    public Type? PreloadScenario => typeof(Sc020CreateAuthors);

    /// <summary>
    /// Seed data - Create books with authors and images.
    /// </summary>
    public async Task SeedData()
    {
        try
        {
            _uoW.BeginTransaction();

            // Get authors from the previous scenario
            var authors = (await _uoW.Authors.GetAsync()).ToList();
            var garciaMarquez = authors.First(a => a.Name.Contains("García Márquez"));
            var borges = authors.First(a => a.Name.Contains("Borges"));
            var allende = authors.First(a => a.Name.Contains("Allende"));

            // Create books
            var books = new List<Book>
            {
                new("Cien años de soledad", "978-0060883287", garciaMarquez.Id)
                    { Category = "Novela", CoverImageUrl = "https://example.com/cien-anos.jpg" },
                new("El amor en los tiempos del cólera", "978-0307389732", garciaMarquez.Id)
                    { Category = "Novela" },
                new("El Aleph", "978-0142437889", borges.Id)
                    { Category = "Cuento" },
                new("Ficciones", "978-0802130303", borges.Id)
                    { Category = "Cuento" },
                new("La casa de los espíritus", "978-1501117015", allende.Id)
                    { Category = "Novela" },
                new("Eva Luna", "978-0553383829", allende.Id)
                    { Category = "Novela" }
            };

            foreach (var book in books)
                await _uoW.Books.AddAsync(book);

            // Add images to some books
            var cienAnos = books[0];
            await _uoW.BookImages.AddAsync(new BookImage(cienAnos.Id, "https://example.com/cien-anos-back.jpg"));
            await _uoW.BookImages.AddAsync(new BookImage(cienAnos.Id, "https://example.com/cien-anos-spine.jpg"));

            _uoW.Commit();
        }
        catch
        {
            _uoW.Rollback();
            throw;
        }
    }
}
