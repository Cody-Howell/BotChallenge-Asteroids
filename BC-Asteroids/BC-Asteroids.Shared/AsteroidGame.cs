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
    public bool IsStarted {get; private set;}
    public List<Bullet> Bullets = [];
    public (int x, int y) size = (1000, 1000);

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

        foreach (KeyValuePair<int, Player> p in Players) {
            p.Value.GameTick(inputs[p.Key], size, AddBullet);
        }
        foreach (Asteroid a in Asteroids) {
            a.GameTick(size);
        }
        foreach (Bullet b in Bullets) {
            b.GameTick(size);
        }
        Bullets = [.. Bullets.Where(a => !a.ShouldBeDestroyed)];
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