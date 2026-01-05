using BC_Asteroids.Shared;
using BC_Asteroids.Shared.Config;
using BC_Asteroids.Training;
using HowlDev.AI.Training.Genetic;
using HowlDev.AI.Training.Saving;

ConfigClass.Initialize("./config.json");

GeneticAlgorithm<AsteroidGame> algo = new(new(new() { CountPerGroup = 5, NumberOfGroups = 100, NumOfGenerations = 100, NumberOfTicks = 3000 },
    new(23, [5], 3),
    new()) {
    SavingStrategy = new() { SavingScheme = NetworkSavingScheme.Best }
},
new FileWriter());

await algo.StartTraining();
