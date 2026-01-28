using FluentValidation;
using kudos.backend.domain.validators;

namespace kudos.backend.domain.entities;

/// <summary>
/// Represents a book in the catalog.
/// </summary>
public class Book : AbstractDomainObject
{
    /// <summary>
    /// Parameterless constructor for NHibernate.
    /// </summary>
    public Book() : base() { }

    /// <summary>
    /// Creates a new Book with the required properties.
    /// </summary>
    /// <param name="title">The book's title.</param>
    /// <param name="isbn">The book's ISBN code.</param>
    /// <param name="authorId">The ID of the book's author.</param>
    public Book(string title, string isbn, Guid authorId) : base()
    {
        Title = title;
        ISBN = isbn;
        AuthorId = authorId;
    }

    /// <summary>
    /// Gets or sets the book's title.
    /// </summary>
    public virtual string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the book's ISBN code.
    /// </summary>
    public virtual string ISBN { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ID of the book's author.
    /// </summary>
    public virtual Guid AuthorId { get; set; }

    /// <summary>
    /// Gets or sets the book's category/genre.
    /// </summary>
    public virtual string? Category { get; set; }

    /// <summary>
    /// Gets or sets the URL of the book's cover image.
    /// </summary>
    public virtual string? CoverImageUrl { get; set; }

    /// <summary>
    /// Gets the validator for the Book entity.
    /// </summary>
    public override IValidator? GetValidator()
        => new BookValidator();
}
