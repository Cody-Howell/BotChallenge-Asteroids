using BC_Asteroids.Shared.Config;
using HowlDev.Simulation.Physics.Primitve2D;

namespace BC_Asteroids.Shared;

public class Asteroid : IGameObject {
    private double rotationSpeed = 0;

    public Vector2D Velocity { get; set; } = new Vector2D();
    public Point2D Center { get; set; } = new Point2D();
    public double Radius { get => (4 - Level) * ConfigClass.Config["asteroid"]["smallestRadius"].AsDouble(); }
    public Rotation2D VisualRotation { get; set; } = new Rotation2D();
    public int Level { get; private set; }

    public Asteroid(Vector2D velocity, double rotationSpeed, Point2D center, int level) {
        this.rotationSpeed = rotationSpeed;
        Velocity = velocity;
        Center = center;
        Level = level;
    }

    public bool IsCollided(IGameObject obj) {
        return Center.GetDistance(obj.Center) <= (obj.Radius + Radius);
    }

    public void GameTick(int ms, (int x, int y) maxSize) {
        Center += new Vector2D(Velocity.Rotation.RotationAngle, Velocity.Velocity * (ms / 1000.0));

        if (Center.X < 0) Center.X += maxSize.x;
        else if (Center.X > maxSize.x) Center.X -= maxSize.x;

        if (Center.Y < 0) Center.Y += maxSize.y;
        else if (Center.Y > maxSize.y) Center.Y -= maxSize.y;

        VisualRotation.AdjustBy(rotationSpeed * (ms / 1000.0));
    }
}