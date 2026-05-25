using ClassService.Services;
using ClassService.Clients;
using Scalar.AspNetCore;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();



// Services
builder.Services.AddSingleton<IMongoClient>(new MongoClient(builder.Configuration["CosmosDb:AccountKey"]));
builder.Services.AddSingleton<ClassService.Services.ClassService>();
builder.Services.AddSingleton<ClassTemplateService>();

// Clients
builder.Services.AddHttpClient<InstructorClient>(client =>
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
