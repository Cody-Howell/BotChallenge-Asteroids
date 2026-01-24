using BC_Asteroids.Shared;
using BC_Asteroids.Shared.Config;
using BC_Asteroids.Training;
using HowlDev.AI.Structures.NeuralNetwork.Options;
using HowlDev.AI.Training.Genetic;
using HowlDev.AI.Training.Saving;

ConfigClass.Initialize("./config.json");

GeneticAlgorithm<AsteroidGame> algo = new(new(new() { CountPerGroup = 5, NumberOfGroups = 100, NumOfGenerations = 5000, NumberOfTicks = 3000 },
    new(23, [5], 3),
    new()) {
    SavingStrategy = NetworkSavingScheme.Best,     
    FunctionKind = ActivationFunctionKind.Tanh
}, 
new FileWriter());

algo.StartTraining();
