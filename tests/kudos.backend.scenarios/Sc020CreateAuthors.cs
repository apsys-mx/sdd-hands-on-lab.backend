using kudos.backend.domain.entities;
using kudos.backend.domain.interfaces.repositories;

namespace kudos.backend.scenarios;

/// <summary>
/// Scenario to create authors for testing.
/// Used in: Book-related tests that require authors.
/// </summary>
public class Sc020CreateAuthors(IUnitOfWork uoW) : IScenario
{
    private readonly IUnitOfWork _uoW = uoW;

    /// <summary>
    /// Get the scenario file name used to store in the file system.
    /// </summary>
    public string ScenarioFileName => "CreateAuthors";

    /// <summary>
    /// No pre-load scenario for this scenario.
    /// </summary>
    public Type? PreloadScenario => null;

    /// <summary>
    /// Seed data - Create authors for testing.
    /// </summary>
    public async Task SeedData()
    {
        var authors = new List<(string Name, string? Country, DateTime? BirthDate)>
        {
            ("Gabriel García Márquez", "Colombia", new DateTime(1927, 3, 6)),
            ("Jorge Luis Borges", "Argentina", new DateTime(1899, 8, 24)),
            ("Isabel Allende", "Chile", new DateTime(1942, 8, 2))
        };

        try
        {
            _uoW.BeginTransaction();
            foreach (var (name, country, birthDate) in authors)
            {
                var author = new Author(name) { Country = country, BirthDate = birthDate };
                await _uoW.Authors.AddAsync(author);
            }
            _uoW.Commit();
        }
        catch
        {
            _uoW.Rollback();
            throw;
        }
    }
}
