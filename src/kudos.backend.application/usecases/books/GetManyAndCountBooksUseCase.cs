using FastEndpoints;
using kudos.backend.domain.daos;
using kudos.backend.domain.interfaces.repositories;
using Microsoft.Extensions.Logging;

namespace kudos.backend.application.usecases.books;

/// <summary>
/// Use case for retrieving a paginated list of books.
/// </summary>
public abstract class GetManyAndCountBooksUseCase
{
    public class Command : ICommand<GetManyAndCountResult<BookDao>>
    {
        public string? Query { get; set; }
    }

    public class Handler(IUnitOfWork uoW, ILogger<Handler> logger)
        : ICommandHandler<Command, GetManyAndCountResult<BookDao>>
    {
        private readonly IUnitOfWork _uoW = uoW;
        private readonly ILogger<Handler> _logger = logger;

        public async Task<GetManyAndCountResult<BookDao>> ExecuteAsync(
            Command command,
            CancellationToken ct)
        {
            try
            {
                _uoW.BeginTransaction();
                _logger.LogInformation(
                    "Executing GetManyAndCountBooksUseCase with query: {Query}",
                    command.Query);

                // Uses BookDaos (read-only repository)
                var books = await _uoW.BookDaos.GetManyAndCountAsync(
                    command.Query,
                    nameof(BookDao.Title),  // Default sorting
                    ct);

                _logger.LogInformation(
                    "End GetManyAndCountBooksUseCase with total: {Total}",
                    books.Count);

                _uoW.Commit();
                return books;
            }
            catch
            {
                _uoW.Rollback();
                throw;
            }
        }
    }
}
