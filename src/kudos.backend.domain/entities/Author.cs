using FluentValidation;
using kudos.backend.domain.validators;

namespace kudos.backend.domain.entities;

/// <summary>
/// Represents an author of books.
/// </summary>
public class Author : AbstractDomainObject
{
    /// <summary>
    /// Parameterless constructor for NHibernate.
    /// </summary>
    public Author() : base() { }

    /// <summary>
    /// Creates a new Author with the specified name.
    /// </summary>
    /// <param name="name">The author's full name.</param>
    public Author(string name) : base()
    {
        Name = name;
    }

    /// <summary>
    /// Gets or sets the author's full name.
    /// </summary>
    public virtual string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the author's country of origin.
    /// </summary>
    public virtual string? Country { get; set; }

    /// <summary>
    /// Gets or sets the author's birth date.
    /// </summary>
    public virtual DateTime? BirthDate { get; set; }

    /// <summary>
    /// Gets the validator for the Author entity.
    /// </summary>
    public override IValidator? GetValidator()
        => new AuthorValidator();
}
