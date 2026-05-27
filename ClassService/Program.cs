using System.Text;
using ClassService.Services;
using ClassService.Clients;
using ClassService.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "FitLife",

            ValidateAudience = true,
            ValidAudience = "FitLifeUsers",

            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this-is-a-development-secret-key-with-enough-length")),
            ValidateIssuerSigningKey = true,
        };
    });

// MongoDB / CosmosDB
builder.Services.AddSingleton<IMongoClient>(
    new MongoClient(builder.Configuration["MongoDB:ConnectionString"]));
builder.Services.AddSingleton<IMongoDatabase>(sp =>
    sp.GetRequiredService<IMongoClient>()
        .GetDatabase(builder.Configuration["MongoDB:DatabaseName"]));

// Repositories
builder.Services.AddScoped<IClassRepository, ClassRepository>();
builder.Services.AddScoped<IClassTemplateRepository, ClassTemplateRepository>();
builder.Services.AddScoped<ICenterRepository, CenterRepository>();  

// Services
builder.Services.AddScoped<IClassesService, ClassesService>();
builder.Services.AddScoped<IClassTemplateService, ClassTemplateService>();

// Clients
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