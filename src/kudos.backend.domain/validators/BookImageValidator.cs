using FluentValidation;
using kudos.backend.domain.entities;

namespace kudos.backend.domain.validators;

/// <summary>
/// Validator for the BookImage entity.
/// </summary>
public class BookImageValidator : AbstractValidator<BookImage>
{
    public BookImageValidator()
    {
        RuleFor(x => x.BookId)
            .NotEmpty().WithMessage("BookId is required");

        RuleFor(x => x.ImageUrl)
            .NotEmpty().WithMessage("ImageUrl is required")
            .MaximumLength(500).WithMessage("ImageUrl cannot exceed 500 characters");
    }
}
