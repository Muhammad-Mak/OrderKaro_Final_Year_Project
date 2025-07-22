// Program.cs
// Entry point of the Smart Cafe Backend API. Configures services, middleware, and application startup logic.

using FYP_Backend.Context;
using FYP_Backend.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// -------------------------
// SERVICE CONFIGURATION
// -------------------------

// Register controller support
builder.Services.AddControllers();

// Register Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpClient(); // Optional: used if your app makes outgoing HTTP requests

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Smart Cafe API", Version = "v1" });

    // Add JWT support in Swagger UI
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by a space and your JWT token."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
});

// Allow cross-origin requests from any origin (adjust in production)
builder.Services.AddCors(option =>
{
    option.AddPolicy("MyPolicy", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Register Entity Framework DbContext with SQL Server connection
builder.Services.AddDbContext<AppDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnStr"));
});

// Configure JWT Bearer authentication
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        var jwtConfig = builder.Configuration.GetSection("JwtSettings");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtConfig["Issuer"],

            ValidateAudience = true,
            ValidAudience = jwtConfig["Audience"],

            ValidateLifetime = true, // Ensures token is not expired

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtConfig["Key"]!)) // Secret key used to validate token signature
        };
    });

builder.Services.AddAuthorization(); // Enables role-based and policy-based auth

// Register AutoMapper with custom mapping profile
builder.Services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();

// -------------------------
// HTTP REQUEST PIPELINE
// -------------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // Enables interactive API docs during development
}

app.UseHttpsRedirection();    // Force HTTPS

app.UseCors("MyPolicy");      // Enable CORS with previously defined policy

app.UseAuthentication();      // Apply JWT authentication middleware
app.UseAuthorization();       // Apply authorization checks

app.MapControllers();         // Map attribute-routed controller actions

app.Run();                    // Start the application
