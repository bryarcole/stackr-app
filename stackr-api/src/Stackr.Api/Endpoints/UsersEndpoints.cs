using Microsoft.EntityFrameworkCore;
using Stackr_Api.Models;
using Stackr_Api.data;

namespace Stackr_Api.Endpoints;

public static class UsersEndpoints
{
    public static RouteGroupBuilder MapUsersEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (AppDbContext db) =>
        {
            var users = await db.Users.ToListAsync();
            return Results.Ok(users);
        })
        .WithName("GetUsers")
        .WithTags("Users")
        .WithSummary("Get all users")
        .WithDescription("Retrieves a list of all users in the system.");

        group.MapGet("/{id}", async (int id, AppDbContext db) =>
        {
            var user = await db.Users.FindAsync(id);
            if (user == null)
                return Results.NotFound();
            return Results.Ok(user);
        })
        .WithName("GetUser")
        .WithTags("Users")
        .WithSummary("Get user by ID")
        .WithDescription("Retrieves a specific user by their ID.");

        group.MapPost("/", async (User user, AppDbContext db) =>
        {
            user.CreatedAt = DateTime.UtcNow;
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return Results.Created($"/api/users/{user.Id}", user);
        })
        .WithName("CreateUser")
        .WithTags("Users")
        .WithSummary("Create a new user")
        .WithDescription("Creates a new user with the provided information.");

        group.MapPut("/{id}", async (int id, User user, AppDbContext db) =>
        {
            var existingUser = await db.Users.FindAsync(id);
            if (existingUser == null)
                return Results.NotFound();

            existingUser.Username = user.Username;
            existingUser.Email = user.Email;
            existingUser.PasswordHash = user.PasswordHash;

            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithName("UpdateUser")
        .WithTags("Users")
        .WithSummary("Update a user")
        .WithDescription("Updates an existing user's information.");

        group.MapDelete("/{id}", async (int id, AppDbContext db) =>
        {
            var user = await db.Users.FindAsync(id);
            if (user == null)
                return Results.NotFound();

            db.Users.Remove(user);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithName("DeleteUser")
        .WithTags("Users")
        .WithSummary("Delete a user")
        .WithDescription("Deletes a user from the system.");

        return group;
    }
} 