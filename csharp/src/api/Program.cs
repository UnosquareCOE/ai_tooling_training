using api.Utils;
using services.Interfaces;
using services.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddTransient<IIdentifierGenerator, IdentifierGenerator>();
builder.Services.AddAutoMapper(config => config.AllowNullCollections = true, typeof(Program).Assembly,
    typeof(GameService).Assembly);

var app = builder.Build();
app.MapControllers();
app.Run();