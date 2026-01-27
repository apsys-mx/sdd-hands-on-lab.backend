using kudos.backend.domain.interfaces.repositories;

namespace kudos.backend.infrastructure.nhibernate;

/// <summary>
/// Sorting expression extender
/// </summary>
public static class SortingCriteriaExtender
{
    /// <summary>
    /// Converts a SortingCriteria object to a string expression for sorting.
    /// </summary>
    public static string ToExpression(this SortingCriteria sort)
    {
        string orderExpression = sort.Criteria == SortingCriteriaType.Ascending
            ? $"{sort.SortBy}"
            : $"{sort.SortBy} descending";
        return orderExpression;
    }
}
