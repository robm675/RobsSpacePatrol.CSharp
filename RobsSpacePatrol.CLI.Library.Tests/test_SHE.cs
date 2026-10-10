
using RobsSpacePatrol.CLI.Library;

namespace RobsSpacePatrol.CLI.Library.Tests;

public class test_SHE
{
    [Fact]
    public void test_SHE_Working()
    {
        var cl = GameBuilder.getCommandLine();
        cl.ExecuteCommandString($"DEBUG SET SKIPENEMYMOVE ON");

        var RES = cl.ExecuteCommandString("SHE 1000");

        Assert.StartsWith(CLI_Messages.SHE_Changed, RES);
        Assert.Contains("1000", RES);
    }
    [Fact]
    public void test_SHE_Damaged()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString("DEBUG SET STARSHIP DEVICE SHE -3");
        var RES = cl.ExecuteCommandString("SHE 1000");

        Assert.Equal(CLI_Messages.SHE_Damaged, RES);
    }
    [Fact]
    public void test_SHE_MissingAmount()
    {
        var cl = GameBuilder.getCommandLine();

        var RES = cl.ExecuteCommandString("SHE");

        Assert.Equal(CLI_Messages.SHE_MissingAmount, RES);
    }
    [Fact]
    public void test_SHE_InvalidAmount()
    {
        var cl = GameBuilder.getCommandLine();

        var RES = cl.ExecuteCommandString("SHE 4000");

        Assert.StartsWith(CLI_Messages.SHE_InvalidAmount, RES);
    }
    [Fact]
    public void test_SHE_VerifyMissionTimeChanged()
    {
        var cl = GameBuilder.getCommandLine();

        var STBefore = cl.ExecuteCommandString("DEBUG GET GALAXY MISSIONTIME BRIEF");
        var RES = cl.ExecuteCommandString("SHE 1000");
        var STAfter = cl.ExecuteCommandString("DEBUG GET GALAXY MISSIONTIME BRIEF");
        cl.ExecuteCommandString($"DEBUG SET SKIPENEMYMOVE ON");

        Console.WriteLine($"STBefore = {STBefore}");
        Console.WriteLine($"STAfter = {STAfter}");

        Assert.NotEqual(STBefore, STAfter);
    }
}
