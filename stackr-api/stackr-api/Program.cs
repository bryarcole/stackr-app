using System.Security.Claims;
using System.Security.Permissions;
using System.Text;
using DnsClient.Protocol;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Options;
using Namotion.Reflection;
using Stackr_Api.Models;
using System.IdentityModel.Tokens.Jwt;
using Stackr_Api.data;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using Stackr_Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("StackrDB"));

var app = builder.Build();

app.MapGet("/", () => "Welcome to Stackr App!");

// Map endpoint groups
app.MapGroup("/items").MapItemsEndpoints();
app.MapGroup("/users").MapUsersEndpoints();
app.MapGroup("/lists").MapListsEndpoints();
app.MapGroup("/rankings").MapRankingsEndpoints();
app.MapGroup("/auth").MapAuthEndpoints();

app.Run();
