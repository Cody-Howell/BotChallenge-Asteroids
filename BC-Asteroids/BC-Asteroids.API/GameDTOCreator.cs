using BC_Asteroids.Shared;

namespace BC_Asteroids.API; 

public static class GameDTOCreator {

    public static AsteroidReturnDTO GetDTOForGame(AsteroidGame game) {
        AsteroidReturnDTO dto = new();
        foreach (KeyValuePair<int, Player> a in game.Players) {
            Player p = a.Value;
            dto.Players.Add($"{a.Key} {p.Boundary.Center.X} {p.Boundary.Center.Y} {p.VisualRotation.RotationAngle} {p.Velocity.Velocity} {p.Health} {p.TimeToFire} {p.IsIntangible} {p.Score}");
        }
        foreach (Bullet b in game.Bullets) {
            dto.Bullets.Add($"{b.PlayerId} {b.Boundary.Center.X} {b.Boundary.Center.Y} {b.Velocity.Rotation.RotationAngle} {b.Velocity.Velocity} {b.Countdown}");
        }
        foreach (Asteroid a in game.Asteroids) {
            dto.Asteroids.Add($"{a.Boundary.Center.X} {a.Boundary.Center.Y} {a.Velocity.Rotation.RotationAngle} {a.Velocity.Velocity} {a.Level}");
        }
        return dto;
    }
}