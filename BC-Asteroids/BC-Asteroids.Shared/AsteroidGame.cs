using HowlDev.Simulation.Physics.Primitive2D;

namespace BC_Asteroids.Shared;

public class AsteroidGame {
    public Dictionary<int, Player> Players = [];
    private Dictionary<int, List<string>> inputs = [];
    public List<Asteroid> Asteroids { get; set; } = [];
    public List<Bullet> Bullets = [];
    public (int x, int y) size = (700, 700);

    public int Register() {
        int newId = Players.Count > 0 ? Players.Keys.Max() + 1 : 1;
        Player newPlayer = new Player(new Point2D(size.x / 2, size.y / 2),
                                      new Vector2D(0, 0),
                                      newId);
        Players.Add(newId, newPlayer);
        return newId;
    }

    public void SendUpdates(int id, List<string> updates) {
        
        foreach (string update in updates) {
            

        }
    }

    public void GameTick() {
        foreach (KeyValuePair<int, Player> p in Players) {
            p.Value.GameTick(size);
        }
        foreach (Asteroid a in Asteroids) {
            a.GameTick(size);
        }
        foreach (Bullet b in Bullets) {
            b.GameTick(size);
        }
    }
}