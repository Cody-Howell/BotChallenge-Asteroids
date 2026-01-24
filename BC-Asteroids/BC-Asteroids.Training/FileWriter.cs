using HowlDev.AI.Core;
using HowlDev.AI.Core.Classes;

namespace BC_Asteroids.Training;

public class FileWriter : IResultReader {
    public void ReadResult(GeneticAlgorithmResult result) {
        Console.WriteLine($"New generation: {result.Generation}");
        if (result.Generation % 5 == 0) {
            Directory.CreateDirectory("./output");
            File.WriteAllText($"./output/Gen-{result.Generation}-Results.txt", string.Join('\n', result.Results));
            File.WriteAllText($"./output/Gen-{result.Generation}-Best-Network.txt", result.Networks[0].network);
        }
    }
}