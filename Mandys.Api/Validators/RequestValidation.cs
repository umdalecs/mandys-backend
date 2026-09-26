using FluentValidation.Results;
using Mandys.Services;

namespace Mandys.Validators;

/// <summary>
/// Bridges FluentValidation results to the ServiceException flow so handlers
/// keep returning the shared <c>{ message }</c> error shape.
/// </summary>
public static class RequestValidation
{
    public static void ThrowIfInvalid(ValidationResult result)
    {
        if (!result.IsValid)
        {
            throw ServiceException.BadRequest(result.Errors[0].ErrorMessage);
        }
    }
}
