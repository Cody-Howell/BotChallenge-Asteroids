using BC_Asteroids.Shared.Config;
using HowlDev.Simulation.Physics.Primitive2D;

namespace BC_Asteroids.Shared;

public class Asteroid : GameObject {
    private double rotationSpeed = 0;

    private double Radius { get => (4 - Level) * ConfigClass.Config["asteroid"]["smallestRadius"].AsDouble(); }
    public Rotation2D VisualRotation { get; set; } = new Rotation2D();
    public int Level { get; private set; }

    public Asteroid(Point2D center, Vector2D vector, int level) {
        Level = level;
        double radius = (4 - Level) * ConfigClass.Config["asteroid"]["smallestRadius"].AsDouble();
        base(center, vector, Radius);
    }

    public void GameTick((int x, int y) maxSize) {
        base.GameTick(maxSize);

        VisualRotation = VisualRotation.AdjustBy(rotationSpeed);
    }
}