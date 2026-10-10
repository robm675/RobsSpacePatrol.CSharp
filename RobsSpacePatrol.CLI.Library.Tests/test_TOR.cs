using RobsSpacePatrol.CLI.Library;

namespace RobsSpacePatrol.CLI.Library.Tests;

public class test_TOR
{
    [Fact]
    public void test_TOR_Working()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 0 0 STARSHIP");
        var resp1 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 7 100");
        var resp2 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 6 100");
        var resp3 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 5 100");


        var RES = cl.ExecuteCommandString("TOR 1");

        Assert.NotEqual(string.Empty, RES);
    }
    [Fact]
    public void test_TOR_Damaged()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");

        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 0 0 STARSHIP");
        var resp1 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 7 100");
        var resp2 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 6 100");
        var resp3 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 5 100");


        var resp = cl.ExecuteCommandString($"DEBUG SET STARSHIP DEVICE TOR -3");
        var RES = cl.ExecuteCommandString("TOR 1");

        Console.WriteLine(RES);

        Assert.Equal(CLI_Messages.TOR_Damaged, RES);
    }
    [Fact]
    public void test_TOR_VerifyMissionTimeChanged()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 0 0 STARSHIP");
        var resp1 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 7 100");
        var resp2 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 6 100");
        var resp3 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 5 100");
        cl.ExecuteCommandString($"DEBUG SET SKIPENEMYMOVE ON");

        var STBefore = cl.ExecuteCommandString("DEBUG GET GALAXY MISSIONTIME BRIEF");
        var RES = cl.ExecuteCommandString("TOR 7");
        var STAfter = cl.ExecuteCommandString("DEBUG GET GALAXY MISSIONTIME BRIEF");

        Console.WriteLine(RES);

        Console.WriteLine($"STBefore = {STBefore}");
        Console.WriteLine($"STAfter = {STAfter}");

        Assert.NotEqual(STBefore, STAfter);
    }
    [Fact]
    public void test_TOR_NoEnemies()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");

        var RES = cl.ExecuteCommandString("TOR 1");

        Assert.Equal(CLI_Messages.TOR_NoEnemies, RES);
    }
    [Fact]
    public void test_TOR_InvalidDirection()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 0 0 STARSHIP");
        var resp1 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 7 100");
        var resp2 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 6 100");
        var resp3 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 5 100");

        var RES = cl.ExecuteCommandString("TOR 9");

        Assert.StartsWith(CLI_Messages.TOR_InvalidDirection, RES);
    }
    [Fact]
    public void test_TOR_VerifyHitResults_EnemyDestroyed()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 0 0 STARSHIP");
        var resp1 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 7 100");
        var resp2 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 6 100");
        var resp3 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 5 100");
        cl.ExecuteCommandString($"DEBUG SET SKIPENEMYMOVE ON");

        var RES = cl.ExecuteCommandString("TOR 7");


        Assert.Contains(CLI_Messages.TOR_Fired, RES);
        Assert.Contains(CLI_Messages.TOR_EnemyAtSector, RES);
        Assert.Contains(CLI_Messages.TOR_Destroyed, RES);
    }
    [Fact]
    public void test_TOR_VerifyHitResults_StarBaseDestroyed()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 0 0 STARSHIP");
        var resp1 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 0 7 STARBASE");
        var resp3 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 4 5 100");
        cl.ExecuteCommandString($"DEBUG SET SKIPENEMYMOVE ON");


        var RES = cl.ExecuteCommandString("TOR 7");


        Assert.Contains(CLI_Messages.TOR_Fired, RES);
        Assert.Contains(CLI_Messages.TOR_StarBaseDestroyed, RES);
    }
    [Fact]
    public void test_TOR_InsufficientTorps()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 0 0 STARSHIP");
        var resp1 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 7 100");
        var resp2 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 6 100");
        var resp3 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 5 100");

        var resp = cl.ExecuteCommandString($"DEBUG SET STARSHIP TORP 0");
        var RES = cl.ExecuteCommandString("TOR 1");

        Assert.Equal(CLI_Messages.TOR_Expended, RES);
    }
    [Fact]
    public void test_TOR_Missed()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 0 0 STARSHIP");
        var resp1 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 5 7 100");

        var RES = cl.ExecuteCommandString("TOR 6");

        Console.WriteLine(RES);

        Assert.Contains(CLI_Messages.TOR_Missed, RES);
    }
    [Fact]
    public void test_TOR_VerifyHitResults_EnemyDestroyedCoords()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 0 0 STARSHIP");
        var resp1 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 7 100");
        var resp2 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 6 100");
        var resp3 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 5 100");
        cl.ExecuteCommandString($"DEBUG SET SKIPENEMYMOVE ON");

        var RES = cl.ExecuteCommandString("TOR 7");

        Console.WriteLine(RES);

        Assert.Contains(CLI_Messages.TOR_Fired, RES);
        Assert.Contains(CLI_Messages.TOR_EnemyAtSector, RES);
        Assert.Contains(CLI_Messages.TOR_Destroyed, RES);
        Assert.Contains("0.5", RES);
    }
}
