using BC_Asteroids.Shared.Config;
using HowlDev.Simulation.Physics.Primitive2D;

namespace BC_Asteroids.Shared;

public class Bullet(Point2D center, Vector2D vector, int id) : GameObject(center, vector, InnerRadius) {
    public bool ShouldBeDestroyed { get => countdown < 0; }
    public int PlayerId { get; private set; } = id;
    private int countdown = ConfigClass.Config["bullet"]["duration"].AsInt();
    private static double InnerRadius { get => ConfigClass.Config["bullet"]["radius"].AsDouble(); }

    public new void GameTick((int x, int y) maxSize) {
        base.GameTick(maxSize);
        countdown--;
    }
}