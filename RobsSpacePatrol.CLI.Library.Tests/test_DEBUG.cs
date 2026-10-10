
using RobsSpacePatrol.CLI.Library;

namespace RobsSpacePatrol.CLI.Library.Tests;

public class test_DEBUG
{
    [Fact]
    public void test_DebugOK_CHECK()
    {
        var cl = GameBuilder.getCommandLine();

        var resp = cl.ExecuteCommandString("DEBUG CHECK");

        Assert.Equal(CLI_Messages.DebugCommandOK, resp);
    }

    [Fact]
    public void test_DebugSetGetSectorContents()
    {
        var cl = GameBuilder.getCommandLine();

        var contents = "STARSHIP";

        var resp = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 0 0 {contents}");
        var resp2 = cl.ExecuteCommandString("DEBUG GET CURRENTQUADRANT SECTOR 0 0");

        Assert.Equal(contents, resp2);
    }

    [Fact]
    public void test_DebugSetGetSectorEnemy()
    {
        var cl = GameBuilder.getCommandLine();
        string shieldLevel = "1000";

        var resp = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 1 1 {shieldLevel}");
        var resp2 = cl.ExecuteCommandString("DEBUG GET CURRENTQUADRANT ENEMY 1 1");

        Console.WriteLine($"[{resp2}]");

        Assert.Contains(shieldLevel, resp2);
    }

    [Fact]
    public void test_DebugSetGetQuadrantNumEnemies()
    {
        var cl = GameBuilder.getCommandLine();
        string numEnemies = "3";

        var resp = cl.ExecuteCommandString($"DEBUG SET QUADRANT 0 0 NUMENEMIES {numEnemies}");
        var resp2 = cl.ExecuteCommandString("DEBUG GET QUADRANT 0 0 NUMENEMIES");

        Console.WriteLine($"[{resp}]");
        Console.WriteLine($"[{resp2}]");

        Assert.Contains(numEnemies, resp2);
    }

    [Fact]
    public void test_DebugSetGetQuadrantNumStars()
    {
        var cl = GameBuilder.getCommandLine();
        string numStars = "5";

        var resp = cl.ExecuteCommandString($"DEBUG SET QUADRANT 0 0 NUMSTARS {numStars}");
        var resp2 = cl.ExecuteCommandString("DEBUG GET QUADRANT 0 0 NUMSTARS");

        Console.WriteLine($"[{resp}]");
        Console.WriteLine($"[{resp2}]");

        Assert.Contains(numStars, resp2);
    }

    [Fact]
    public void test_DebugSetGetQuadrantStarBase()
    {
        var cl = GameBuilder.getCommandLine();
        string hasSB = "1";

        var resp = cl.ExecuteCommandString($"DEBUG SET QUADRANT 0 0 STARBASE {hasSB}");
        var resp2 = cl.ExecuteCommandString("DEBUG GET QUADRANT 0 0 STARBASE");

        Console.WriteLine($"[{resp}]");
        Console.WriteLine($"[{resp2}]");

        Assert.Contains(hasSB, resp2);
    }

    [Fact]
    public void test_DebugSetGetQuadrantExplored()
    {
        var cl = GameBuilder.getCommandLine();
        string exploredSW = "1";

        var resp = cl.ExecuteCommandString($"DEBUG SET QUADRANT 0 0 EXPLORED {exploredSW}");
        var resp2 = cl.ExecuteCommandString("DEBUG GET QUADRANT 0 0 EXPLORED");

        Console.WriteLine($"[{resp}]");
        Console.WriteLine($"[{resp2}]");

        Assert.Contains(exploredSW, resp2);
    }

    [Fact]
    public void test_DebugSetGet_Starship_Energy()
    {
        var cl = GameBuilder.getCommandLine();
        string energyAmt = "2000";

        var resp = cl.ExecuteCommandString($"DEBUG SET STARSHIP ENERGY {energyAmt}");
        var resp2 = cl.ExecuteCommandString("DEBUG GET STARSHIP ENERGY");

        Console.WriteLine($"[{resp}]");
        Console.WriteLine($"[{resp2}]");

        Assert.Contains(energyAmt, resp2);
    }

