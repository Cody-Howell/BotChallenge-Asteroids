using HowlDev.Simulation.Physics.Primitive2D;

namespace BC_Asteroids.Shared;

public static class GameInputParser {
    public static PlayerActions ParseCommands(IEnumerable<string> input) {
        PlayerActions a = new();
        foreach (string i in input.Reverse()) {
            string val = i.Trim();
            string[] parameters = val.Split(' ');
            switch (parameters[0]) {
                case "POINTAT":
                    if (a.Pointer is null && a.RotationAdjustment is null) {
                        double x = Convert.ToDouble(parameters[1]);
                        double y = Convert.ToDouble(parameters[2]);
                        a.Pointer ??= new Point2D(x, y);
                    }
                    break;
                case "LEFT":
                    if (a.Pointer is null && a.RotationAdjustment is null) {
                        a.RotationAdjustment = 1;
                    }
                    break;
                case "RIGHT":
                    if (a.Pointer is null && a.RotationAdjustment is null) {
                        a.RotationAdjustment = -1;
                    }
                    break;
                case "FIRE":
                    a.Fire = true;
                    break;
                case "ACCEL":
                    if (a.Acceleration is null && !a.Brake) {
                        double amount = Convert.ToDouble(parameters[1]);
                        if (amount < -1 || amount > 1)
                            throw new ArgumentOutOfRangeException("Can only accelerate between -1 and 1 (backwards and forwards respectively)");
                        a.Acceleration = Math.Round(amount, 2);
                    }
                    break;
                case "BRAKE":
                    if (a.Acceleration is null) {
                        a.Brake = true;
                    }
                    break;
                default:
                    throw new InvalidOperationException($"Server does not recognize command of name: {parameters[0]}");
            }
        }
        return a;
    }
}

public class PlayerActions {
    public Point2D? Pointer { get; set; } = null;
    public double? RotationAdjustment { get; set; } = null;
    public double? Acceleration { get; set; } = null;
    public bool Fire { get; set; } = false;
    public bool Brake { get; set; } = false;
}
