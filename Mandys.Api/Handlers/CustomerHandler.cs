using Carter;
using FluentValidation;
using Mandys.Domain;
using Mandys.DTOs;
using Mandys.Services;
using Mandys.Services.Interfaces;
using Mandys.Validators;
using Microsoft.AspNetCore.Mvc;

namespace Mandys.Handlers;

/// <summary>
/// Customer self-service, split from the staff-facing user module. Creating
/// a user on someone else's behalf stays in <see cref="UserHandler"/>.
/// </summary>
public class CustomerHandler : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var customerRoutes = app.MapGroup("/customers")
            .WithTags("Customers");

        customerRoutes.MapPost("/register", RegisterCustomer);

        customerRoutes.MapGet("", GetCustomers)
            .RequireAuthorization(policy => policy.RequireRole(
                Roles.Administrator,
                // Roles.Cashier,
                Roles.BranchChief,
                Roles.OpChief));

        customerRoutes.MapPost("", CreateCustomer)
            .RequireAuthorization(policy => policy.RequireRole(
                Roles.Administrator,
                Roles.Cashier));

        customerRoutes.MapPut("/{id:int}", UpdateCustomer)
            .RequireAuthorization(policy => policy.RequireRole(
                Roles.Administrator,
                Roles.Cashier,
                Roles.Customer,
                Roles.BranchChief));

        // Banning is a punitive account action, so it is narrower than the
        // read and edit policies: no cashier, and never a customer.
        customerRoutes.MapPost("/{id:int}/ban", BanCustomer)
            .RequireAuthorization(policy => policy.RequireRole(
                Roles.Administrator,
                Roles.BranchChief,
                Roles.OpChief));

        customerRoutes.MapDelete("/{id:int}/ban", UnbanCustomer)
            .RequireAuthorization(policy => policy.RequireRole(
                Roles.Administrator,
                Roles.BranchChief,
                Roles.OpChief));
    }

    private static async Task<IResult> GetCustomers(
        ICustomerService customerService,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null)
    {
        return Results.Ok(await customerService.GetCustomersAsync(page, pageSize, search));
    }

    private static async Task<IResult> RegisterCustomer(
        [FromBody] RegisterCustomerRequest request,
        IValidator<RegisterCustomerRequest> validator,
        ICustomerService customerService)
    {
        try
        {
            RequestValidation.ThrowIfInvalid(validator.Validate(request));

            var created = await customerService.RegisterAsync(request);
            return Results.Created($"/users/{created.Id}", created);
        }
        catch (ServiceException ex)
        {
            return MapCustomerError(ex);
        }
    }

    private static async Task<IResult> CreateCustomer(
        [FromBody] CreateCustomerRequest request,
        IValidator<CreateCustomerRequest> validator,
        ICustomerService customerService)
    {
        try
        {
            RequestValidation.ThrowIfInvalid(validator.Validate(request));

            var created = await customerService.CreateAsync(request);
            return Results.Created($"/users/{created.Id}", created);
        }
        catch (ServiceException ex)
        {
            return MapCustomerError(ex);
        }
    }

    private static async Task<IResult> UpdateCustomer(
        int id,
        [FromBody] UpdateCustomerRequest request,
        IValidator<UpdateCustomerRequest> validator,
        ICustomerService customerService)
    {
        try
        {
            RequestValidation.ThrowIfInvalid(validator.Validate(request));

            return Results.Ok(await customerService.UpdateAsync(id, request));
        }
        catch (ServiceException ex)
        {
            return MapCustomerError(ex);
        }
    }

    private static async Task<IResult> BanCustomer(
        int id,
        ICustomerService customerService)
    {
        try
        {
            return Results.Ok(await customerService.BanAsync(id));
        }
        catch (ServiceException ex)
        {
            return MapCustomerError(ex);
        }
    }

    private static async Task<IResult> UnbanCustomer(
        int id,
        ICustomerService customerService)
    {
        try
        {
            return Results.Ok(await customerService.UnbanAsync(id));
        }
        catch (ServiceException ex)
        {
            return MapCustomerError(ex);
        }
    }

    private static IResult MapCustomerError(ServiceException ex) =>
        ex.StatusCode switch
        {
            StatusCodes.Status400BadRequest => Results.BadRequest(new { message = ex.Message }),
            StatusCodes.Status404NotFound => Results.NotFound(new { message = ex.Message }),
            StatusCodes.Status409Conflict => Results.Conflict(new { message = ex.Message }),
            _ => Results.Unauthorized()
        };
}
