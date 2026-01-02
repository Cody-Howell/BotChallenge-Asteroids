using BC_Asteroids.API;
using BC_Asteroids.Shared;
using BC_Asteroids.Shared.Config;
using HowlDev.Web.Helpers.WebSockets;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<AsteroidGame>();
builder.Services.AddSingleton<GameService>();
builder.AddWebSocketService<int>();

var app = builder.Build();
app.UseWebSockets();

// Generate random admin key
string GenerateAdminKey(int length = 32)
{
    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()-_=+[]{}|;:,.<>?";
    var random = new Random();
    var adminKey = new string([.. Enumerable.Range(0, length).Select(_ => chars[random.Next(chars.Length)])]);
    return adminKey;
}

var adminKey = GenerateAdminKey();
Console.WriteLine($"Admin Key: {adminKey}");

// Admin endpoints
app.MapPost("/api/admin/game/create", (HttpContext context, GameService games) => {
    if (!context.Request.Headers.TryGetValue("Admin-Key", out var key) || key != adminKey) {
        return Results.Unauthorized();
    }
    
    int gameId = games.CreateGame();
    return Results.Ok(new { gameId });
});

app.MapPost("/api/admin/game/start/{id}", (int id, HttpContext context, GameService games, WebSocketService<int> service) => {
    if (!context.Request.Headers.TryGetValue("Admin-Key", out var key) || key != adminKey) {
        return Results.Unauthorized();
    }
    
    try {
        games.StartGame(id, (gameId, data) => {
            try {
                service.SendSocketMessage(gameId, data).Wait();
            } catch (Exception e) {
                Console.WriteLine($"Error sending socket message for game {gameId}: {e.Message}");
            }
        });
        return Results.Ok(new { message = $"Game {id} started" });
    } catch (KeyNotFoundException ex) {
        return Results.NotFound(new { error = ex.Message });
    }
});

app.MapDelete("/api/admin/game/{id}", (int id, HttpContext context, GameService games) => {
    if (!context.Request.Headers.TryGetValue("Admin-Key", out var key) || key != adminKey) {
        return Results.Unauthorized();
    }
    
    bool deleted = games.DeleteGame(id);
    if (!deleted) {
        return Results.NotFound(new { error = $"Game with ID {id} not found" });
    }
    
    return Results.Ok(new { message = $"Game {id} deleted" });
});

app.MapGet("/api/admin/games", (HttpContext context, GameService games) => {
    if (!context.Request.Headers.TryGetValue("Admin-Key", out var key) || key != adminKey) {
        return Results.Unauthorized();
    }
    
    var allGames = games.GetAllGames();
    return Results.Ok(new { 
        count = allGames.Count,
        games = allGames.Select(g => new { 
            gameId = g.Key, 
            isStarted = g.Value.IsStarted, 
            isOver = g.Value.IsOver,
            playerCount = g.Value.Players.Count
        })
    });
});

// Public game endpoints
app.MapGet("/api/game/debug/{id}", (int id, GameService games) => {
    AsteroidGame? game = games.GetGame(id);
    // if (game is null) {
    //     return Results.NotFound(new { error = $"Game with ID {id} not found" });
    // }
    return JsonSerializer.Serialize(GameDTOCreator.GetDTOForGame(game));
});

app.MapPost("/api/game/register/{gameId}", (int gameId, GameService games) => {
    try {
        int playerId = games.RegisterToGame(gameId);
        return Results.Ok(new { playerId, gameId });
    } catch (KeyNotFoundException ex) {
        return Results.NotFound(new { error = ex.Message });
    }
});

app.MapGet("/api/game/size/{id}", (int id, GameService games) => {
    AsteroidGame? game = games.GetGame(id);
    if (game is null) {
        return Results.NotFound(new { error = $"Game with ID {id} not found" });
    }
    return Results.Ok(new { width = game.size.x, height = game.size.y });
});

app.MapPost("/api/game/move/{gameId}/{playerId}", (int gameId, int playerId, GameService games, [FromBody] List<string> moves) => {
    try {
        games.UpdatePlayerToGame(gameId, playerId, moves);
        return Results.Ok(new { message = "Move processed" });
    } catch (KeyNotFoundException ex) {
        return Results.NotFound(new { error = ex.Message });
    }
});

app.Map("/api/game/ws/{id}", async (int id, HttpContext context, WebSocketService<int> service, GameService games) => {
    var game = games.GetGame(id);
    if (game is null) {
        return Results.NotFound(new { error = $"Game with ID {id} not found" });
    }
    
    if (context.WebSockets.IsWebSocketRequest) {
        await service.RegisterSocket(context, id);
        return Results.Accepted();
    } else {
        return Results.BadRequest(new { error = "Not a web socket request" });
    }
});

ConfigClass.Initialize("./config.json");

app.Run();