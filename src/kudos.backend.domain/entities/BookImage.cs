using FluentValidation;
using kudos.backend.domain.validators;

namespace kudos.backend.domain.entities;

/// <summary>
/// Represents an additional image for a book (not the cover).
/// </summary>
public class BookImage : AbstractDomainObject
{
    /// <summary>
    /// Parameterless constructor for NHibernate.
    /// </summary>
    public BookImage() : base() { }

    /// <summary>
    /// Creates a new BookImage with the required properties.
    /// </summary>
    /// <param name="bookId">The ID of the book this image belongs to.</param>
    /// <param name="imageUrl">The URL of the image.</param>
    public BookImage(Guid bookId, string imageUrl) : base()
    {
        BookId = bookId;
        ImageUrl = imageUrl;
    }

    /// <summary>
    /// Gets or sets the ID of the book this image belongs to.
    /// </summary>
    public virtual Guid BookId { get; set; }

    /// <summary>
    /// Gets or sets the URL of the image.
    /// </summary>
    public virtual string ImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets the validator for the BookImage entity.
    /// </summary>
    public override IValidator? GetValidator()
        => new BookImageValidator();
}
