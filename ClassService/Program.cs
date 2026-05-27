using ClassService.Services;
using ClassService.Clients;
using ClassService.Repositories;
using Scalar.AspNetCore;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

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
app.UseAuthorization();
app.MapControllers();

app.Run();