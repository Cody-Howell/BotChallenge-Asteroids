using HowlDev.Simulation.Physics.Primitive2D;

namespace BC_Asteroids.Shared;

/// <summary>
/// The server takes in certain string types. You can write your own or use these helper 
/// methods to ensure you don't make invalid requests. <br/>
/// This class lists available moves to your bot. Return these as a list to the server. 
/// <br/>
/// You can refer duplicate (Left(), Left()) or contradictory (Left(), Right()) 
/// commands, but the server will only parse later commands and disregard previous ones. 
/// Specifically (written with calls that overwrite each other, separated by hyphens): 
/// <list type="bullet">
///     <item><c>PointAt - Left - Right</c></item>
///     <item><c>Accelerate - Brake</c></item>
///     <item><c>Fire</c></item>
/// </list>
/// Again, only the most recent commands (latest in the returned List) will be executed. 
/// </summary>
public static class AvailableMoves {
    /// <summary>
    /// Points your ship as well as it can to a given point. If it's within range to target 
    /// it exactly, it will do so; if not, it will do as much as it can per tick. 
    /// </summary>
    /// <param name="point">Point to target</param>
    /// <returns>Server-ready interpretation</returns>
    public static string PointAt(Point2D point) {
        return $"POINTAT {point.X} {point.Y}";
    }
    /// <summary>
    /// Turns Left as much as possible in one tick. Primarily used by the Human interface.
    /// </summary>
    /// <returns>Server-ready interpretation</returns>
    public static string Left() {
        return "LEFT";
    }
    /// <summary>
    /// Turns Right as much as possible in one tick. Primarily used by the Human interface.
    /// </summary>
    /// <returns>Server-ready interpretation</returns>
    public static string Right() {
        return "RIGHT";
    }
    /// <summary>
    /// If <see cref="Player.TimeToFire"/> is equal to 0, then you can execute a "FIRE" command
    /// (if not, the server will simply discard this command). 
    /// This applies your current vector added to the default bullet vector, and fires from the 
    /// point of your ship. 
    /// </summary>
    /// <returns>Server-ready interpretation</returns>
    public static string Fire() {
        return "FIRE";
    }
    /// <summary>
    /// Takes in a value between -1 and 1, throwing an exception otherwise. This accelerates
    /// forwards (1) or backwards (-1) relative to your <see cref="Player.VisualRotation"/>. 
    /// </summary>
    /// <returns>Server-ready interpretation</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static string Accelerate(double a) {
        if (a < -1 || a > 1)
            throw new ArgumentOutOfRangeException("Can only accelerate between -1 and 1 (backwards and forwards respectively)");
        if (a < -1 || a > 1)
            throw new ArgumentOutOfRangeException("Can only accelerate between -1 and 1 (backwards and forwards respectively)");

        return $"ACCEL {Math.Round(a, 2)}";
    }
    /// <summary>
    /// Slows your ship in space, regardless of which way you are facing. 
    /// </summary>
    /// <returns>Server-ready interpretation</returns>
    public static string Brake() {
        return "BRAKE";
    }
}