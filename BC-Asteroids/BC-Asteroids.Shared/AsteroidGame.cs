using BC_Asteroids.Shared.Config;
using HowlDev.Simulation.Physics.Primitive2D;

namespace BC_Asteroids.Shared;

public class AsteroidGame {
    public Dictionary<int, Player> Players = [];
    private Dictionary<int, PlayerActions?> inputs = [];
    public List<Asteroid> Asteroids { get; set; } = [];
    private int asteroidFrequencyConfig = ConfigClass.Config["game"]["asteroidFrequency"].AsInt();
    private int asteroidFrequency = 10;
    private int AsteroidFrequency {
        get => asteroidFrequency;
        set {
            if (asteroidFrequency < 0) {
                asteroidFrequency = asteroidFrequencyConfig;
            } else {
                asteroidFrequency = value;
            }
        }
    }
    public bool IsStarted { get; private set; }
    public List<Bullet> Bullets = [];
    public (int x, int y) size = (1500, 800); // Size for display is inverted, don't ask why

    public int Register() {
        int newId = Players.Count > 0 ? Players.Keys.Max() + 1 : 1;
        Player newPlayer = new Player(new Point2D(size.x / 2, size.y / 2),
                                      new Vector2D(0, 0),
                                      newId);
        Players.Add(newId, newPlayer);
        inputs.Add(newId, null);
        return newId;
    }

    public void StartGame() {
        IsStarted = true;
    }

    public void SendUpdates(int id, List<string> updates) {
        inputs[id] = GameInputParser.ParseCommands(updates);
    }

    public void GameTick() {
        if (IsStarted) {
            AsteroidFrequency--;
        }
        if (AsteroidFrequency == 0) {
            SpawnAsteroid();
        }

        MoveObjectsAndDestroyBullets();
        if (IsStarted) {
            ProcessBulletCollisions();
            ProcessPlayerCollisions();
        }
    }

    private void ProcessPlayerCollisions() {
        foreach (KeyValuePair<int, Player> playerEntry in Players) {
            Player player = playerEntry.Value;
            if (player.Health <= 0 || player.IsIntangible) continue;

            Asteroid? asteroid = Asteroids.FirstOrDefault(a => a.IsCollided(player));
            if (asteroid is null) continue;

            player.Velocity = player.Velocity.WithVelocity(0);
            CalculateHitAsteroid(asteroid, player);
            player.CalculateDamage(34);
        }
    }

    private void ProcessBulletCollisions() {
        for (int i = 0; i < Bullets.Count; i++) {
            Asteroid? a = Asteroids.FirstOrDefault(a => a.IsCollided(Bullets[i]));
            if (a is not null) {
                int playerId = RemoveBulletAndAddPoints(ref i, a.Level * 50);
                CalculateHitAsteroid(a, Players[playerId]);
                continue;
            }
            Player? p = Players.FirstOrDefault(a => a.Value.Id != Bullets[i].PlayerId && a.Value.Health > 0 && a.Value.IsCollided(Bullets[i])).Value;
            if (p is not null) {
                RemoveBulletAndAddPoints(ref i, 20);
                p.Health--;
            }
        }
    }

    private int RemoveBulletAndAddPoints(ref int i, int points) {
        int playerId = Bullets[i].PlayerId;
        Players[playerId].Score += points;
        Bullets.RemoveAt(i);
        i--;
        return playerId;
    }

    private void MoveObjectsAndDestroyBullets() {
        foreach (KeyValuePair<int, Player> p in Players.Where(p => p.Value.Health > 0)) {
            p.Value.GameTick(inputs[p.Key], size, AddBullet);
            if (IsStarted)
                p.Value.Score++;
        }
        foreach (Asteroid a in Asteroids) {
            a.GameTick(size);
        }
        foreach (Bullet b in Bullets) {
            b.GameTick(size);
        }
        Bullets = [.. Bullets.Where(a => !a.ShouldBeDestroyed)];
    }

    private void CalculateHitAsteroid(Asteroid a, Player p) {
        Asteroids.Remove(a);
        p.Score += a.Level * 50;
        if (a.Level == 3) return;
        Vector2D newVec1 = new(Random.Shared.NextDouble() * 360, Random.Shared.NextDouble() * 1);
        Vector2D newVec2 = new(Random.Shared.NextDouble() * 360, Random.Shared.NextDouble() * 1);
        Asteroids.Add(new Asteroid(a.Boundary.Center, a.Velocity + newVec1, a.Level + 1, (int)Random.Shared.NextInt64(-5, 5)));
        Asteroids.Add(new Asteroid(a.Boundary.Center, a.Velocity + newVec2, a.Level + 1, (int)Random.Shared.NextInt64(-5, 5)));
    }

    private void SpawnAsteroid() {
        Point2D center;
        if (Random.Shared.NextDouble() > 0.5) {
            center = new(0, Random.Shared.NextDouble() * size.x);
        } else {
            center = new(Random.Shared.NextDouble() * size.x, 0);
        }
        Vector2D vec = new(Random.Shared.NextDouble() * 360, Random.Shared.NextDouble() * 4);
        Asteroids.Add(new Asteroid(center, vec, 1, (int)Random.Shared.NextInt64(-5, 5)));
    }

    private void AddBullet(Bullet b) {
        Bullets.Add(b);
    }
}