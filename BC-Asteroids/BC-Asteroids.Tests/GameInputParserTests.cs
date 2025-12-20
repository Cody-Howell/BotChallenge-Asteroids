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
public class GameInputParserTwoValueTests {
    [Test]
    public async Task InputParserWorksWith2Values1() {
        // LEFT and RIGHT conflict - last in list (RIGHT) is processed first and wins
        List<string> commands = [
            "LEFT",
            "RIGHT"
        ];
        PlayerActions p = GameInputParser.ParseCommands(commands);

        await Assert.That(p.RotationAdjustment).IsEqualTo(-1);
        await Assert.That(p.Pointer).IsNull();
    }

    [Test]
    public async Task InputParserWorksWith2Values2() {
        // RIGHT and LEFT conflict - last in list (LEFT) is processed first and wins
        List<string> commands = [
            "RIGHT",
            "LEFT"
        ];
        PlayerActions p = GameInputParser.ParseCommands(commands);

        await Assert.That(p.RotationAdjustment).IsEqualTo(1);
        await Assert.That(p.Pointer).IsNull();
    }

    [Test]
    public async Task InputParserWorksWith2Values3() {
        // POINTAT and LEFT conflict - last in list (LEFT) is processed first and wins
        List<string> commands = [
            "POINTAT 100 200",
            "LEFT"
        ];
        PlayerActions p = GameInputParser.ParseCommands(commands);

        await Assert.That(p.RotationAdjustment).IsEqualTo(1);
        await Assert.That(p.Pointer).IsNull();
    }

    [Test]
    public async Task InputParserWorksWith2Values4() {
        // LEFT and POINTAT conflict - last in list (POINTAT) is processed first and wins
        List<string> commands = [
            "LEFT",
            "POINTAT 100 200"
        ];
        PlayerActions p = GameInputParser.ParseCommands(commands);

        await Assert.That(p.Pointer).IsNotNull();
        await Assert.That(p.Pointer!.Value.X).IsEqualTo(100);
        await Assert.That(p.Pointer!.Value.Y).IsEqualTo(200);
        await Assert.That(p.RotationAdjustment).IsNull();
    }

    [Test]
    public async Task InputParserWorksWith2Values5() {
        // POINTAT and RIGHT conflict - last in list (RIGHT) is processed first and wins
        List<string> commands = [
            "POINTAT 100 200",
            "RIGHT"
        ];
        PlayerActions p = GameInputParser.ParseCommands(commands);

        await Assert.That(p.RotationAdjustment).IsEqualTo(-1);
        await Assert.That(p.Pointer).IsNull();
    }

    [Test]
    public async Task InputParserWorksWith2Values6() {
        // RIGHT and POINTAT conflict - last in list (POINTAT) is processed first and wins
        List<string> commands = [
            "RIGHT",
            "POINTAT 50 75"
        ];
        PlayerActions p = GameInputParser.ParseCommands(commands);

        await Assert.That(p.Pointer).IsNotNull();
        await Assert.That(p.Pointer!.Value.X).IsEqualTo(50);
        await Assert.That(p.Pointer!.Value.Y).IsEqualTo(75);
        await Assert.That(p.RotationAdjustment).IsNull();
    }

    [Test]
    public async Task InputParserWorksWith2Values7() {
        // Multiple POINTATs - last in list is processed first and wins
        List<string> commands = [
            "POINTAT 100 200",
            "POINTAT 50 75"
        ];
        PlayerActions p = GameInputParser.ParseCommands(commands);

        await Assert.That(p.Pointer).IsNotNull();
        await Assert.That(p.Pointer!.Value.X).IsEqualTo(50);
        await Assert.That(p.Pointer!.Value.Y).IsEqualTo(75);
    }

    [Test]
    public async Task InputParserWorksWith2Values8() {
        // ACCEL and BRAKE conflict - last in list (BRAKE) is processed first and wins
        List<string> commands = [
            "ACCEL 0.5",
            "BRAKE"
        ];
        PlayerActions p = GameInputParser.ParseCommands(commands);

        await Assert.That(p.Brake).IsTrue();
        await Assert.That(p.Acceleration).IsNull();
    }

    [Test]
    public async Task InputParserWorksWith2Values9() {
        // BRAKE and ACCEL conflict - last in list (ACCEL) is processed first and wins
        List<string> commands = [
            "BRAKE",
            "ACCEL 0.8"
        ];
        PlayerActions p = GameInputParser.ParseCommands(commands);

        await Assert.That(p.Acceleration).IsEqualTo(0.8);
        await Assert.That(p.Brake).IsFalse();
    }

