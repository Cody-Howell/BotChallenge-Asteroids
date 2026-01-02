using BC_Asteroids.Shared;
using System.Collections.Concurrent;
using System.Text.Json;

namespace BC_Asteroids.API;

public class GameService {
    private readonly ConcurrentDictionary<int, AsteroidGame> _games = new();
    private int _nextGameId = 1;
    private readonly object _lock = new();

    public int CreateGame() {
        int gameId;
        lock (_lock) {
            gameId = _nextGameId++;
        }

        var game = new AsteroidGame();
        _games.TryAdd(gameId, game);

        Console.WriteLine($"Created new game with ID: {gameId}");
        return gameId;
    }

    public Dictionary<int, AsteroidGame> GetAllGames() {
        return new Dictionary<int, AsteroidGame>(_games);
    }

    public AsteroidGame? GetGame(int gameId) {
        _games.TryGetValue(gameId, out var game);
        return game;
    }

    public int RegisterToGame(int gameId) {
        if (_games.TryGetValue(gameId, out var game)) {
            int playerId = game.Register();
            Console.WriteLine($"Player {playerId} registered to game {gameId}");
            return playerId;
        }

        throw new KeyNotFoundException($"Game with ID {gameId} not found");
    }

    public void UpdatePlayerToGame(int gameId, int playerId, List<string> moves) {
        if (_games.TryGetValue(gameId, out var game)) {
            game.SendUpdates(playerId, moves);
            return;
        }

        throw new KeyNotFoundException($"Game with ID {gameId} not found");
    }

    public void StartGame(int gameId, Action<int, string> updateSockets) {
        if (_games.TryGetValue(gameId, out var game)) {
            if (game.IsStarted) return;
            game.StartGame();
            _ = Task.Run(async () => {
                while (!game.IsOver) {
                    game.GameTick();
                    updateSockets(gameId, JsonSerializer.Serialize(GameDTOCreator.GetDTOForGame(game)));
                    await Task.Delay(TimeSpan.FromSeconds(1) / 30);
                }
            });
            return;
        }

        throw new KeyNotFoundException($"Game with ID {gameId} not found");
    }

    public bool DeleteGame(int gameId) {
        bool removed = _games.TryRemove(gameId, out _);
        if (removed) {
            Console.WriteLine($"Deleted game with ID: {gameId}");
        }
        return removed;
    }

    public int GetGameCount() {
        return _games.Count;
    }
}
