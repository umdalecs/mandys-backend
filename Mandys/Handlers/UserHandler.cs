using System.Security.Claims;
using Carter;
using Isopoh.Cryptography.Argon2;
using Mandys.Common;
using Mandys.DTOs;
using Mandys.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mandys.Entities;

namespace Mandys.Handlers;

public class UserHandler : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var userRoutes = app.MapGroup("/users")
            .WithTags("Users");

        // Profile of currently authenticated user
        userRoutes.MapGet("/me", GetCurrentUser)
            .RequireAuthorization()
            .WithName("GetCurrentUser")
            .WithSummary("Get the current authenticated user's profile");

        // User management endpoints - restricted to Admin users only
        var adminRoutes = userRoutes.MapGroup("")
            .RequireAuthorization(policy => policy.RequireRole(Roles.Admin));

        adminRoutes.MapGet("", GetUsers)
            .WithName("GetUsers")
            .WithSummary("List all users with pagination and filtering (Admin only)");

        adminRoutes.MapGet("/{id:guid}", GetUserById)
            .WithName("GetUserById")
            .WithSummary("Get user details by ID (Admin only)");

        adminRoutes.MapPost("", CreateUser)
            .WithName("CreateUser")
            .WithSummary("Create a new user with role (Admin only)");

        adminRoutes.MapPut("/{id:guid}", UpdateUser)
            .WithName("UpdateUser")
            .WithSummary("Edit an existing user (Admin only)");

        adminRoutes.MapDelete("/{id:guid}", DeleteUser)
            .WithName("DeleteUser")
            .WithSummary("Delete a user (Admin only)");
    }

    private static async Task<IResult> GetCurrentUser(
        ClaimsPrincipal claimsPrincipal,
        ApplicationDbContext db)
    {
        var currentUserId = claimsPrincipal.GetUserId();

        if (currentUserId is null)
        {
            return Results.Unauthorized();
        }

        var user = await db.Users.FindAsync(currentUserId.Value);
        if (user is null)
        {
            return Results.NotFound(new { message = "User not found." });
        }

        return Results.Ok(user.ToResponse());
    }

    private static async Task<IResult> GetUsers(
        ApplicationDbContext db,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? role = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var query = db.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(u =>
                u.Email.ToLower().Contains(term) ||
                u.FirstName.ToLower().Contains(term) ||
                u.LastName.ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(role))
        {
            var roleFilter = role.Trim().ToLower();
            query = query.Where(u => u.Role.ToLower() == roleFilter);
        }

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .OrderBy(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => u.ToResponse())
            .ToListAsync();

        return Results.Ok(new PagedUsersResponse(totalCount, page, pageSize, totalPages, items));
    }

    private static async Task<IResult> GetUserById(
        Guid id,
        ApplicationDbContext db)
    {
        var user = await db.Users.FindAsync(id);
        if (user is null)
        {
            return Results.NotFound(new { message = $"User with ID '{id}' not found." });
        }

        return Results.Ok(user.ToResponse());
    }

    private static async Task<IResult> CreateUser(
        [FromBody] CreateUserRequest request,
        ApplicationDbContext db)
    {
        // TODO: Change for fluent validation
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.UserName) ||
            string.IsNullOrWhiteSpace(request.Password) ||
            string.IsNullOrWhiteSpace(request.FirstName) ||
            string.IsNullOrWhiteSpace(request.LastName))
        {
            return Results.BadRequest(new { message = "First name, last name, username, email, and password are required." });
        }

        var role = string.IsNullOrWhiteSpace(request.Role)
            ? Roles.User
            : Roles.Normalize(request.Role.Trim());

        if (!Roles.IsValid(role))
        {
            return Results.BadRequest(new
            {
                message = $"Invalid role '{request.Role}'. Allowed roles: {string.Join(", ", Roles.All)}."
            });
        }

        var normalizedUserName = request.UserName.Trim();
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var emailExists = await db.Users.AnyAsync(u => u.Email.ToLower() == normalizedEmail);
        if (emailExists)
        {
            return Results.Conflict(new { message = $"Email '{normalizedEmail}' is already registered." });
        }

        var user = new UserEntity
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = normalizedEmail,
            Password = Argon2.Hash(request.Password),
            Role = role
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return Results.Created($"/users/{user.Id}", user.ToResponse());
    }

    private static async Task<IResult> UpdateUser(
        Guid id,
        [FromBody] UpdateUserRequest request,
        ApplicationDbContext db)
    {
        var user = await db.Users.FindAsync(id);
        if (user is null)
        {
            return Results.NotFound(new { message = $"User with ID '{id}' not found." });
        }

        if (!string.IsNullOrWhiteSpace(request.Email) && request.Email.Trim().ToLowerInvariant() != user.Email.ToLowerInvariant())
        {
            var trimmedEmail = request.Email.Trim().ToLowerInvariant();
            var emailExists = await db.Users.AnyAsync(u => u.Id != id && u.Email.ToLower() == trimmedEmail);
            if (emailExists)
            {
                return Results.Conflict(new { message = $"Email '{trimmedEmail}' is already registered." });
            }
            user.Email = trimmedEmail;
        }

        // TODO: Use fluent validation here
        if (!string.IsNullOrWhiteSpace(request.FirstName))
        {
            user.FirstName = request.FirstName.Trim();
        }

        if (!string.IsNullOrWhiteSpace(request.LastName))
        {
            user.LastName = request.LastName.Trim();
        }

        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            var normalizedRole = Roles.Normalize(request.Role.Trim());
            if (!Roles.IsValid(normalizedRole))
            {
                return Results.BadRequest(new
                {
                    message = $"Invalid role '{request.Role}'. Allowed roles: {string.Join(", ", Roles.All)}."
                });
            }
            user.Role = normalizedRole;
        }

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            user.Password = Argon2.Hash(request.Password);
        }

        user.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        return Results.Ok(user.ToResponse());
    }

    private static async Task<IResult> DeleteUser(
        Guid id,
        ClaimsPrincipal claimsPrincipal,
        ApplicationDbContext db)
    {
        var currentUserId = claimsPrincipal.GetUserId();

        if (currentUserId.HasValue)
        {
            if (currentUserId.Value == id)
            {
                return Results.BadRequest(new { message = "Administrators cannot delete their own account." });
            }
        }

        var user = await db.Users.FindAsync(id);
        if (user is null)
        {
            return Results.NotFound(new { message = $"User with ID '{id}' not found." });
        }

        db.Users.Remove(user);
        await db.SaveChangesAsync();

        return Results.NoContent();
    }
}