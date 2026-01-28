using kudos.backend.webapi.dtos;

namespace kudos.backend.webapi.features.books.models;

/// <summary>
/// Data model for retrieving many books with count.
/// </summary>
public class GetManyAndCountBooksModel
{
    /// <summary>
    /// Represents the request data used to get many books with count.
    /// Empty - query params are obtained from HttpContext.Request.QueryString.Value
    /// </summary>
    public class Request
    {
        // Empty - query params are obtained from the QueryString
    }

    /// <summary>
    /// Represents a paginated list of books along with the total count.
    /// </summary>
    public class Response : GetManyAndCountResultDto<BookDto>
    {
        // Inherits from GetManyAndCountResultDto:
        // - IEnumerable<BookDto> Items
        // - long Count
        // - int PageNumber
        // - int PageSize
        // - string SortBy
        // - string SortCriteria
    }
}
