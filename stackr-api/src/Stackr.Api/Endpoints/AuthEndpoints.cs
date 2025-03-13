using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Stackr_Api.Models;
using Stackr_Api.data;
using BCrypt.Net;
using Microsoft.Extensions.Configuration;

namespace Stackr_Api.Endpoints;

public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(this RouteGroupBuilder group, IConfiguration configuration)
    {
        // Endpoint to register a new user
        // This endpoint accepts a User object and hashes the password before storing it in the database.
        // It returns a success message upon successful registration.
        group.MapPost("/register", async (AppDbContext db, User user) =>
        {
            
            if (string.IsNullOrEmpty(user.Password))
            {
                return Results.BadRequest("Password is required");
            }
            // Hash the user's password for security
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            user.CreatedAt = DateTime.UtcNow;
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return Results.Ok(new { message = "User registered successfully" });
        })
        .WithName("RegisterUser")
        .WithTags("Authentication")
        .WithSummary("Register a new user")
        .WithDescription("Registers a new user in the system. The password will be hashed before storage.");

        // Endpoint to login a user
        // This endpoint accepts a User object with email and password, verifies the credentials, and returns a JWT token.
        group.MapPost("/login", async (AppDbContext db, User loginUser) => 
        {
            // Find the user by email
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == loginUser.Email);
            // Verify the password
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginUser.PasswordHash, user.PasswordHash))
                return Results.Unauthorized();

            // Generate a JWT token for the authenticated user
            var token = GenerateJwtToken(user, configuration);
            return Results.Ok(new { token });
        })
        .WithName("LoginUser")
        .WithTags("Authentication")
        .WithSummary("Login user")
        .WithDescription("Authenticates a user and returns a JWT token for subsequent requests.");

        return group;
    }

    // Helper method to generate a JWT token
    // This method creates a token with the user's email as a claim and signs it with the configured key.
    private static string GenerateJwtToken(User user, IConfiguration configuration)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured")));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[] { new Claim(ClaimTypes.Name, user.Email) };
        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(configuration["Jwt:DurationInMinutes"] ?? "60")),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
} 