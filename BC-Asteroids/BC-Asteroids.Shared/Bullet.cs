using BC_Asteroids.Shared.Config;
using HowlDev.Simulation.Physics.Primitive2D;

namespace BC_Asteroids.Shared;

public class Bullet : GameObject {
    public bool ShouldBeDestroyed { get => Countdown < 0; }
    public int PlayerId { get; private set; }
    public int Countdown = ConfigClass.Config["bullet"]["duration"].AsInt();
    private static double Radius { get => ConfigClass.Config["bullet"]["radius"].AsDouble(); }

    public Bullet(Point2D center, Vector2D vector, int id) : base(center, vector, Radius) {
        PlayerId = id;
    }

    public Bullet(Point2D center, Vector2D vector, int id, double radius) : base(center, vector, radius) {
        PlayerId = id;
    }


    public new void GameTick((int x, int y) maxSize) {
        base.GameTick(maxSize);
        Countdown--;
    }

    public string ToTextFormat() {
        return $"{PlayerId} {Boundary.Center.X} {Boundary.Center.Y} {Velocity.Rotation.RotationAngle} {Velocity.Velocity} {Countdown} {Boundary.Radius}";
    }

    public static Bullet FromTextFormat(string input) {
        string[] items = input.Split(' ');
        Point2D center = new(Convert.ToDouble(items[1]), Convert.ToDouble(items[2]));
        Vector2D vec = new(Convert.ToDouble(items[3]), Convert.ToDouble(items[4]));
        Bullet b = new(center, vec, Convert.ToInt32(items[0]), Convert.ToDouble(items[6])) {
            Countdown  = Convert.ToInt32(items[5])
        };

        return b;
    }
}