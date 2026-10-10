using RobsSpacePatrol.Library.Factories;
using RobsSpacePatrol.Library.Factories.Interfaces;

namespace RobsSpacePatrol.CLI.Library.Tests;

public static class GameBuilder
{
    public static CommandLine_Library getCommandLine()
    {
        var game = GameFactory.getGame();
        var debug = new DebugCommands_Library(game);
        return new CommandLine_Library(game, debug);
    }
    //private static Game getGame()
    //{
    //    IRandomFactory randomFactory;
    //    randomFactory = new RandomFactory();

    //    var galaxyFactory = new GalaxyFactory();
    //    var currentQuadrantFactory = new CurrentQuadrantFactory();
    //    var quadrantFactory = new QuadrantFactory();

    //    var galaxy = galaxyFactory.createGalaxy(randomFactory, currentQuadrantFactory, quadrantFactory);
    //    var game = new Game(galaxy, randomFactory, currentQuadrantFactory);
    //    game.SetSkipEnemyMove(true);

    //    return game;
    //}
}
