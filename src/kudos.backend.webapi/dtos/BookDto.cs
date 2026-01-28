namespace kudos.backend.webapi.dtos;

/// <summary>
/// Data transfer object for Book.
/// </summary>
public class BookDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? CoverImageUrl { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public DateTime CreationDate { get; set; }
}
