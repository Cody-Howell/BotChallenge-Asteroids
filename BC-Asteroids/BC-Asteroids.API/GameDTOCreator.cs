using BC_Asteroids.Shared;

namespace BC_Asteroids.API; 

public static class GameDTOCreator {

    public static AsteroidReturnDTO GetDTOForGame(AsteroidGame game) {
        AsteroidReturnDTO dto = new();
        foreach (KeyValuePair<int, Player> a in game.Players) {
            Player p = a.Value;
            dto.Players.Add(p.ToTextFormat());
        }
        foreach (Bullet b in game.Bullets) {
            dto.Bullets.Add(b.ToTextFormat());
        }
        foreach (Asteroid a in game.Asteroids) {
            dto.Asteroids.Add(a.ToTextFormat());
        }
        return dto;
    }
}