    [Fact]
    public void test_DebugSetGet_Starship_ShieldLevel()
    {
        var cl = GameBuilder.getCommandLine();
        string shieldAmt = "1000";

        var resp = cl.ExecuteCommandString($"DEBUG SET STARSHIP SHIELD {shieldAmt}");
        var resp2 = cl.ExecuteCommandString("DEBUG GET STARSHIP SHIELD");

        Console.WriteLine($"[{resp}]");
        Console.WriteLine($"[{resp2}]");

        Assert.Contains(shieldAmt, resp2);
    }

    [Fact]
    public void test_DebugSetGet_Starship_Torps()
    {
        var cl = GameBuilder.getCommandLine();
        string torpAmount = "5";

        var resp = cl.ExecuteCommandString($"DEBUG SET STARSHIP TORP {torpAmount}");
        var resp2 = cl.ExecuteCommandString("DEBUG GET STARSHIP TORP");

        Console.WriteLine($"[{resp}]");
        Console.WriteLine($"[{resp2}]");

        Assert.Contains(torpAmount, resp2);
    }

    [Fact]
    public void test_DebugSetGet_Starship_DeviceDamage()
    {
        var cl = GameBuilder.getCommandLine();
        string damaageAmt = "-3";
        string device = "TOR";

        var resp = cl.ExecuteCommandString($"DEBUG SET STARSHIP DEVICE {device} {damaageAmt}");
        var resp2 = cl.ExecuteCommandString($"DEBUG GET STARSHIP DEVICE {device}");

        Console.WriteLine($"[{resp}]");
        Console.WriteLine($"[{resp2}]");

        Assert.Contains(damaageAmt, resp2);
    }

    [Fact]
    public void test_DebugSetGet_Galaxy_MissionTime()
    {
        var cl = GameBuilder.getCommandLine();
        string sd = "2400";

        var resp = cl.ExecuteCommandString($"DEBUG SET GALAXY MISSIONTIME {sd}");
        var resp2 = cl.ExecuteCommandString($"DEBUG GET GALAXY MISSIONTIME");

        Console.WriteLine($"[{resp}]");
        Console.WriteLine($"[{resp2}]");

        Assert.Contains(sd, resp2);
    }

    [Fact]
    public void test_DebugSetGet_Galaxy_MissionTimeDeadLine()
    {
        var cl = GameBuilder.getCommandLine();
        string sd = "2410";

        var resp = cl.ExecuteCommandString($"DEBUG SET GALAXY MISSIONTIMEDEADLINE {sd}");
        var resp2 = cl.ExecuteCommandString($"DEBUG GET GALAXY MISSIONTIMEDEADLINE");

        Console.WriteLine($"[{resp}]");
        Console.WriteLine($"[{resp2}]");

        Assert.Contains(sd, resp2);
    }

    [Fact]
    public void test_DebugSetClearSector()
    {
        var cl = GameBuilder.getCommandLine();
        int x = 3;
        int y = 4;
        string contents = "STAR";

        cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR {x} {y} {contents}");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 0 0 STARSHIP");
        var res = cl.ExecuteCommandString($"DEBUG GET CURRENTQUADRANT SECTOR {x} {y}");
        var res2 = cl.ExecuteCommandString($"DEBUG GET CURRENTQUADRANT SECTOR 0 0 BRIEF");

        Assert.Equal(contents, res);
        Assert.Equal("STARSHIP", res2);
    }

    [Fact]
    public void test_Debug_GetSet_StarshipQuadrant()
    {
        var cl = GameBuilder.getCommandLine();
        string x = "1";
        string y = "1";

        var resp = cl.ExecuteCommandString($"DEBUG SET STARSHIP QUADRANT {x} {y}");
        var resp2 = cl.ExecuteCommandString("DEBUG GET STARSHIP QUADRANT");

        Console.WriteLine($"[{resp}]");
        Console.WriteLine($"[{resp2}]");

        Assert.Contains($"{x}.{y}", resp2);
    }
}
