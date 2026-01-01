using HowlDev.Simulation.Physics.Primitive2D;

namespace BC_Asteroids.Shared;

public class GameObject {
    public Vector2D Velocity { get; protected set; }
    public Circle2D Boundary { get; protected set; }
    public virtual GameObjectTypes Type { get; protected set; }

    public GameObject(Point2D center, Vector2D vector, double radius) {
        Boundary = new Circle2D(center, radius);
        Velocity = vector;
    }

    public bool IsCollided(GameObject obj) {
        return Boundary.IsOverlapping(obj.Boundary);
    }

    protected void GameTick((int x, int y) maxSize) {
        Point2D newCenter = Boundary.Center + Velocity;
        double x = newCenter.X;
        double y = newCenter.Y;
        if (newCenter.X < 0) x += maxSize.x;
        else if (newCenter.X > maxSize.x) x -= maxSize.x;

        if (newCenter.Y < 0) y += maxSize.y;
        else if (newCenter.Y > maxSize.y) y -= maxSize.y;

        Boundary = Boundary.WithNewCenter(new Point2D(x, y));
    }
}