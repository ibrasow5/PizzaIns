using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using PizzaIns;
using PizzaIns.Data;
using PizzaIns.Models;

var builder = WebApplication.CreateBuilder(args);

// Ajouter services Swagger + EF Core SQLite
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Pizzéria",
        Description = "Faire les pizzas que vous aimez",
        Version = "v1"
    });
});

builder.Services.AddDbContext<PizzaDbContext>(options =>
    options.UseSqlite("Data Source=pizzas.db"));

var app = builder.Build();

// Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pizzéria API V1");
});

// Point d’accueil personnalisé
app.MapGet("/", () => "Ecole Supérieure Polytechnique DIT2 2024");

// Endpoints API minimal
app.MapGet("/pizzas", async (PizzaDbContext db) =>
    await db.Pizzas.ToListAsync());

app.MapGet("/pizzas/{id}", async (int id, PizzaDbContext db) =>
    await db.Pizzas.FindAsync(id) is PizzaIns.Models.PizzaIns pizza
        ? Results.Ok(pizza)
        : Results.NotFound());

app.MapPost("/pizzas", async (PizzaIns.Models.PizzaIns pizza, PizzaDbContext db) =>
{
    db.Pizzas.Add(pizza);
    await db.SaveChangesAsync();
    return Results.Created($"/pizzas/{pizza.Id}", pizza);
});

app.MapPut("/pizzas/{id}", async (int id, PizzaIns.Models.PizzaIns input, PizzaDbContext db) =>
{
    var pizza = await db.Pizzas.FindAsync(id);
    if (pizza is null) return Results.NotFound();

    pizza.Nom = input.Nom;
    pizza.Description = input.Description;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/pizzas/{id}", async (int id, PizzaDbContext db) =>
{
    var pizza = await db.Pizzas.FindAsync(id);
    if (pizza is null) return Results.NotFound();

    db.Pizzas.Remove(pizza);
    await db.SaveChangesAsync();
    return Results.Ok(pizza);
});

app.Run();