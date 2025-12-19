namespace BC_Asteroids.Shared;

public class AsteroidGame {
    public Player Player { get; set; } = new Player() {
        Center = new Point2D(200, 200)
    };
    public List<Asteroid> Asteroids { get; set; } = new List<Asteroid>() {
        new Asteroid(new Vector2D(30, 3), 2, new Point2D(0, 50), 1)
    };

    public void GameTick() {
        Player.GameTick(50, (700, 700));
        foreach (Asteroid a in Asteroids) {
            a.GameTick(50, (700, 700));
        }
    }
}