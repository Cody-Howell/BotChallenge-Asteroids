using BC_Asteroids.Shared;
using BC_Asteroids.Shared.Config;
using BC_Asteroids.Training;
using HowlDev.AI.Training.Genetic;

ConfigClass.Initialize("./config.json");

GeneticAlgorithm<AsteroidGame> algo = new(new(new() { CountPerGroup = 5, NumberOfGroups = 5, NumOfGenerations = 5, NumberOfTicks = 3000 },
    new(5, [5], 3),
    new()),
new FileWriter());

await algo.StartTraining();
