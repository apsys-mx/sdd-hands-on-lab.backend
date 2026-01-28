using FluentValidation;
using kudos.backend.domain.entities;

namespace kudos.backend.domain.validators;

/// <summary>
/// Validator for the Book entity.
/// </summary>
public class BookValidator : AbstractValidator<Book>
{
    public BookValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(300).WithMessage("Title cannot exceed 300 characters");

        RuleFor(x => x.ISBN)
            .NotEmpty().WithMessage("ISBN is required")
            .MaximumLength(20).WithMessage("ISBN cannot exceed 20 characters");

        RuleFor(x => x.AuthorId)
            .NotEmpty().WithMessage("AuthorId is required");

        RuleFor(x => x.Category)
            .MaximumLength(100).WithMessage("Category cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Category));

        RuleFor(x => x.CoverImageUrl)
            .MaximumLength(500).WithMessage("CoverImageUrl cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.CoverImageUrl));
    }
}
