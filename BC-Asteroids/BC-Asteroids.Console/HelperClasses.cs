using BC_Asteroids.Shared;
using HowlDev.Simulation.Physics.Primitive2D;

namespace BC_Asteroids.Console; 

public static class HelperClass {

    public static List<string> PrepareAction(List<double> outputs) {
        if (outputs.Count != 3) throw new Exception("Output not of expected length: 3");

        List<string> moves = [];
        moves.Add($"TURN {Clamp(outputs[0], -1, 1)}");
        moves.Add($"ACCEL {Clamp(outputs[1], -1, 1)}");
        if (Clamp(outputs[2], 0, 1) >= 0.5) {
            moves.Add("FIRE");
        }
        return moves;
    }

    public static double[] GetRepresentation(int id, List<Player> Players, List<Asteroid> Asteroids) {
        Player p = Players.First(a => a.Id == id);
        double[] neurons = new double[23];
        neurons[0] = p.VisualRotation.DistanceTo(p.Velocity.Rotation) / 180;
        neurons[1] = p.Velocity.Velocity / 100;

        (double distance, Asteroid asteroid)[] closeAsteroids = [.. Asteroids.Select(a => (p.Boundary.Center.GetDistance(a.Boundary.Center), a)).Where(a => a.Item1 <= 300)];
        (double angleDistance, double distance, double threat)[] gameObjectLocations =
            [.. closeAsteroids.Select((a => (
            p.VisualRotation.DistanceTo(Rotation2D.FromCoordinates(p.Boundary.Center, a.asteroid.Boundary.Center)),
            a.distance,
            ThreatLevel(p, a.asteroid)
        )))
        .OrderBy(a => a.Item1)];

        (double minAngle, double maxAngle)[] angleBins = [
            (-2, 2),
        (-20, -2),
        (2, 20),
        (-90, -20),
        (20, 90),
        (-180, -90),
        (90, 180),
    ];

        int neuronIndex = 2;
        foreach (var (minAngle, maxAngle) in angleBins) {
            // Find all asteroids in this angle bin
            var asteroidsInBin = gameObjectLocations
                .Where(a => a.angleDistance >= minAngle && a.angleDistance < maxAngle)
                .OrderBy(a => a.distance)
                .ToList();

            if (asteroidsInBin.Count > 0) {
                // Get the closest asteroid in this bin
                var (angleDistance, distance, threat) = asteroidsInBin[0];
                neurons[neuronIndex] = distance / 300.0;      // Normalized distance
                neurons[neuronIndex + 1] = threat / 10.0;     // Normalized threat
                neurons[neuronIndex + 2] = 1.0;                        // Bin occupied flag
            } else {
                neurons[neuronIndex] = 1.0;      // Max distance (no asteroid)
                neurons[neuronIndex + 1] = 0.0;  // No threat
                neurons[neuronIndex + 2] = 0.0;  // Bin empty flag
            }

            neuronIndex += 3;
            if (neuronIndex >= neurons.Length) break;
        }

        return neurons;
    }

    private static double Clamp(double input, double start, double end) {
        if (input > end) return end;
        if (input < start) return start;
        return input;
    }

    private static double ThreatLevel(Player p, GameObject o) {
        double threat = 0.0;
        double angleDistance = o.Velocity.Rotation.DistanceTo(Rotation2D.FromCoordinates(o.Boundary.Center, p.Boundary.Center));
        if (angleDistance < 10) {
            threat = 10 / angleDistance;
            threat *= o.Velocity.Velocity / 10;
        }
        return threat;
    }
}