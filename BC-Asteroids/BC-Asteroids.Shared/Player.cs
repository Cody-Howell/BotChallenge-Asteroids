using BC_Asteroids.Shared.Config;
using HowlDev.Simulation.Physics.Primitive2D;

namespace BC_Asteroids.Shared;

public class Player : GameObject {
    public int Id { get; }
    private static int FireSpeed { get => ConfigClass.Config["player"]["fireSpeed"].AsInt(); }
    private static double RotationSpeed { get => ConfigClass.Config["player"]["rotationSpeed"].AsDouble(); }
    private static double MovementSpeed { get => ConfigClass.Config["player"]["movementSpeed"].AsDouble(); }
    private static double TopSpeed { get => ConfigClass.Config["player"]["topSpeed"].AsDouble(); }
    private static double Radius { get => ConfigClass.Config["player"]["radius"].AsDouble(); }
    private const double BrakeValue = 1.2;
    public Rotation2D VisualRotation { get; set; } = new Rotation2D();
    public int Health {
        get; set {
            if (value < 0) field = 0;
            else field = value;
        }
    } = 100;
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

    public Player(Point2D center, Vector2D vector, int id) : base(center, vector, Radius) {
        Id = id;
    }

    public Player(Point2D center, Vector2D vector, int id, double radius) : base(center, vector, radius) {
        Id = id;
    }

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

    public void CalculateDamage(int damage) {
        Health -= damage;
        IntangibleTicks = -60;
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
        return $"{Id} {Boundary.Center.X} {Boundary.Center.Y} {VisualRotation.RotationAngle} {Velocity.Velocity} {Health} {TimeToFire} {IsIntangible} {Score} {Boundary.Radius}";
    }

    public static Player FromTextFormat(string input) {
        string[] items = input.Split(' ');
        Point2D center = new(Convert.ToDouble(items[1]), Convert.ToDouble(items[2]));
        Vector2D vec = new(Convert.ToDouble(items[3]), Convert.ToDouble(items[4]));
        Player p = new(center, vec, Convert.ToInt32(items[0]), Convert.ToDouble(items[9])) {
            Health = Convert.ToInt32(items[5]), 
            TimeToFire = Convert.ToInt32(items[6]), 
            intangibility = Convert.ToBoolean(items[7]) ? 1 : 0, 
            Score = Convert.ToInt32(items[8])
        };

        return p;
    }

    private void ProcessPlayerActions(PlayerActions action, Action<Bullet> fire) {
        if (Health <= 0) return;

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