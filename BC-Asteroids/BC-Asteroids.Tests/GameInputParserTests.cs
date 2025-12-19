using BC_Asteroids.Shared;

namespace BC_Asteroids.Tests;


public class GameInputParserOneValueTests {
    [Test]
    public async Task InputParserWorksWith1Value1() {
        List<string> commands = [
            "LEFT"
        ];
        PlayerActions p = GameInputParser.ParseCommands(commands);

        await Assert.That(p.RotationAdjustment).IsEqualTo(1);
        await Assert.That(p.Pointer).IsNull();
        await Assert.That(p.Acceleration).IsNull();
    }

    [Test]
    public async Task InputParserWorksWith1Value2() {
        List<string> commands = [
            "RIGHT"
        ];
        PlayerActions p = GameInputParser.ParseCommands(commands);

        await Assert.That(p.RotationAdjustment).IsEqualTo(-1);
        await Assert.That(p.Pointer).IsNull();
        await Assert.That(p.Acceleration).IsNull();
    }

    [Test]
    public async Task InputParserWorksWith1Value3() {
        List<string> commands = [
            "POINTAT 100 200"
        ];
        PlayerActions p = GameInputParser.ParseCommands(commands);

        await Assert.That(p.Pointer).IsNotNull();
        await Assert.That(p.Pointer!.Value.X).IsEqualTo(100);
        await Assert.That(p.Pointer!.Value.Y).IsEqualTo(200);
        await Assert.That(p.RotationAdjustment).IsNull();
        await Assert.That(p.Acceleration).IsNull();
    }

    [Test]
    public async Task InputParserWorksWith1Value4() {
        List<string> commands = [
            "FIRE"
        ];
        PlayerActions p = GameInputParser.ParseCommands(commands);

        await Assert.That(p.Fire).IsTrue();
        await Assert.That(p.RotationAdjustment).IsNull();
        await Assert.That(p.Pointer).IsNull();
        await Assert.That(p.Acceleration).IsNull();
    }

    [Test]
    public async Task InputParserWorksWith1Value5() {
        List<string> commands = [
            "ACCEL 0.5"
        ];
        PlayerActions p = GameInputParser.ParseCommands(commands);

        await Assert.That(p.Acceleration).IsEqualTo(0.5);
        await Assert.That(p.Brake).IsFalse();
        await Assert.That(p.RotationAdjustment).IsNull();
        await Assert.That(p.Pointer).IsNull();
    }

    [Test]
    public async Task InputParserWorksWith1Value6() {
        List<string> commands = [
            "ACCEL -0.5"
        ];
        PlayerActions p = GameInputParser.ParseCommands(commands);

        await Assert.That(p.Acceleration).IsEqualTo(-0.5);
        await Assert.That(p.Brake).IsFalse();
        await Assert.That(p.RotationAdjustment).IsNull();
        await Assert.That(p.Pointer).IsNull();
    }

    [Test]
    public async Task InputParserWorksWith1Value7() {
        List<string> commands = [
            "BRAKE"
        ];
        PlayerActions p = GameInputParser.ParseCommands(commands);

        await Assert.That(p.Brake).IsTrue();
        await Assert.That(p.Acceleration).IsNull();
        await Assert.That(p.RotationAdjustment).IsNull();
        await Assert.That(p.Pointer).IsNull();
    }

    [Test]
    public async Task InputParserThrowsExceptionForInvalidCommand() {
        List<string> commands = [
            "INVALID"
        ];

        Assert.Throws<InvalidOperationException>(() => GameInputParser.ParseCommands(commands));
    }

    [Test]
    public async Task InputParserThrowsExceptionForAccelTooHigh() {
        List<string> commands = [
            "ACCEL 1.5"
        ];

        Assert.Throws<ArgumentOutOfRangeException>(() => GameInputParser.ParseCommands(commands));
    }

    [Test]
    public async Task InputParserThrowsExceptionForAccelTooLow() {
        List<string> commands = [
            "ACCEL -1.5"
        ];

        Assert.Throws<ArgumentOutOfRangeException>(() => GameInputParser.ParseCommands(commands));
    }
}