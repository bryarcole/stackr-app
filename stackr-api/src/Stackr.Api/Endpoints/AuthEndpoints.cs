using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Stackr_Api.Models;
using Stackr_Api.data;
using BCrypt.Net;

namespace Stackr_Api.Endpoints;

public static class AuthEndpoints
{
    private static readonly string JWT_KEY = "YourSuperSecretKeyHere1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890";

    public static RouteGroupBuilder MapAuthEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/register", async (AppDbContext db, User user) =>
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            user.CreatedAt = DateTime.UtcNow;
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return Results.Ok(new { message = "User registered successfully" });
        });

        group.MapPost("/login", async (AppDbContext db, User loginUser) => 
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == loginUser.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginUser.PasswordHash, user.PasswordHash))
                return Results.Unauthorized();

            var token = GenerateJwtToken(user);
            return Results.Ok(new { token });
        });

        return group;
    }

    private static string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWT_KEY));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[] { new Claim(ClaimTypes.Name, user.Email) };
        var token = new JwtSecurityToken("Issuer", "Audience", claims, expires: DateTime.Now.AddHours(3), signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
} 