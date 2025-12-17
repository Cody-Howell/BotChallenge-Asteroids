using HowlDev.Simulation.Physics.Primitve2D;

namespace BC_Asteroids.Shared;

public interface IGameObject {
    public Vector2D Velocity { get; }
    public Point2D Center { get; }
    public double Radius { get; }
    public bool IsCollided(IGameObject obj);
    public void GameTick(int ms, (int x, int y) maxSize);
}