namespace kudos.backend.domain.daos;

/// <summary>
/// Data Access Object for Book entities, used for read-only database operations.
/// Maps to books_view which includes author information.
/// </summary>
public class BookDao
{
    public virtual Guid Id { get; set; }
    public virtual string Title { get; set; } = string.Empty;
    public virtual string ISBN { get; set; } = string.Empty;
    public virtual string? Category { get; set; }
    public virtual string? CoverImageUrl { get; set; }
    public virtual Guid AuthorId { get; set; }
    public virtual string AuthorName { get; set; } = string.Empty;
    public virtual DateTime CreationDate { get; set; }
    public virtual string SearchAll { get; set; } = string.Empty;
}
