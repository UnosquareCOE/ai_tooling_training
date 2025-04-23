using dal;
using Microsoft.EntityFrameworkCore;
using service.interfaces;
using service.services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddScoped<AppDbContext>();
builder.Services.AddTransient<IIdentifierGenerator, IdentifierGeneratorService>();
builder.Services.AddTransient<IWordService, WordService>();
builder.Services.AddScoped<IHangmanGameService, HangmanGameService>();

builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddAutoMapper(typeof(GameProfile));


var app = builder.Build();
app.MapControllers();
app.Run();