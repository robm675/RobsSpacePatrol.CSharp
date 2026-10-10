
using RobsSpacePatrol.CLI.Library;

namespace RobsSpacePatrol.CLI.Library.Tests;

public class test_SRS
{
    [Fact]
    public void test_SRS_Working()
    {
        var cl = GameBuilder.getCommandLine();

        var RES = cl.ExecuteCommandString("SRS");

        Assert.NotEqual(string.Empty, RES);
    }

    [Fact]
    public void test_SRS_Damaged()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString("DEBUG SET STARSHIP DEVICE SRS -3");
        var RES = cl.ExecuteCommandString("SRS");

        Assert.Equal(CLI_Messages.SRS_Damaged, RES);
    }

    [Fact]
    public void test_SRS_VerifyMissionTimeChanged()
    {
        var cl = GameBuilder.getCommandLine();

        var STBefore = cl.ExecuteCommandString("DEBUG GET GALAXY MISSIONTIME BRIEF");
        var RES = cl.ExecuteCommandString("SRS");
        var STAfter = cl.ExecuteCommandString("DEBUG GET GALAXY MISSIONTIME BRIEF");

        Console.WriteLine($"STBefore = {STBefore}");
        Console.WriteLine($"STAfter = {STAfter}");

        Assert.NotEqual(STBefore, STAfter);
    }

    [Fact]
    public void test_SRS_Contents()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString($"DEBUG SET STARSHIP QUADRANT 0 0");
        cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 1 2 STAR");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 3 4 STAR");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 5 7 STAR");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 1 1 STARBASE");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 0 0 STARSHIP");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 7 1 ENEMY");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 7 3 ENEMY");

        var RES = cl.ExecuteCommandString("SRS");

        Assert.Equal(3, Regex.Matches(RES, Regex.Escape("***")).Count);
        Assert.Equal(2, Regex.Matches(RES, Regex.Escape("XXX")).Count);
        Assert.Single(Regex.Matches(RES, Regex.Escape("SSS")));
        Assert.Single(Regex.Matches(RES, Regex.Escape("BBB")));
    }
}