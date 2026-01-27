namespace kudos.backend.infrastructure.nhibernate.filtering;

/// <summary>
/// Filter operator class
/// </summary>
public class FilterOperator
{
    public FilterOperator() { }

    public FilterOperator(string fileldName, IEnumerable<string> values, string relationalOperatorType)
    {
        FieldName = fileldName;
        RelationalOperatorType = relationalOperatorType;
        Values = values.ToList();
    }

    public string FieldName { get; set; } = string.Empty;
    public string RelationalOperatorType { get; set; } = string.Empty;
    public IList<string> Values { get; set; } = new List<string>();
}
