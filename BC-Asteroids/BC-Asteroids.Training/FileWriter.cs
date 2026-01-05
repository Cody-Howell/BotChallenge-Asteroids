using HowlDev.AI.Core;

namespace BC_Asteroids.Training;

public class FileWriter : IFileWriter {
    public void WriteFile(string path, string value) {
        Console.WriteLine($"Writing file: {path}");
        Directory.CreateDirectory("./output");
        File.WriteAllText("./output/" + path + ".txt", value);
    }
}