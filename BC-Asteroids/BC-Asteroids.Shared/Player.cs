using BC_Asteroids.Shared.Config;
using HowlDev.Simulation.Physics.Primitive2D;

namespace BC_Asteroids.Shared;

public class Player(Point2D center, Vector2D vector, int id) : GameObject(center, vector, Radius) {
    public int Id { get; set; } = id;
    private static int FireSpeed { get => ConfigClass.Config["player"]["fireSpeed"].AsInt(); }
    private static double RotationSpeed { get => ConfigClass.Config["player"]["rotationSpeed"].AsDouble(); }
    private static double Radius { get => ConfigClass.Config["player"]["radius"].AsDouble(); }
    public Rotation2D VisualRotation { get; set; } = new Rotation2D();

    #region Intangibility
    private int intagibility = 0;
    private int IntangibleTicks {
        get => intagibility;
        set {
            if (value < 0) {
                intagibility = value;
            } else {
                intagibility = 0;
            }
        }
    }
    public bool IsIntangible { get => IntangibleTicks < 0; }
    #endregion
    #region TimeToFire
    private int timeToFire = 0;

    public int TimeToFire {
        get => timeToFire;
        set {
            if (value < 0) {
                timeToFire = value;
            } else {
                timeToFire = 0;
            }
        }
    }

    public bool CanFire { get => timeToFire == 0; }
    #endregion

    public new void GameTick((int x, int y) maxSize) {
        base.GameTick(maxSize);

        TimeToFire++;
        IntangibleTicks++;
    }

    public Bullet? FireBullet() {
        if (!CanFire) return null;

        timeToFire = -FireSpeed;

        Vector2D bulletSpeed = new Vector2D(VisualRotation.RotationAngle, Radius);
        Point2D summedVectors = new Point2D() + bulletSpeed + Velocity;
        bulletSpeed = Vector2D.FromCoordinates(summedVectors);

        return new Bullet(Boundary.Center, bulletSpeed, Id);
    }
}