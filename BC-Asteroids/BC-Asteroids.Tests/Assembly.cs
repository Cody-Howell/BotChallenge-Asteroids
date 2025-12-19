using BC_Asteroids.Shared.Config;

namespace BC_Asteroids.Tests;

public class Assembly {
    [Before(TestSession)]
    public static void SetUp() {
        ConfigClass.Initialize("../../../../BC-Asteroids.Shared/Config/config.json");
    }
}