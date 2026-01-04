using HowlDev.AI.Core;

namespace BC_Asteroids.Training;

public class FileWriter : IFileWriter {
    public void WriteFile(string path, string value) {
        Console.WriteLine($"Path: {path}, Value: {value}");
        Console.WriteLine("=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
    }
}