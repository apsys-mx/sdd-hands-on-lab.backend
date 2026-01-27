using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;

namespace kudos.backend.infrastructure.nhibernate.filtering;

/// <summary>
/// Query string parser
/// </summary>
public class QueryStringParser
{
    public const int DEFAULT_PAGE_NUMBER = 1;
    public const int DEFAULT_PAGE_SIZE = 25;

    private const string _pageNumber = "pageNumber";
    private const string _pageSize = "pageSize";
    private const string _sortBy = "sortBy";
    private const string _sortDirection = "sortDirection";
    private const string _query = "query";
    private const string _query_ColumnsToSearch = "query_ColumnsToSearch";
    private readonly string _queryString;
    private readonly string _descending = "desc";
    private readonly string _ascending = "asc";
    private readonly string[] _excludedKeys = new string[] { _pageNumber, _pageSize, _sortBy, _sortDirection, _query };

    public QueryStringParser(string queryString)
    {
        _queryString = WebUtility.UrlDecode(queryString);
    }

    public static string GetDescendingValue() => "desc";
    public static string GetAscendingValue() => "asc";

    public int ParsePageNumber()
    {
        int pageNumber = DEFAULT_PAGE_NUMBER;
        if (string.IsNullOrEmpty(_queryString))
            return pageNumber;

        QueryStringArgs parameters = new(_queryString);
        if (parameters.ContainsKey(_pageNumber) && !int.TryParse(parameters[_pageNumber], out pageNumber))
            throw new InvalidQueryStringArgumentException(_pageNumber);

        if (pageNumber < 0)
            throw new InvalidQueryStringArgumentException(_pageNumber);
        return pageNumber;
    }

    public int ParsePageSize()
    {
        int pageSize = DEFAULT_PAGE_SIZE;
        if (string.IsNullOrEmpty(_queryString))
            return pageSize;
        QueryStringArgs parameters = new(_queryString);

        if (parameters.ContainsKey(_pageSize) && !int.TryParse(parameters[_pageSize], out pageSize))
            throw new InvalidQueryStringArgumentException(_pageSize);

        if (pageSize <= 0)
            throw new InvalidQueryStringArgumentException(_pageSize);
        return pageSize;
    }

    public Sorting ParseSorting<T>(string defaultFieldName)
    {
        string? sortByField = defaultFieldName;
        string? sortDirection = _ascending;

        QueryStringArgs parameters = new(_queryString);
        if (parameters.TryGetValue(_sortBy, out var sortByValue))
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            if (!properties.Any(p => p.Name.Equals(sortByValue, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidQueryStringArgumentException(_sortBy);
            sortByField = sortByValue;
        }
        if (parameters.TryGetValue(_sortDirection, out var sortDirectionValue))
        {
            if (sortDirectionValue != _descending && sortDirectionValue != _ascending)
                throw new InvalidQueryStringArgumentException(_sortDirection);
            sortDirection = sortDirectionValue;
        }

        if (sortByField == null || sortDirection == null)
            throw new InvalidQueryStringArgumentException("Sorting parameters cannot be null");

        return new Sorting(sortByField, sortDirection);
    }

    public IList<FilterOperator> ParseFilterOperators<T>()
    {
        IList<FilterOperator> filterOperatorsResult = new List<FilterOperator>();
        QueryStringArgs parameters = new(_queryString);
        IEnumerable<KeyValuePair<string, string>> allFilters = parameters.Where(parameter => !_excludedKeys.Contains(parameter.Key));
        foreach (var filter in allFilters)
        {
            string[] filterData = filter.Value.Split("||");
            string[] filterValues = filterData[0].Split("|");
            var fileterOperator = filterData[1];
            var operatorFieldName = filter.Key.ToPascalCase();
            filterOperatorsResult.Add(new FilterOperator(operatorFieldName, filterValues, fileterOperator));
        }
        return filterOperatorsResult;
    }

    public QuickSearch? ParseQuery<T>()
    {
        string? query = string.Empty;
        IList<string> fields = new List<string>();
        QuickSearch quickSearch = new();

        if (string.IsNullOrEmpty(_queryString))
            return null;

        QueryStringArgs parameters = new(_queryString);

        if (!parameters.ContainsKey(_query))
            return null;

        if (!parameters.Any() && string.IsNullOrWhiteSpace(parameters[_query]))
            return null;

        query = parameters[_query].Split("||").FirstOrDefault();

        if (string.IsNullOrEmpty(query))
            throw new InvalidQueryStringArgumentException(_query);

        PropertyInfo[] properties = typeof(T).GetProperties();

        if (parameters[_query].Split("||").Count() <= 1)
        {
            ICollection<string> stringFields = new List<string>();
            quickSearch.Value = parameters[_query];
            quickSearch.Value = quickSearch.Value.ToLowerInvariant();

            foreach (PropertyInfo property in properties)
                if ((property.PropertyType == typeof(string) || property.PropertyType == typeof(int)) && property.Name != "Id")
                    stringFields.Add(property.Name);

            quickSearch.FieldNames = stringFields.ToList();
            return quickSearch;
        }

        quickSearch.Value = query;

        if (string.IsNullOrWhiteSpace(parameters[_query].Split("||")[1]))
            throw new InvalidQueryStringArgumentException(_query_ColumnsToSearch);

        fields = parameters[_query].Split("||")[1].Split("|");

        foreach (string field in fields)
            if (!properties.Any(p => p.Name.Equals(field, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidQueryStringArgumentException(_query_ColumnsToSearch);

        quickSearch.FieldNames = fields;
        return quickSearch;
    }
}

internal class QueryStringArgs : Dictionary<string, string>
{
    private const string Pattern = @"(?<argName>\w+)=(?<argValue>.+)";
    private readonly Regex _regex = new(Pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));

    public bool ContainsValidArguments()
    {
        return (this.ContainsKey("cnn"));
    }

    public QueryStringArgs(string query)
    {
        var args = query.Split('&');
        foreach (var match in args.Select(arg => _regex.Match(arg)).Where(m => m.Success))
        {
            try
            {
                this.Add(match.Groups["argName"].Value, match.Groups["argValue"].Value);
            }
            catch
            {
                // Continues execution
            }
        }
    }
}
