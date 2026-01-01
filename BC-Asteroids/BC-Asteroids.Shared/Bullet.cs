using BC_Asteroids.Shared.Config;
using HowlDev.Simulation.Physics.Primitive2D;

namespace BC_Asteroids.Shared;

public class Bullet(Point2D center, Vector2D vector, int id) : GameObject(center, vector, Radius) {
    public bool ShouldBeDestroyed { get => Countdown < 0; }
    public int PlayerId { get; private set; } = id;
    public int Countdown = ConfigClass.Config["bullet"]["duration"].AsInt();
    private static double Radius { get => ConfigClass.Config["bullet"]["radius"].AsDouble(); }

    public new void GameTick((int x, int y) maxSize) {
        base.GameTick(maxSize);
        Countdown--;
    }

    public string ToTextFormat() {
        return $"{PlayerId} {Boundary.Center.X} {Boundary.Center.Y} {Velocity.Rotation.RotationAngle} {Velocity.Velocity} {Countdown}";
    }
}