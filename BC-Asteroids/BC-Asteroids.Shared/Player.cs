using BC_Asteroids.Shared.Config;
using HowlDev.Simulation.Physics.Primitive2D;

namespace BC_Asteroids.Shared;

public class Player(Point2D center, Vector2D vector, int id) : GameObject(center, vector, Radius) {
    public int Id { get; } = id;
    private static int FireSpeed { get => ConfigClass.Config["player"]["fireSpeed"].AsInt(); }
    private static double RotationSpeed { get => ConfigClass.Config["player"]["rotationSpeed"].AsDouble(); }
    private static double MovementSpeed { get => ConfigClass.Config["player"]["movementSpeed"].AsDouble(); }
    private static double TopSpeed { get => ConfigClass.Config["player"]["topSpeed"].AsDouble(); }
    private static double Radius { get => ConfigClass.Config["player"]["radius"].AsDouble(); }
    private const double BrakeValue = 1.2;
    public Rotation2D VisualRotation { get; set; } = new Rotation2D();
    public int Health { get; set; } = 100;
    public int Score { get; set; } = 0;

    #region Intangibility
    private int intangibility = 0;
    private int IntangibleTicks {
        get => intangibility;
        set {
            if (value < 0) {
                intangibility = value;
            } else {
                intangibility = 0;
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

    public void GameTick(PlayerActions? action, (int x, int y) maxSize, Action<Bullet> fire) {
        if (action is not null)
            ProcessPlayerActions(action, fire);

        GameTick(maxSize);

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

    public string ToTextFormat() {
        return $"{Id} {Boundary.Center.X} {Boundary.Center.Y} {VisualRotation.RotationAngle} {Velocity.Velocity} {Health} {TimeToFire} {IsIntangible} {Score}";
    }

    // public static Player FromTextFormat(string input) {
    //     return new(new(), new(), 1);
    // }

    private void ProcessPlayerActions(PlayerActions action, Action<Bullet> fire) {
        if (action.Fire) {
            Bullet? b = FireBullet();
            if (b is not null) {
                fire(b);
            }
        }

        if (action.Pointer is not null) {
            VisualRotation = VisualRotation.MoveTo(Rotation2D.FromCoordinates((Point2D)action.Pointer), RotationSpeed);
        }
        if (action.RotationAdjustment is not null) {
            VisualRotation = VisualRotation.AdjustBy((double)action.RotationAdjustment * RotationSpeed);
        }

        if (action.Brake) {
            Velocity /= BrakeValue;
        }
        if (action.Acceleration is not null) {
            Velocity += Vector2D.FromCoordinates(VisualRotation * (MovementSpeed * (double)action.Acceleration));
            Velocity = Velocity.WithVelocity(Math.Min(TopSpeed, Velocity.Velocity));
        }
    }
}