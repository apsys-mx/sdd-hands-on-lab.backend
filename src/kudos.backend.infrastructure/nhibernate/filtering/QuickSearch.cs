namespace kudos.backend.infrastructure.nhibernate.filtering;

/// <summary>
/// Quick search filter
/// </summary>
public class QuickSearch
{
    public string Value { get; set; } = string.Empty;
    public IList<string> FieldNames { get; set; } = new List<string>();
}
