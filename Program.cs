using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models; // Swagger

using UserManagement.Models;

var builder = WebApplication.CreateBuilder(args);

// Configure the webserver to listen at a specific port
builder.WebHost.UseUrls("http://localhost:5000");

// Connection string for Azure SQL Database
var connectionString = builder.Configuration.GetConnectionString("AzureSqlDatabase") 
    ?? throw new InvalidOperationException("Connection string 'AzureSqlDatabase' not found.");

// Configure services
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<UserDb>(options =>
    options.UseSqlServer(connectionString)); // Use SQL Server as database

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "UserManagement API",
        Description = "Managing users effectively",
        Version = "v1"
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserManagement API V1");
    });
}

// CRUD endpoints for user

// Create a user
app.MapPost("/users", async (UserDb db, User user) =>
{
    await db.Users.AddAsync(user);
    await db.SaveChangesAsync();
    return Results.Created($"/users/{user.Id}", user);
});

// Get all users
app.MapGet("/users", async (UserDb db) => await db.Users.ToListAsync());

// Get a single user by ID
app.MapGet("/users/{id}", async (UserDb db, int id) =>
    await db.Users.FindAsync(id) is User user
        ? Results.Ok(user)
        : Results.NotFound());

// Update a user
app.MapPut("/users/{id}", async (UserDb db, User updatedUser, int id) =>
{
    var user = await db.Users.FindAsync(id);
    if (user is null) return Results.NotFound();

    user.FirstName = updatedUser.FirstName;
    user.LastName = updatedUser.LastName;
    user.Email = updatedUser.Email;
    user.UserName = updatedUser.UserName;
    user.PassWord = updatedUser.PassWord;
    user.AccessLevel = updatedUser.AccessLevel;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Delete a user
app.MapDelete("/users/{id}", async (UserDb db, int id) =>
{
    var user = await db.Users.FindAsync(id);
    if (user is null) return Results.NotFound();

    db.Users.Remove(user);
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.Run();
