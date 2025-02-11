using api.Profiles;
using Game.Services.Interfaces;
using Game.Services.Services;
using Game.Services.Utilities;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddTransient<IIdentifierGenerator, IdentifierGenerator>();
builder.Services.AddTransient<IGameService, GameService>();
builder.Services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();
app.MapControllers();
app.Run();