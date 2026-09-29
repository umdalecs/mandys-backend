using Carter;
using FluentValidation;
using Mandys.DTOs;
using Mandys.Services;
using Mandys.Services.Interfaces;
using Mandys.Validators;
using Microsoft.AspNetCore.Mvc;

namespace Mandys.Handlers;

public class LogHandler : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var logRoutes = app.MapGroup("/logs")
            .WithTags("Logs")
            .RequireAuthorization();


        logRoutes.MapPost("", CreateLog)
            .RequireAuthorization();
    }

    private static async Task<IResult> CreateLog(
        [FromBody] CreateLogRequest request,
        IValidator<CreateLogRequest> validator,
        ILogService logs)
    {
        try
        {
            RequestValidation.ThrowIfInvalid(validator.Validate(request));

            await logs.CreateLog(request.Verb, request.Description);
            return Results.NoContent();
        }
        catch (ServiceException ex)
        {
            return MapLogError(ex);
        }
    }

    private static IResult MapLogError(ServiceException ex) =>
        ex.StatusCode switch
        {
            StatusCodes.Status400BadRequest => Results.BadRequest(new { message = ex.Message }),
            StatusCodes.Status404NotFound => Results.NotFound(new { message = ex.Message }),
            StatusCodes.Status409Conflict => Results.Conflict(new { message = ex.Message }),
            _ => Results.Unauthorized()
        };
}
