using Microsoft.AspNetCore.Mvc;
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
        });

        group.MapGet("/{id}", async (AppDbContext db, int id) => 
        {
            var user = await db.Users.FindAsync(id);
            if(user == null) Results.NotFound();
            return Results.Ok(user);
        });

        group.MapPost("/", async (AppDbContext db, User user) => 
        {
            db.Users.Add(user);
            await db.SaveChangesAsync();    
            return Results.Created($"/users/{user.Id}", user);
        });

        group.MapPut("/{id}", async (AppDbContext db, int id, User updatedUser) => 
        {
            var user = await db.Users.FindAsync(id);
            if(user == null) Results.NotFound();

            user.Username = updatedUser.Username;
            user.Email = updatedUser.Email;
            await db.SaveChangesAsync();
            return Results.Ok(user);
        });

        group.MapDelete("/{id}", async (AppDbContext db, int id) => 
        {
            var user = await db.Users.FindAsync(id);
            if(user == null) Results.NotFound();
            return Results.Ok();
        });

        return group;
    }
} 