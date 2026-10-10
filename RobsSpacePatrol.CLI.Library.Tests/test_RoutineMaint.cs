using RobsSpacePatrol.CLI.Library;

[assembly: CaptureConsole]

namespace RobsSpacePatrol.CLI.Library.Tests;

public class test_RoutineMaint
{
    [Fact]
    public void test_RoutineMaint_EnemyFiredCoords()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 0 0 STARSHIP");
        cl.ExecuteCommandString("SHE 1000");
        var resp1 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 7 100");
        var resp2 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 6 100");
        var resp3 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 5 100");
        cl.ExecuteCommandString($"DEBUG SET SKIPENEMYMOVE ON");

        var RES = cl.ExecuteCommandString("SRS");

        Console.WriteLine(RES);

        Assert.Contains("0.5", RES);
    }

    [Fact]
    public void test_RoutineMaint_StarshipDestroyed_NoWarningMessage()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 0 0 STARSHIP");
        var resp1 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 7 100");
        var resp2 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 6 100");
        var resp3 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 5 100");

        var RES = cl.ExecuteCommandString("SRS");

        Console.WriteLine(RES);

        Assert.DoesNotContain(CLI_Messages.WarningMessageShieldDownInCombatArea, RES);
    }

    [Fact]
    public void test_RoutineMaint_EnemyFired_ThreeEnemiesFired()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 0 0 STARSHIP");
        var resp1 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 7 100");
        var resp2 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 6 100");
        var resp3 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 5 100");
        cl.ExecuteCommandString("DEBUG SET STARSHIP SHIELD 1000");
        cl.ExecuteCommandString($"DEBUG SET SKIPENEMYMOVE ON");

        var RES = cl.ExecuteCommandString("SRS");

        Console.WriteLine(RES);

        Assert.Contains("0.5", RES);
        Assert.Contains("0.6", RES);
        Assert.Contains("0.7", RES);
    }

    [Fact]
    public void test_RoutineMaint_EnemyFired_ProtectedByStarBase()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 0 0 STARSHIP");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 0 1 STARBASE");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 7 100");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 6 100");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 5 100");
        cl.ExecuteCommandString($"DEBUG SET SKIPENEMYMOVE ON");

        var RES = cl.ExecuteCommandString("SRS");

        Console.WriteLine(RES);

        Assert.Contains(CLI_Messages.MAINT_StarBaseShieldsProtected, RES);
    }

    [Fact]
    public void test_RoutineMaint_EnemyFired_ProtectedByStarBase_Repeatedly()
    {
        var cl = GameBuilder.getCommandLine();
        cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 0 0 STARSHIP");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 0 1 STARBASE");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 7 100");
        cl.ExecuteCommandString($"DEBUG SET SKIPENEMYMOVE ON");

        var RES = cl.ExecuteCommandString("SRS");
        RES = cl.ExecuteCommandString("SRS");
        RES = cl.ExecuteCommandString("SRS");
        RES = cl.ExecuteCommandString("SRS");
        RES = cl.ExecuteCommandString("SRS");

        Console.WriteLine(RES);

        Assert.Contains(CLI_Messages.MAINT_StarBaseShieldsProtected, RES);
    }

    [Fact]
    public void test_RoutineMaint_DeviceRepaired()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 0 0 STARSHIP");
        cl.ExecuteCommandString("SHE 1000");
        cl.ExecuteCommandString("DEBUG SET STARSHIP DEVICE TOR -.1");
        cl.ExecuteCommandString($"DEBUG SET SKIPENEMYMOVE ON");

        var RES = cl.ExecuteCommandString("SRS");

        Console.WriteLine(RES);

        Assert.Contains(CLI_Messages.MAINT_RepairsComplete, RES);
    }

    [Fact]
    public void test_RoutineMaint_DeviceRepaired_NotRepeated()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 0 0 STARSHIP");
        cl.ExecuteCommandString("SHE 1000");
        cl.ExecuteCommandString("DEBUG SET STARSHIP DEVICE TOR -.1");

        var RES = cl.ExecuteCommandString("SRS");
        RES = cl.ExecuteCommandString("SRS");

        Console.WriteLine(RES);

        Assert.DoesNotContain(CLI_Messages.MAINT_RepairsComplete, RES); //Repair should not be repeated
    }

    [Fact]
    public void test_RoutineMaint_EnemiesMoved()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 0 0 STARSHIP");
        cl.ExecuteCommandString("SHE 1000");
        var resp1 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 7 100");
        cl.ExecuteCommandString($"DEBUG SET SKIPENEMYMOVE FORCE");

        var RES = cl.ExecuteCommandString("SRS");

        Console.WriteLine(RES);

        Assert.Contains("Enemy Moved", RES);
    }
}
