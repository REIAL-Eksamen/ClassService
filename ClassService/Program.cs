using MassTransit;
using System.Text;
using ClassService.Services;
using ClassService.Clients;
using ClassService.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(); // Tilføjer controllers, så API-endpoints kan bruges.
builder.Services.AddOpenApi(); // Tilføjer OpenAPI/Swagger-lignende dokumentation.

// Sætter JWT authentication op, så beskyttede endpoints kan kræve login-token.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Tjekker at tokenet kommer fra den forventede issuer.
            ValidateIssuer = true,
            ValidIssuer = "FitLife",

            // Tjekker at tokenet er lavet til den rigtige audience.
            ValidateAudience = true,
            ValidAudience = "FitLifeUsers",

            // Tjekker at tokenet ikke er udløbet.
            ValidateLifetime = true,
            // Secret key bruges til at validere signaturen på JWT-tokenet.
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this-is-a-development-secret-key-with-enough-length")),
            ValidateIssuerSigningKey = true, // Tjekker at tokenets signatur passer med secret key.
        };
    });

// MongoDB / CosmosDB
// Opretter MongoClient ud fra connection string i configuration.
builder.Services.AddSingleton<IMongoClient>(
    new MongoClient(builder.Configuration["MongoDB:ConnectionString"]));

// Finder den database, som ClassService skal bruge.
builder.Services.AddSingleton<IMongoDatabase>(sp =>
    sp.GetRequiredService<IMongoClient>()
        .GetDatabase(builder.Configuration["MongoDB:DatabaseName"]));

// Repositories
// Registrerer repository-laget, som står for databaseadgang.
builder.Services.AddScoped<IClassRepository, ClassRepository>();
builder.Services.AddScoped<IClassTemplateRepository, ClassTemplateRepository>();
builder.Services.AddScoped<ICenterRepository, CenterRepository>();  

// Services
// Registrerer service-laget, som indeholder forretningslogikken.
builder.Services.AddScoped<IClassesService, ClassesService>();
builder.Services.AddScoped<IClassTemplateService, ClassTemplateService>();

// Tilføjer memory cache, som bruges til class overview.
builder.Services.AddMemoryCache();

// Sætter MassTransit op med RabbitMQ, så service kan publicere events.
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        // Forbinder til RabbitMQ-containeren.
        cfg.Host(builder.Configuration["RabbitMQ:Host"] ?? "rabbitmq", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        // Konfigurerer event endpoints automatisk.
        cfg.ConfigureEndpoints(context);
    });
});

// Clients
// Registrerer AdminClient, så ClassService kan kalde AdminService via HTTP.
builder.Services.AddHttpClient<IAdminClient, AdminClient>(client =>  
{
    client.BaseAddress = new Uri(builder.Configuration["AdminService:BaseUrl"]!);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();