namespace kudos.backend.infrastructure.nhibernate.filtering;

/// <summary>
/// Sorting criteria
/// </summary>
public class Sorting
{
    public Sorting() { }

    public Sorting(string by, string direction)
    {
        By = by;
        Direction = direction;
    }

    public string By
    {
        get => string.IsNullOrEmpty(_by) ? _by : _by.ToPascalCase();
        set => _by = value;
    }
    private string _by = string.Empty;

    public string Direction { get; set; } = string.Empty;

    public bool IsValid() => !string.IsNullOrEmpty(By) && !string.IsNullOrEmpty(Direction);
}
