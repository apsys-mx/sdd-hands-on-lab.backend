using FastEndpoints;
using kudos.backend.application.usecases.books;
using kudos.backend.webapi.dtos;
using kudos.backend.webapi.features.books.models;
using Microsoft.AspNetCore.Http;

namespace kudos.backend.webapi.features.books.endpoint;

/// <summary>
/// Endpoint to get a paginated list of books.
/// </summary>
public class GetManyAndCountBooksEndpoint(AutoMapper.IMapper mapper)
    : EndpointWithoutRequest<GetManyAndCountResultDto<BookDto>>
{
    private readonly AutoMapper.IMapper _mapper = mapper;

    public override void Configure()
    {
        Get("/api/books");
        AllowAnonymous();
        Description(d => d
            .WithTags("Books")
            .WithName("GetBooks")
            .WithDescription(@"Get a paginated list of books with filtering and sorting.

Query Parameters:
- search: Search in title, ISBN, author name
- pageNumber: Page number (default: 1)
- pageSize: Items per page (default: 10, max: 100)
- sortBy: Field to sort by (default: Title)
- sortCriteria: Sort direction 'asc' or 'desc' (default: asc)")
            .Produces<GetManyAndCountBooksModel.Response>(200, "application/json")
            .ProducesProblemDetails(StatusCodes.Status400BadRequest)
            .ProducesProblemDetails(StatusCodes.Status401Unauthorized)
            .ProducesProblemDetails(StatusCodes.Status500InternalServerError));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        try
        {
            // APSYS Pattern: Complete query string is passed to Use Case
            var command = new GetManyAndCountBooksUseCase.Command
            {
                Query = HttpContext.Request.QueryString.Value
            };

            // FastEndpoints resolves the handler automatically
            var result = await command.ExecuteAsync(ct);
            var response = _mapper.Map<GetManyAndCountResultDto<BookDto>>(result);

            Logger.LogInformation("Successfully retrieved {Count} books", result.Count);
            await Send.OkAsync(response, cancellation: ct);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to retrieve books");
            AddError($"An unexpected error occurred: {ex.Message}");
            await Send.ErrorsAsync(statusCode: StatusCodes.Status500InternalServerError, cancellation: ct);
        }
    }
}
