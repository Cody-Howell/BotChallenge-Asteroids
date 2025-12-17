using BC_Asteroids.Shared.Config;
using HowlDev.Simulation.Physics.Primitve2D;

namespace BC_Asteroids.Shared;

public class Bullet : IGameObject {
    private int countdown = ConfigClass.Config["bullet"]["duration"].AsInt();

    public int PlayerId { get; set; } = -1;
    public Vector2D Velocity { get; set; } = new Vector2D();
    public Point2D Center { get; set; } = new Point2D();
    public double Radius { get => ConfigClass.Config["bullet"]["radius"].AsDouble(); }
    public bool ShouldBeDestroyed { get => countdown < 0; }

    public void GameTick(int ms, (int x, int y) maxSize) {
        Center += new Vector2D(Velocity.Rotation.RotationAngle, Velocity.Velocity * 1000 / ms);
        if (Center.X < 0) Center.X += maxSize.x;
        else if (Center.X > maxSize.x) Center.X += maxSize.x;

        if (Center.Y < 0) Center.Y += maxSize.y;
        else if (Center.Y > maxSize.y) Center.Y += maxSize.y;

        countdown -= ms;
    }

    public bool IsCollided(IGameObject obj) {
        return Center.GetDistance(obj.Center) <= (obj.Radius + Radius);
    }
}