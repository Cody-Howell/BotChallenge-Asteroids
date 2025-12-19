using BC_Asteroids.Shared;
using HowlDev.Simulation.Physics.Primitive2D;

namespace BC_Asteroids.Tests;


public class AsteroidObjectTests {
    [Test] // Expects config.json ["smallestRadius"] to be 10.
    public async Task LevelsReturnProperRadii() {
        Asteroid a1 = new Asteroid(new Point2D(), new Vector2D(), 1, 1);
        Asteroid a2 = new Asteroid(new Point2D(), new Vector2D(), 2, 1);
        Asteroid a3 = new Asteroid(new Point2D(), new Vector2D(), 3, 1);

        await Assert.That(a1.Radius).IsEqualTo(30);
        await Assert.That(a2.Radius).IsEqualTo(20);
        await Assert.That(a3.Radius).IsEqualTo(10);
    }

    [Test]
    public async Task VisualRotationUpdatesProperly1() {
        Asteroid a = new Asteroid(new Point2D(200, 200), new Vector2D(90, 1), 1, 5);
        a.VisualRotation = 356;
        a.GameTick((400, 400));

        await Assert.That(a.VisualRotation).IsEqualTo(1);
    }

    [Test]
    public async Task VisualRotationUpdatesProperly2() {
        Asteroid a = new Asteroid(new Point2D(200, 200), new Vector2D(90, 1), 1, -5);

        a.GameTick((400, 400));

        await Assert.That(a.VisualRotation).IsEqualTo(355);
    }

    [Test]
    public async Task AsteroidGameTickWorksAsExpected1() {
        Asteroid a = new Asteroid(new Point2D(200, 200), new Vector2D(90, 1), 1, 3);

        a.GameTick((400, 400));

        await Assert.That(a.VisualRotation).IsEqualTo(3);
        await Assert.That(a.Boundary.Center.X).IsEqualTo(200);
        await Assert.That(a.Boundary.Center.Y).IsEqualTo(201);
    }

    [Test]
    public async Task AsteroidGameTickWorksAsExpected2() {
        Asteroid a = new Asteroid(new Point2D(200, 200), new Vector2D(45, 4.225), 1, -5);
        a.VisualRotation = 3;

        a.GameTick((200, 200));

        await Assert.That(a.VisualRotation).IsEqualTo(358);
        await Assert.That(Math.Round(a.Boundary.Center.X, 2)).IsEqualTo(3);
        await Assert.That(Math.Round(a.Boundary.Center.Y, 2)).IsEqualTo(3);
    }

    [Test]
    public async Task AsteroidGameTickWorksAsExpected3() {
        Asteroid a = new Asteroid(new Point2D(0, 0), new Vector2D(180, 4), 1, 1);

        a.GameTick((400, 400));

        await Assert.That(a.Boundary.Center.X).IsEqualTo(396);
        await Assert.That(a.Boundary.Center.Y).IsEqualTo(0);
    }
}
public class BulletObjectTests {

    [Test]
    public async Task AsteroidGameTickWorksAsExpected1() {
        Bullet a = new Bullet(new Point2D(200, 200), new Vector2D(90, 1), 1);

        a.GameTick((400, 400));

        await Assert.That(a.PlayerId).IsEqualTo(1);
        await Assert.That(a.Boundary.Center.X).IsEqualTo(200);
        await Assert.That(a.Boundary.Center.Y).IsEqualTo(201);
    }

    [Test]
    public async Task AsteroidGameTickWorksAsExpected2() {
        Bullet a = new Bullet(new Point2D(200, 200), new Vector2D(45, 4.225), 1);

        a.GameTick((200, 200));

        await Assert.That(Math.Round(a.Boundary.Center.X, 2)).IsEqualTo(3);
        await Assert.That(Math.Round(a.Boundary.Center.Y, 2)).IsEqualTo(3);
    }
}