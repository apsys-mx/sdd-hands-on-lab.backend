using FluentValidation;
using kudos.backend.domain.entities;

namespace kudos.backend.domain.validators;

/// <summary>
/// Validator for the Author entity.
/// </summary>
public class AuthorValidator : AbstractValidator<Author>
{
    public AuthorValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters");

        RuleFor(x => x.Country)
            .MaximumLength(100).WithMessage("Country cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Country));

        RuleFor(x => x.BirthDate)
            .LessThan(DateTime.UtcNow).WithMessage("BirthDate must be in the past")
            .When(x => x.BirthDate.HasValue);
    }
}
