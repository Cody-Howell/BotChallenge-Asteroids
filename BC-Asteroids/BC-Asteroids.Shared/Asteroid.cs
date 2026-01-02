using BC_Asteroids.Shared.Config;
using HowlDev.Simulation.Physics.Primitive2D;

namespace BC_Asteroids.Shared;

public class Asteroid : GameObject {
    private double rotationSpeed = 0;

    public double Radius => Boundary.Radius;
    public Rotation2D VisualRotation { get; set; } = new Rotation2D();
    public int Level { get; private set; }
    private static double smallestRadius = ConfigClass.Config["asteroid"]["smallestRadius"].AsDouble();

    public Asteroid(Point2D center, Vector2D vector, int level, int rotationSpeed)
        : base(center, vector, (4 - level) * smallestRadius) {
        Level = level;
        this.rotationSpeed = rotationSpeed;
    }

    public Asteroid(Point2D center, Vector2D vector, int level, int rotationSpeed, int radius)
        : base(center, vector, radius) {
        Level = level;
        this.rotationSpeed = rotationSpeed;
    }

    public new void GameTick((int x, int y) maxSize) {
        base.GameTick(maxSize);

        VisualRotation = VisualRotation.AdjustBy(rotationSpeed);
    }

    public string ToTextFormat() {
        return $"{Boundary.Center.X} {Boundary.Center.Y} {Velocity.Rotation.RotationAngle} {Velocity.Velocity} {Radius} {Level}";
    }

    public static Asteroid FromTextFormat(string input) {
        string[] items = input.Split(' ');
        Point2D center = new(Convert.ToDouble(items[0]), Convert.ToDouble(items[1]));
        Vector2D vec = new(Convert.ToDouble(items[2]), Convert.ToDouble(items[3]));
        int radius = Convert.ToInt32(items[4]);
        int level = Convert.ToInt32(items[5]);
        Asteroid a = new(center, vec, level, 0, radius);

        return a;
    }
}