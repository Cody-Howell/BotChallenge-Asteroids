using BC_Asteroids.API;
using BC_Asteroids.Shared;
using BC_Asteroids.Shared.Config;
using HowlDev.Web.Helpers.WebSockets;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<AsteroidGame>();
builder.AddWebSocketService<int>();

var app = builder.Build();
app.UseWebSockets();

app.MapGet("/api/game/debug/{id}", (int id, AsteroidGame game) => {
    return JsonSerializer.Serialize(GameDTOCreator.GetDTOForGame(game));
});

app.MapGet("/api/game/tick/{id}", (int id, AsteroidGame game, WebSocketService<int> service) => {
    game.GameTick();
});

app.MapGet("/api/game/start/{id}", (int id, AsteroidGame game, WebSocketService<int> service) => {
    game.StartGame();
});

app.MapGet("/api/game/register/{id}", (int id, AsteroidGame game) => {
    int playerId = game.Register();
    return playerId;
});

app.MapGet("/api/game/size", (AsteroidGame game) => {
    return new { width = game.size.x, height = game.size.y };
});

app.MapPost("/api/game/move/{id}", (int id, AsteroidGame game, [FromBody] List<string> moves) => {
    game.SendUpdates(id, moves);
    return Results.Ok();
});

app.Map("/api/game/{id}", async (int id, HttpContext context, WebSocketService<int> service) => {
    if (context.WebSockets.IsWebSocketRequest) {
        await service.RegisterSocket(context, id);
        return Results.Accepted(); // To satisfy the compiler
    } else {
        return Results.BadRequest("Not a web socket request.");
    }
});

ConfigClass.Initialize("./config.json");

_ = Task.Run(async () => {
    using var scope = app.Services.CreateScope();
    var game = scope.ServiceProvider.GetRequiredService<AsteroidGame>();
    var service = scope.ServiceProvider.GetRequiredService<WebSocketService<int>>();
    while (true) {
        game.GameTick();
        try {
            await service.SendSocketMessage(1, JsonSerializer.Serialize(GameDTOCreator.GetDTOForGame(game)));

        } catch (Exception e) {
            Console.WriteLine($"Error sending socket message: {e.Message}");
        }

        await Task.Delay(TimeSpan.FromSeconds(1) / 30);
    }
});

app.Run();