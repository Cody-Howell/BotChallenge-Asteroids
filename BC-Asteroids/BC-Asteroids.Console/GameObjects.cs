using BC_Asteroids.Shared;

namespace BC_Asteroids.Console; 

public class StringifiedGameObjects {
    public List<string> Players { get; set; } = [];
    public List<string> Asteroids { get; set; } = [];
    public List<string> Bullets { get; set; } = [];
}
public class GameObjects {
    public List<Player> Players { get; set; } = [];
    public List<Asteroid> Asteroids { get; set; } = [];
    public List<Bullet> Bullets { get; set; } = [];
    public override string ToString() {
        return $"""
        {string.Join('\n', Players.Select(p => p.ToTextFormat()))}
        ---
        {string.Join('\n', Asteroids.Select(a => a.ToTextFormat()))}
        ---
        {string.Join('\n', Bullets.Select(b => b.ToTextFormat()))}
        """;
    }
}