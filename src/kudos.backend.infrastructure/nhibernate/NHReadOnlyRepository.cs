using System.Linq.Expressions;
using kudos.backend.domain.interfaces.repositories;
using kudos.backend.infrastructure.nhibernate.filtering;
using System.Linq.Dynamic.Core;
using NHibernate;
using NHibernate.Linq;

namespace kudos.backend.infrastructure.nhibernate;

/// <summary>
/// Implementation of the read-only repository pattern using NHibernate ORM.
/// </summary>
public class NHReadOnlyRepository<T, TKey>(ISession session) : IReadOnlyRepository<T, TKey> where T : class, new()
{
    protected internal readonly ISession _session = session;

    public int Count() => this._session.QueryOver<T>().RowCount();

    public int Count(Expression<Func<T, bool>> query)
        => this._session.Query<T>().Where(query).Count();

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
        => this._session.Query<T>().CountAsync(cancellationToken);

    public Task<int> CountAsync(Expression<Func<T, bool>> query, CancellationToken cancellationToken = default)
        => this._session.Query<T>().Where(query).CountAsync(cancellationToken);

    public T Get(TKey id) => this._session.Get<T>(id);

    public IEnumerable<T> Get() => this._session.Query<T>();

    public IEnumerable<T> Get(Expression<Func<T, bool>> query)
        => this._session.Query<T>().Where(query);

    public IEnumerable<T> Get(Expression<Func<T, bool>> query, int page, int pageSize, SortingCriteria sortingCriteria)
        => this._session.Query<T>()
                .Where(query)
                .OrderBy(sortingCriteria.ToExpression())
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

    public Task<T> GetAsync(TKey id, CancellationToken cancellationToken = default)
        => this._session.GetAsync<T>(id, cancellationToken);

    public async Task<IEnumerable<T>> GetAsync(CancellationToken cancellationToken = default)
        => await this._session.Query<T>().ToListAsync(cancellationToken);

    public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> query, CancellationToken cancellationToken = default)
        => await this._session.Query<T>().Where(query).ToListAsync(cancellationToken);

    public GetManyAndCountResult<T> GetManyAndCount(string? query, string defaultSorting)
    {
        var (expression, pageNumber, pageSize, sortingCriteria) = PrepareQuery(query, defaultSorting);
        var items = this.Get(expression, pageNumber, pageSize, sortingCriteria);
        var total = this.Count(expression);
        return new GetManyAndCountResult<T>(items, total, pageNumber, pageSize, sortingCriteria);
    }

    public async Task<GetManyAndCountResult<T>> GetManyAndCountAsync(string? query, string defaultSorting, CancellationToken cancellationToken = default)
    {
        var (expression, pageNumber, pageSize, sortingCriteria) = PrepareQuery(query, defaultSorting);

        var total = await this._session.Query<T>()
            .Where(expression)
            .CountAsync(cancellationToken);

        var items = await this._session.Query<T>()
            .OrderBy(sortingCriteria.ToExpression())
            .Where(expression)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new GetManyAndCountResult<T>(items, total, pageNumber, pageSize, sortingCriteria);
    }

    private static (Expression<Func<T, bool>> expression, int pageNumber, int pageSize, SortingCriteria sortingCriteria) PrepareQuery(string? query, string defaultSorting)
    {
        var queryString = string.IsNullOrEmpty(query) ? string.Empty : query;
        QueryStringParser queryStringParser = new(queryString);

        int pageNumber = queryStringParser.ParsePageNumber();
        int pageSize = queryStringParser.ParsePageSize();

        Sorting sorting = queryStringParser.ParseSorting<T>(defaultSorting);
        SortingCriteriaType directions = sorting.Direction == QueryStringParser.GetDescendingValue()
            ? SortingCriteriaType.Descending
            : SortingCriteriaType.Ascending;
        SortingCriteria sortingCriteria = new(sorting.By, directions);

        IList<FilterOperator> filters = queryStringParser.ParseFilterOperators<T>();
        QuickSearch? quickSearch = queryStringParser.ParseQuery<T>();
        var expression = FilterExpressionParser.ParsePredicate<T>(filters);
        if (quickSearch != null)
            expression = FilterExpressionParser.ParseQueryValuesToExpression(expression, quickSearch);

        return (expression, pageNumber, pageSize, sortingCriteria);
    }
}