    [Test]
    public async Task InputParserWorksWith2Values10() {
        // Multiple ACCELs - last in list is processed first and wins
        List<string> commands = [
            "ACCEL 0.5",
            "ACCEL 0.8"
        ];
        PlayerActions p = GameInputParser.ParseCommands(commands);

        await Assert.That(p.Acceleration).IsEqualTo(0.8);
        await Assert.That(p.Brake).IsFalse();
    }

    [Test]
    public async Task InputParserWorksWith2Values11() {
        // FIRE doesn't conflict with LEFT - both should be set
        List<string> commands = [
            "LEFT",
            "FIRE"
        ];
        PlayerActions p = GameInputParser.ParseCommands(commands);

        await Assert.That(p.RotationAdjustment).IsEqualTo(1);
        await Assert.That(p.Fire).IsTrue();
    }

    [Test]
    public async Task InputParserWorksWith2Values12() {
        // FIRE doesn't conflict with RIGHT - both should be set
        List<string> commands = [
            "FIRE",
            "RIGHT"
        ];
        PlayerActions p = GameInputParser.ParseCommands(commands);

        await Assert.That(p.RotationAdjustment).IsEqualTo(-1);
        await Assert.That(p.Fire).IsTrue();
    }

    [Test]
    public async Task InputParserWorksWith2Values13() {
        // FIRE doesn't conflict with POINTAT - both should be set
        List<string> commands = [
            "FIRE",
            "POINTAT 100 200"
        ];
        PlayerActions p = GameInputParser.ParseCommands(commands);

        await Assert.That(p.Pointer).IsNotNull();
        await Assert.That(p.Pointer!.Value.X).IsEqualTo(100);
        await Assert.That(p.Pointer!.Value.Y).IsEqualTo(200);
        await Assert.That(p.Fire).IsTrue();
    }

    [Test]
    public async Task InputParserWorksWith2Values14() {
        // FIRE doesn't conflict with ACCEL - both should be set
        List<string> commands = [
            "ACCEL 0.5",
            "FIRE"
        ];
        PlayerActions p = GameInputParser.ParseCommands(commands);

        await Assert.That(p.Acceleration).IsEqualTo(0.5);
        await Assert.That(p.Fire).IsTrue();
    }

    [Test]
    public async Task InputParserWorksWith2Values15() {
        // FIRE doesn't conflict with BRAKE - both should be set
        List<string> commands = [
            "FIRE",
            "BRAKE"
        ];
        PlayerActions p = GameInputParser.ParseCommands(commands);

        await Assert.That(p.Brake).IsTrue();
        await Assert.That(p.Fire).IsTrue();
    }

    [Test]
    public async Task InputParserWorksWith2Values16() {
        // LEFT and ACCEL don't conflict - both should be set
        List<string> commands = [
            "LEFT",
            "ACCEL 0.5"
        ];
        PlayerActions p = GameInputParser.ParseCommands(commands);

        await Assert.That(p.RotationAdjustment).IsEqualTo(1);
        await Assert.That(p.Acceleration).IsEqualTo(0.5);
    }

    [Test]
    public async Task InputParserWorksWith2Values17() {
        // RIGHT and BRAKE don't conflict - both should be set
        List<string> commands = [
            "RIGHT",
            "BRAKE"
        ];
        PlayerActions p = GameInputParser.ParseCommands(commands);

        await Assert.That(p.RotationAdjustment).IsEqualTo(-1);
        await Assert.That(p.Brake).IsTrue();
    }

    [Test]
    public async Task InputParserWorksWith2Values18() {
        // POINTAT and ACCEL don't conflict - both should be set
        List<string> commands = [
            "POINTAT 100 200",
            "ACCEL -0.5"
        ];
        PlayerActions p = GameInputParser.ParseCommands(commands);

        await Assert.That(p.Pointer).IsNotNull();
        await Assert.That(p.Pointer!.Value.X).IsEqualTo(100);
        await Assert.That(p.Pointer!.Value.Y).IsEqualTo(200);
        await Assert.That(p.Acceleration).IsEqualTo(-0.5);
    }

    [Test]
    public async Task InputParserWorksWith2Values19() {
        // POINTAT and BRAKE don't conflict - both should be set
        List<string> commands = [
            "BRAKE",
            "POINTAT 50 75"
        ];
        PlayerActions p = GameInputParser.ParseCommands(commands);

        await Assert.That(p.Pointer).IsNotNull();
        await Assert.That(p.Pointer!.Value.X).IsEqualTo(50);
        await Assert.That(p.Pointer!.Value.Y).IsEqualTo(75);
        await Assert.That(p.Brake).IsTrue();
    }
}