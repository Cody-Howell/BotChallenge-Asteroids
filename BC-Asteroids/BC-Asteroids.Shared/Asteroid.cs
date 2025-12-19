using BC_Asteroids.Shared.Config;
using HowlDev.Simulation.Physics.Primitive2D;

namespace BC_Asteroids.Shared;

public class Asteroid : GameObject {
    private double rotationSpeed = 0;

    public double Radius => Boundary.Radius;
    public Rotation2D VisualRotation { get; set; } = new Rotation2D();
    public int Level { get; private set; }

    public Asteroid(Point2D center, Vector2D vector, int level, int rotationSpeed)
        : base(center, vector, (4 - level) * ConfigClass.Config["asteroid"]["smallestRadius"].AsDouble()) {
        Level = level;
        this.rotationSpeed = rotationSpeed;
    }

    public new void GameTick((int x, int y) maxSize) {
        base.GameTick(maxSize);

        VisualRotation = VisualRotation.AdjustBy(rotationSpeed);
    }
}