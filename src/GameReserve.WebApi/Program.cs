using GameReserve.WebApi.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddApiDI();

var app = builder.Build();

app.UseRouting();

app.MapControllers();

app.UseHttpsRedirection();

app.Run();