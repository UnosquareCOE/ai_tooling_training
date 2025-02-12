using FluentValidation;
using FluentValidation.Results;

namespace api.ViewModels;

public class GuessViewModel : IValidatable<GuessViewModelValidator>
{
    public char? Letter { get; set; }

    public GuessViewModelValidator GetValidator() => new();

    public ValidationResult Validate() => GetValidator().Validate(this);
}

public class GuessViewModelValidator : AbstractValidator<GuessViewModel>
{
    public GuessViewModelValidator()
    {
        RuleFor(x => x.Letter)
            .NotEmpty().WithMessage("Letter is required.");

        RuleFor(x => x.Letter)
            .Must(x => char.IsLetter(x!.Value)).WithMessage("Letter must be alphabetical.")
            .When(x => x.Letter.HasValue);
    }
}