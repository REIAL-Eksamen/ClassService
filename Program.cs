using ClassService.Services;
using ClassService.Clients;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton<ClassService.Services.ClassService>();
builder.Services.AddSingleton<ClassTemplateService>();
builder.Services.AddSingleton<ClassroomService>();

builder.Services.AddHttpClient<AdminClient>(client =>
    client.BaseAddress = new Uri(builder.Configuration["Services:AdminService"]!));

builder.Services.AddHttpClient<UserClient>(client =>
    client.BaseAddress = new Uri(builder.Configuration["Services:UserService"]!));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
