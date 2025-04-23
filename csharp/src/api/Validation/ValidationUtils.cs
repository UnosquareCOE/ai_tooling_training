using api.ViewModels;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace api.Validation
{
    public static class ValidationUtils
    {
        public static ResponseErrorViewModel? CreateBadRequestResponse(ValidationResult validationResult)
        {
            if (validationResult.IsValid)
            {
                return null;
            }

            return new ResponseErrorViewModel
            {
                Message = "Validation failed",
                Errors = validationResult.Errors.Select(e => new ErrorDetail
                {
                    Field = e.PropertyName,
                    Message = e.ErrorMessage
                }).ToList()
            };
        }

        public static ResponseErrorViewModel? ValidateGuid(Guid guid)
        {
            var validator = new InlineValidator<Guid>();
            validator.RuleFor(x => x).NotEmpty().WithMessage("GUID cannot be empty");

            var validationResult = validator.Validate(guid);

            if (validationResult.IsValid)
            {
                return null;
            }

            return new ResponseErrorViewModel
            {
                Message = "Validation failed",
                Errors = validationResult.Errors.Select(e => new ErrorDetail
                {
                    Field = "Guid",
                    Message = e.ErrorMessage
                }).ToList()
            };
        }
    }
}
