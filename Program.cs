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
    new MongoClient(builder.Configuration["CosmosDb:AccountKey"]));
builder.Services.AddSingleton<IMongoDatabase>(sp =>
    sp.GetRequiredService<IMongoClient>()
        .GetDatabase(builder.Configuration["MongoDB:Database"]));

// Repositories
builder.Services.AddScoped<IClassRepository, ClassRepository>();
builder.Services.AddScoped<IClassTemplateRepository, ClassTemplateRepository>();
builder.Services.AddScoped<IClassroomRepository, ClassroomRepository>();

// Services
builder.Services.AddScoped<ClassesService>();
builder.Services.AddScoped<ClassTemplateService>();

// Clients
builder.Services.AddHttpClient<IInstructorClient, InstructorClient>(client =>
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