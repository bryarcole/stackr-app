using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Stackr_Api.data;
using Stackr_Api.Endpoints;

namespace Stackr_Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Set the environment to Development
        builder.Environment.EnvironmentName = "Development";

        // Add services to the container.
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddAuthorization();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new() { Title = "Stackr API", Version = "v1" });
            
            // Add JWT Authentication
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        // Configure JWT Authentication
        var jwtKey = builder.Configuration["Jwt:Key"] ?? "development-super-secret-key-with-minimum-16-characters";
        var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "stackr-api";
        var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "stackr-client";

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });

        // Configure in-memory database
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("StackrDb"));

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Stackr API V1");
            c.RoutePrefix = string.Empty; // Serve the Swagger UI at the root URL
        });

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/error");
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        var api = app.MapGroup("/api");
        api.MapGroup("/auth").MapAuthEndpoints(app.Configuration);
        api.MapGroup("/users").MapUsersEndpoints();
        api.MapGroup("/lists").MapListsEndpoints();
        api.MapGroup("/items").MapItemsEndpoints();
        api.MapGroup("/rankings").MapRankingsEndpoints();

        // Add error handling endpoint
        app.MapGet("/error", () => Results.Problem("An error occurred."));

        app.Run();
    }
} 