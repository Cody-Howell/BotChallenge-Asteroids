using BC_Asteroids.Shared;
using BC_Asteroids.Shared.Config;
using HowlDev.Simulation.Physics.Primitve2D;

namespace BC_Asteroids.Tests;

public class AsteroidObjectTests {
    [Before(HookType.Class)]
    public static void SetUp() {
        ConfigClass.Initialize("../../../../BC-Asteroids.Shared/Config/config.json");
    }

    [Test] // Expects config.json ["smallestRadius"] to be 10.
    public async Task LevelsReturnProperRadii() {
        Asteroid a1 = new Asteroid(new Vector2D(), 0, new Point2D(), 1);
        Asteroid a2 = new Asteroid(new Vector2D(), 0, new Point2D(), 2);
        Asteroid a3 = new Asteroid(new Vector2D(), 0, new Point2D(), 3);

        await Assert.That(a1.Radius).IsEqualTo(30);
        await Assert.That(a2.Radius).IsEqualTo(20);
        await Assert.That(a3.Radius).IsEqualTo(10);
    }

    [Test]
    public async Task VisualRotationUpdatesProperly1() {
        Asteroid a = new Asteroid(new Vector2D(90, 1), 5, new Point2D(200, 200), 1);
        a.VisualRotation = 356;
        a.GameTick(1000, (400, 400));

        await Assert.That(a.VisualRotation).IsEqualTo(1);
    }

    [Test]
    public async Task VisualRotationUpdatesProperly2() {
        Asteroid a = new Asteroid(new Vector2D(90, 1), -5, new Point2D(200, 200), 1);

        a.GameTick(1000, (400, 400));

        await Assert.That(a.VisualRotation).IsEqualTo(355);
    }

    [Test]
    public async Task AsteroidGameTickWorksAsExpected1() {
        Asteroid a = new Asteroid(new Vector2D(90, 1), 3, new Point2D(200, 200), 1);

        a.GameTick(1000, (400, 400));

        await Assert.That(a.VisualRotation).IsEqualTo(3);
        await Assert.That(a.Center.X).IsEqualTo(200);
        await Assert.That(a.Center.Y).IsEqualTo(201);
    }

    [Test]
    public async Task AsteroidGameTickWorksAsExpected2() {
        Asteroid a = new Asteroid(new Vector2D(45, 4.225), -5, new Point2D(200, 200), 1);
        a.VisualRotation = 3;

        a.GameTick(1000, (200, 200));

        await Assert.That(a.VisualRotation).IsEqualTo(358);
        await Assert.That(Math.Round(a.Center.X, 2)).IsEqualTo(3);
        await Assert.That(Math.Round(a.Center.Y, 2)).IsEqualTo(3);
    }

    [Test]
    public async Task AsteroidGameTickWorksAsExpected3() {
        Asteroid a = new Asteroid(new Vector2D(180, 4), 1, new Point2D(0, 0), 1);

        a.GameTick(250, (400, 400));

        await Assert.That(a.Center.X).IsEqualTo(399);
        await Assert.That(a.Center.Y).IsEqualTo(0);
    }
}
public class BulletObjectTests {
}