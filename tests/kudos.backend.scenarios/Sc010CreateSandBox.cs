namespace kudos.backend.scenarios;

/// <summary>
/// Empty scenario - clears the database.
/// This is the base scenario that all other scenarios can depend on.
/// </summary>
public class Sc010CreateSandBox() : IScenario
{

    /// <summary>
    /// Get the scenario file name used to store in the file system
    /// </summary>
    public string ScenarioFileName => "CreateSandBox";

    /// <summary>
    /// No pre-load scenario for this scenario
    /// </summary>
    public Type? PreloadScenario => null;

    /// <summary>
    /// Seed data - Clean all tables before starting scenarios
    /// </summary>
    public async Task SeedData()
    {
        // Nothing to do here for sandbox
    }
}
