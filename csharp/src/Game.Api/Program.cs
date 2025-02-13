using api.Profiles;
using Game.DAL.Contexts;
using Game.DAL.Interfaces;
using Game.Services.Interfaces;
using Game.Services.Services;
using Game.Services.Utilities;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddTransient<IIdentifierGenerator, IdentifierGenerator>();
builder.Services.AddTransient<IGameService, GameService>();
builder.Services.AddScoped<IGameContext, GameContext>();
builder.Services.AddAutoMapper(config => config.AllowNullCollections = true, typeof(Program).Assembly,
    typeof(GameService).Assembly);

var app = builder.Build();
app.MapControllers();
app.Run();