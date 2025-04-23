using FluentValidation;
using FluentValidation.Results;

public interface IValidatable<out T> where T : IValidator
{
    T GetValidator();

    ValidationResult Validate();
}