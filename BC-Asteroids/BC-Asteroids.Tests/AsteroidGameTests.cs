using BC_Asteroids.Shared;

namespace BC_Asteroids.Tests;


public class AsteroidGameRegisterTests {
    [Test] 
    public async Task AsteroidGameReturnsOneIncrementingID() {
        AsteroidGame game = new ();
        int id = game.Register();

        await Assert.That(id).IsEqualTo(1);
        await Assert.That(game.Players.Count).IsEqualTo(1);
        await Assert.That(game.Players[1].Boundary.Center.X).IsEqualTo(350);
    }

    [Test] 
    public async Task AsteroidGameReturnsTwoIncrementingIDs() {
        AsteroidGame game = new ();
        int id1 = game.Register();
        int id2 = game.Register();

        await Assert.That(id1).IsEqualTo(1);
        await Assert.That(id2).IsEqualTo(2);
        await Assert.That(game.Players.Count).IsEqualTo(2);
    }
}

public class AsteroidGameMovementTests {
    [Test] 
    public async Task AsteroidGameCanSendUpdatesForPlayers() {
        AsteroidGame game = new ();
        int id = game.Register();
        List<string> commands = [
          "LEFT"  
        ];
        game.SendUpdates(1, commands);
        game.GameTick();
    }
}