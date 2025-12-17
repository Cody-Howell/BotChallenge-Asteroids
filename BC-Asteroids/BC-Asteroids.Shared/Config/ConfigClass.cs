using HowlDev.IO.Text.ConfigFile;

namespace BC_Asteroids.Shared.Config;

public static class ConfigClass {
    private static TextConfigFile? config;
    public static bool _isInitialized = false;

    public static void Initialize(string filePath) {
        if (_isInitialized)
            throw new InvalidOperationException("ConfigStore is already initialized.");

        config = new TextConfigFile(filePath);
        _isInitialized = true;
    }

    public static TextConfigFile Config {
        get {
            if (!_isInitialized || config == null)
                throw new InvalidOperationException("ConfigStore is not initialized.");
            return config;
        }
    }
}