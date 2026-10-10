using System.Diagnostics;
using System.Text.RegularExpressions;
using RobsSpacePatrol.CLI.Library;
using RobsSpacePatrol.Library.Game;
using RobsSpacePatrol.Factories;
using RobsSpacePatrol.Library.Factories.Interfaces;
using RobsSpacePatrol.Library.Factories;

namespace RobsSpacePatrol.CLI;

public class RunGame
{
    private bool GameDone = false;
    private string GameOverMessage = string.Empty;

    private GameExecutor getGE_Library()
    {
        var game = GameFactory.getGame();
        var debug = new DebugCommands_Library(game);
        var cl = new CommandLine_Library(game, debug);

        return new GameExecutor(cl, debug, game);
    }

    public void RGS()
    {
        Stopwatch SW = new Stopwatch();
        SW.Start();

        GameExecutor? ge;

        ge = getGE_Library();

        Console.WriteLine(ge.IntroMessage());

        while (!GameDone)
        {
            string MSG = ge.ExecuteCommandString("");

            if (MSG.Trim() != string.Empty)
            {
                Console.WriteLine(MSG);
            }

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("Command : ");
            var CMD = Console.ReadLine();
            if (CMD == null) continue;
            var RESP = ge.ExecuteCommandString(CMD!);

            RESP = RESP.TrimEnd(new char[] { '\n' });

            Console.WriteLine(RESP);
            if (ge.GameOver) GameDone = true;
        }
        TimeSpan TS = SW.Elapsed;

        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}", TS.Hours, TS.Minutes, TS.Seconds);

        Console.WriteLine(GameOverMessage);
        SW.Stop();


        Console.WriteLine("Game Over!");
        Console.WriteLine("Total Time: " + elapsedTime);

        Console.ReadLine();
    }
}