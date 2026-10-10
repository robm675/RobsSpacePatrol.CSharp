using RobsSpacePatrol.CLI.Library;

namespace RobsSpacePatrol.CLI.Library.Tests;

public class test_LRS
{
    [Fact]
    public void test_LRS_Working()
    {
        var cl = GameBuilder.getCommandLine();

        var RES = cl.ExecuteCommandString("LRS");

        Assert.NotEqual(string.Empty, RES);
    }
    [Fact]
    public void test_LRS_Damaged()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString("DEBUG SET STARSHIP DEVICE LRS -3");

        var RES = cl.ExecuteCommandString("LRS");

        Assert.Equal(CLI_Messages.LRS_Damaged, RES);
    }
    [Fact]
    public void test_LRS_VerifyMissionTimeChanged()
    {
        var cl = GameBuilder.getCommandLine();

        var STBefore = cl.ExecuteCommandString("DEBUG GET GALAXY MISSIONTIME BRIEF");
        var RES = cl.ExecuteCommandString("LRS");
        var STAfter = cl.ExecuteCommandString("DEBUG GET GALAXY MISSIONTIME BRIEF");

        Console.WriteLine($"STBefore = {STBefore}");
        Console.WriteLine($"STAfter = {STAfter}");

        Assert.NotEqual(STBefore, STAfter);

    }
    [Fact]
    public void test_LRS_Contents()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString($"DEBUG SET STARSHIP QUADRANT 0 0");
        cl.ExecuteCommandString($"DEBUG SET QUADRANT 0 0 NUMSTARS 5");
        cl.ExecuteCommandString($"DEBUG SET QUADRANT 0 0 STARBASE 0");
        cl.ExecuteCommandString($"DEBUG SET QUADRANT 0 0 NUMENEMIES 3");

        cl.ExecuteCommandString($"DEBUG SET QUADRANT 1 0 NUMSTARS 1");
        cl.ExecuteCommandString($"DEBUG SET QUADRANT 1 0 STARBASE 1");
        cl.ExecuteCommandString($"DEBUG SET QUADRANT 1 0 NUMENEMIES 0");

        cl.ExecuteCommandString($"DEBUG SET QUADRANT 0 1 NUMSTARS 1");
        cl.ExecuteCommandString($"DEBUG SET QUADRANT 0 1 STARBASE 1");
        cl.ExecuteCommandString($"DEBUG SET QUADRANT 0 1 NUMENEMIES 0");

        cl.ExecuteCommandString($"DEBUG SET QUADRANT 1 1 NUMSTARS 9");
        cl.ExecuteCommandString($"DEBUG SET QUADRANT 1 1 STARBASE 0");
        cl.ExecuteCommandString($"DEBUG SET QUADRANT 1 1 NUMENEMIES 4");

        var RES = cl.ExecuteCommandString("LRS");
        var lines = RES.Split('\n');

        Console.WriteLine(RES);

        Console.WriteLine(lines[2]);
        Console.WriteLine(lines[4]);
        Console.WriteLine(lines[6]);

        Assert.NotEqual(string.Empty, RES);
        Assert.Equal("| | *** | *** | *** |", lines[2]);
        Assert.Equal("Y | *** | 305 | 011 |", lines[4]);
        Assert.Equal("| | *** | 011 | 409 |", lines[6]);
    }
}
