
using RobsSpacePatrol.CLI.Library;

namespace RobsSpacePatrol.CLI.Library.Tests;

public class test_LAS
{
    [Fact]
    public void test_LAS_Working()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 0 0 STARSHIP");
        var resp1 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 7 100");
        var resp2 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 6 100");
        var resp3 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 5 100");
        cl.ExecuteCommandString($"DEBUG SET SKIPENEMYMOVE ON");

        var RES = cl.ExecuteCommandString("LAS 300");

        Assert.NotEqual(string.Empty, RES);
    }

    [Fact]
    public void test_LAS_Damaged()
    {
        var cl = GameBuilder.getCommandLine();

        var resp1 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 7 100");
        var resp2 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 6 100");
        var resp3 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 5 100");

        var resp = cl.ExecuteCommandString($"DEBUG SET STARSHIP DEVICE LAS -3");

        var RES = cl.ExecuteCommandString("LAS 300");

        Assert.Equal(CLI_Messages.LAS_Damaged, RES);
    }

    [Fact]
    public void test_LAS_VerifyMissionTimeChanged()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 0 0 STARSHIP");
        var resp1 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 7 100");
        var resp2 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 6 100");
        var resp3 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 5 100");
        cl.ExecuteCommandString($"DEBUG SET SKIPENEMYMOVE ON");

        var STBefore = cl.ExecuteCommandString("DEBUG GET GALAXY MISSIONTIME BRIEF");
        var RES = cl.ExecuteCommandString("LAS 300");
        var STAfter = cl.ExecuteCommandString("DEBUG GET GALAXY MISSIONTIME BRIEF");

        Console.WriteLine($"STBefore = {STBefore}");
        Console.WriteLine($"STAfter = {STAfter}");

        Assert.NotEqual(STBefore, STAfter);
    }

    [Fact]
    public void test_LAS_NoEnemies()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString("DEBUG SET CLEARCQ");
        var RES = cl.ExecuteCommandString("LAS 300");

        Assert.Equal(CLI_Messages.LAS_NoEnemies, RES);
    }

    [Fact]
    public void test_LAS_InvalidAmount()
    {
        var cl = GameBuilder.getCommandLine();

        var resp1 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 7 100");
        var resp2 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 6 100");
        var resp3 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 5 100");

        var RES = cl.ExecuteCommandString("LAS a300");

        Assert.StartsWith(CLI_Messages.LAS_InvalidAmount, RES);
    }

    [Fact]
    public void test_LAS_VerifyHitResults_Destroyed()
    {
        var cl = GameBuilder.getCommandLine();
        cl.ExecuteCommandString("DEBUG SET CLEARCQ");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 0 0 STARSHIP");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 7 1");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 6 1");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 5 1");

        var RES = cl.ExecuteCommandString("LAS 800");

        Console.WriteLine(RES);

        Assert.Equal(3, new Regex(Regex.Escape(CLI_Messages.LAS_Fired)).Matches(RES).Count);
        Assert.Equal(3, new Regex(Regex.Escape(CLI_Messages.LAS_Hit)).Matches(RES).Count);
        Assert.Equal(3, new Regex(Regex.Escape(CLI_Messages.LAS_EnemyDestroyed)).Matches(RES).Count);

        Assert.Single(new Regex(Regex.Escape("[0.7]")).Matches(RES));
        Assert.Single(new Regex(Regex.Escape("[0.6]")).Matches(RES));
        Assert.Single(new Regex(Regex.Escape("[0.5]")).Matches(RES));
    }

    [Fact]
    public void test_LAS_VerifyHitResults_Hit()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 0 0 STARSHIP");
        var resp1 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 7 100");
        var resp2 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 6 100");
        var resp3 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 5 100");
        cl.ExecuteCommandString($"DEBUG SET SKIPENEMYMOVE ON");


        var RES = cl.ExecuteCommandString("LAS 50");

        Console.WriteLine(RES);

        Assert.Equal(3, new Regex(Regex.Escape(CLI_Messages.LAS_Fired)).Matches(RES).Count);
        Assert.Equal(3, new Regex(Regex.Escape(CLI_Messages.LAS_Hit)).Matches(RES).Count);
        Assert.Equal(3, new Regex(Regex.Escape(CLI_Messages.LAS_SensorsIndicate)).Matches(RES).Count);
        Assert.Equal(3, new Regex(Regex.Escape(CLI_Messages.LAS_Remaining)).Matches(RES).Count);

        Assert.Single(new Regex(Regex.Escape("[0.7] (Sensors")).Matches(RES));
        Assert.Single(new Regex(Regex.Escape("[0.6] (Sensors")).Matches(RES));
        Assert.Single(new Regex(Regex.Escape("[0.5] (Sensors")).Matches(RES));
    }

    [Fact]
    public void test_LAS_InsufficientEnergy()
    {
        var cl = GameBuilder.getCommandLine();

        var resp1 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 7 1");
        var resp2 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 6 1");
        var resp3 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 5 1");

        var resp = cl.ExecuteCommandString($"DEBUG SET STARSHIP ENERGY 500");
        var RES = cl.ExecuteCommandString("LAS 1000");

        Assert.Equal(CLI_Messages.LAS_InsufficientEnergy, RES);
    }
}