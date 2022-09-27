// MailKit email
global using MailKit.Net.Smtp;
global using MailKit.Security;
global using Microsoft.EntityFrameworkCore;
global using MimeKit;
global using MimeKit.Text;
// DataAnnotations
global using System.ComponentModel.DataAnnotations;
// ENCRYPT PASSWORD
global using System.Security.Cryptography;
// CONTEXT
global using UserSignUpAPI.Data;
// ENTITIES
global using UserSignUpAPI.Models;
global using UserSignUpAPI.Models.Email;
global using UserSignUpAPI.Services.EmailServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Se configura en la clase la conexión a sqlServer
builder.Services.AddDbContext<DataContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
