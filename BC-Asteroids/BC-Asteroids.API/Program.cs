using BC_Asteroids.API;
using BC_Asteroids.Shared;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<AsteroidGame>();

var app = builder.Build();

app.MapGet("/api/game/{id}", (int id, AsteroidGame game) => {
    AsteroidReturnDTO dto = new();
    foreach (KeyValuePair<int, Player> a in game.Players) {
        Player p = a.Value;
        dto.Players.Add($"{a.Key} {p.Boundary.Center.X} {p.Boundary.Center.Y} {p.VisualRotation.RotationAngle} {p.Health} {p.TimeToFire}");
    }
    foreach (Bullet b in game.Bullets) {
        dto.Bullets.Add($"{b.PlayerId} {b.Boundary.Center.X} {b.Boundary.Center.Y} {b.Velocity.Rotation.RotationAngle} {b.Velocity.Velocity} {b.Countdown}");
    }
    foreach (Asteroid a in game.Asteroids) {
        dto.Asteroids.Add($"{a.Boundary.Center.X} {a.Boundary.Center.Y} {a.Velocity.Rotation.RotationAngle} {a.Velocity.Velocity} {a.Level}");
    }
    return dto;
});

app.MapGet("/api/game/tick/{id}", (int id, AsteroidGame game) => {
    game.GameTick();
});

app.MapGet("/api/game/register/{id}", (int id, AsteroidGame game) => {
    int playerId = game.Register();
    return playerId;
});

app.MapPost("/api/game/move/{id}", (int id, AsteroidGame game, [FromBody] List<string> moves) => {
    game.SendUpdates(id, moves);
    return Results.Ok();
});

app.Run();