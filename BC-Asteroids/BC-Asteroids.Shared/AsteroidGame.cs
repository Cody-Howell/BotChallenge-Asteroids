using BC_Asteroids.Shared.Config;
using HowlDev.AI.Core;
using HowlDev.Simulation.Physics.Primitive2D;

namespace BC_Asteroids.Shared;

public class AsteroidGame : IGeneticRunner<int> {
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
    public bool IsOver => Players.Count > 0 && Players.All(a => a.Value.Health <= 0);
    public List<Bullet> Bullets = [];
    public (int x, int y) size = (1500, 800); // Size for display is inverted, don't ask why

    public int Register() {
        int newId = Players.Count > 0 ? Players.Keys.Max() + 1 : 1;
        AddPlayer(newId);
        return newId;
    }

    private void AddPlayer(int id) {
        Player newPlayer = new Player(new Point2D(size.x / 2, size.y / 2),
                                      new Vector2D(0, 0),
                                      id);
        Players.Add(id, newPlayer);
        inputs.Add(id, null);
    }

    public void StartGame() {
        IsStarted = true;
    }

    public void StopGame() {
        foreach (Player p in Players.Values) {
            p.CalculateDamage(100);
        }
    }

    public void ReadUpdates(int id, List<string> updates) {
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

    #region IGeneticRunner implementation
    public static IGeneticRunner<int> Initialize(IEnumerable<int> ids) {
        AsteroidGame game = new();
        foreach (int id in ids) {
            game.AddPlayer(id);
        }
        game.StartGame();
        return game;
    }

    public double[] GetRepresentation(int id) {
        Player p = Players[id];
        double[] neurons = new double[23];
        neurons[0] = p.VisualRotation.DistanceTo(p.Velocity.Rotation) / 180;
        neurons[1] = p.Velocity.Velocity / 100;

        (double distance, Asteroid asteroid)[] closeAsteroids = [.. Asteroids.Select(a => (p.Boundary.Center.GetDistance(a.Boundary.Center), a)).Where(a => a.Item1 <= 300)];
        (double angleDistance, double distance, double threat)[] gameObjectLocations = 
            [.. closeAsteroids.Select((a => (
                p.VisualRotation.DistanceTo(Rotation2D.FromCoordinates(p.Boundary.Center, a.asteroid.Boundary.Center)), 
                a.distance, 
                ThreatLevel(p, a.asteroid)
            )))
            .OrderBy(a => a.Item1)];
        
        (double minAngle, double maxAngle)[] angleBins = [
            (-2, 2), 
            (-20, -2), 
            (2, 20), 
            (-90, -20), 
            (20, 90), 
            (-180, -90), 
            (90, 180), 
        ];
        
        int neuronIndex = 2;
        foreach (var (minAngle, maxAngle) in angleBins) {
            // Find all asteroids in this angle bin
            var asteroidsInBin = gameObjectLocations
                .Where(a => a.angleDistance >= minAngle && a.angleDistance < maxAngle)
                .OrderBy(a => a.distance)
                .ToList();
            
            if (asteroidsInBin.Count > 0) {
                // Get the closest asteroid in this bin
                var (angleDistance, distance, threat) = asteroidsInBin[0];
                neurons[neuronIndex] = distance / 300.0;      // Normalized distance
                neurons[neuronIndex + 1] = threat / 10.0;     // Normalized threat
                neurons[neuronIndex + 2] = 1.0;                        // Bin occupied flag
            } else {
                neurons[neuronIndex] = 1.0;      // Max distance (no asteroid)
                neurons[neuronIndex + 1] = 0.0;  // No threat
                neurons[neuronIndex + 2] = 0.0;  // Bin empty flag
            }
            
            neuronIndex += 3;
            if (neuronIndex >= neurons.Length) break;
        }
        
        return neurons;
    }

    public void PrepareAction(int id, List<double> outputs) {
        if (outputs.Count != 3) throw new Exception("Output not of expected length: 3");

        List<string> moves = [];
        moves.Add($"TURN {Clamp(outputs[0], -1, 1)}");
        moves.Add($"ACCEL {Clamp(outputs[1], -1, 1)}");
        if (Clamp(outputs[2], 0, 1) >= 0.5) {
            moves.Add("FIRE");
        }
        ReadUpdates(id, moves);
    }

    public void RunTick() {
        GameTick();
    }

    public double GetEvaluation(int id) {
        return Players[id].Score;
    }

    private static double Clamp(double input, double start, double end) {
        if (input > end) return end;
        if (input < start) return start;
        return input;
    }

    private static double ThreatLevel(Player p, GameObject o) {
        double threat = 0.0;
        double angleDistance = o.Velocity.Rotation.DistanceTo(Rotation2D.FromCoordinates(o.Boundary.Center, p.Boundary.Center));
        if (angleDistance < 10) {
            threat = 10 / angleDistance;
            threat *= o.Velocity.Velocity / 10;
        }
        return threat;
    }
    #endregion
    #region Boring things
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
                RemoveBulletAndAddPoints(ref i, 100);
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
    #endregion
}