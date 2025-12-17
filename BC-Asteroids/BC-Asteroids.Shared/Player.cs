using BC_Asteroids.Shared.Config;
using HowlDev.Simulation.Physics.Primitve2D;

namespace BC_Asteroids.Shared;

public class Player : IGameObject {
    public int Id { get; set; } = -1;
    public Vector2D Velocity { get; set; } = new Vector2D();
    public Point2D Center { get; set; } = new Point2D();
    public double Radius { get => ConfigClass.Config["player"]["radius"].AsDouble(); }
    public Rotation2D VisualRotation { get; set; } = new Rotation2D();

    #region Intangibility
    private int intagibility = 0;
    private int intangibleTicks {
        get => intagibility;
        set {
            if (value < 0) {
                intagibility = value;
            } else {
                intagibility = 0;
            }
        }
    }
    public bool IsIntangible { get => intangibleTicks < 0; }
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

    public bool IsCollided(IGameObject obj) {
        return Center.GetDistance(obj.Center) <= (obj.Radius + Radius);
    }

    public void GameTick(int ms, (int x, int y) maxSize) {
        Center += new Vector2D(Velocity.Rotation.RotationAngle, Velocity.Velocity * 1000 / ms);
        if (Center.X < 0) Center.X += maxSize.x;
        else if (Center.X > maxSize.x) Center.X += maxSize.x;

        if (Center.Y < 0) Center.Y += maxSize.y;
        else if (Center.Y > maxSize.y) Center.Y += maxSize.y;

        TimeToFire += ms;
        intangibleTicks += ms;
    }

    public Bullet? FireBullet() {
        if (!CanFire) return null;

        timeToFire = -ConfigClass.Config["player"]["fireSpeed"].AsInt();

        Vector2D bulletSpeed = new Vector2D(VisualRotation.RotationAngle, ConfigClass.Config["bullet"]["radius"].AsDouble());
        Point2D summedVectors = (new Point2D() + bulletSpeed) + Velocity;
        bulletSpeed.AssignToCoordinates(summedVectors);

        return new Bullet {
            Velocity = bulletSpeed,
            Center = Center + new Vector2D(VisualRotation.RotationAngle, Radius)
        };
    }
}
