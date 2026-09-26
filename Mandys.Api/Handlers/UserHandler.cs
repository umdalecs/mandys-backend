using System.Security.Claims;
using Carter;
using FluentValidation;
using Mandys.Common;
using Mandys.Domain;
using Mandys.DTOs;
using Mandys.Services;
using Mandys.Services.Interfaces;
using Mandys.Validators;
using Microsoft.AspNetCore.Mvc;

namespace Mandys.Handlers;

public class UserHandler : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var userRoutes = app.MapGroup("/users")
            .WithTags("Users");

        // Profile of currently authenticated user
        userRoutes.MapGet("/me", GetCurrentUser)
            .RequireAuthorization();

        // User management endpoints - restricted to Admin users only
        var adminRoutes = userRoutes.MapGroup("")
            .RequireAuthorization(policy => policy.RequireRole(Roles.Administrator));

        adminRoutes.MapGet("", GetUsers);

        adminRoutes.MapGet("/{id:guid}", GetUserById);

        adminRoutes.MapPost("", CreateUser);

        adminRoutes.MapPut("/{id:guid}", UpdateUser);

        adminRoutes.MapDelete("/{id:guid}", DeleteUser);
    }

    private static async Task<IResult> GetCurrentUser(
        ClaimsPrincipal claimsPrincipal,
        IUserService userService)
    {
        try
        {
            return Results.Ok(await userService.GetCurrentUserAsync(claimsPrincipal.GetUserId()));
        }
        catch (ServiceException ex)
        {
            return MapUserError(ex);
        }
    }

    private static async Task<IResult> GetUsers(
        IUserService userService,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? role = null)
    {
        return Results.Ok(await userService.GetUsersAsync(page, pageSize, search, role));
    }

    private static async Task<IResult> GetUserById(
        int id,
        IUserService userService)
    {
        try
        {
            return Results.Ok(await userService.GetUserByIdAsync(id));
        }
        catch (ServiceException ex)
        {
            return MapUserError(ex);
        }
    }

    private static async Task<IResult> CreateUser(
        [FromBody] CreateUserRequest request,
        IValidator<CreateUserRequest> validator,
        IUserService userService)
    {
        try
        {
            RequestValidation.ThrowIfInvalid(validator.Validate(request));

            var created = await userService.CreateUserAsync(request);
            return Results.Created($"/users/{created.Id}", created);
        }
        catch (ServiceException ex)
        {
            return MapUserError(ex);
        }
    }

    private static async Task<IResult> UpdateUser(
        int id,
        [FromBody] UpdateUserRequest request,
        IValidator<UpdateUserRequest> validator,
        IUserService userService)
    {
        try
        {
            RequestValidation.ThrowIfInvalid(validator.Validate(request));

            return Results.Ok(await userService.UpdateUserAsync(id, request));
        }
        catch (ServiceException ex)
        {
            return MapUserError(ex);
        }
    }

    private static async Task<IResult> DeleteUser(
        int id,
        ClaimsPrincipal claimsPrincipal,
        IUserService userService)
    {
        try
        {
            await userService.DeleteUserAsync(id, claimsPrincipal.GetUserId());
            return Results.NoContent();
        }
        catch (ServiceException ex)
        {
            return MapUserError(ex);
        }
    }

    private static IResult MapUserError(ServiceException ex) =>
        ex.StatusCode switch
        {
            StatusCodes.Status400BadRequest => Results.BadRequest(new { message = ex.Message }),
            StatusCodes.Status404NotFound => Results.NotFound(new { message = ex.Message }),
            StatusCodes.Status409Conflict => Results.Conflict(new { message = ex.Message }),
            _ => Results.Unauthorized()
        };
}
