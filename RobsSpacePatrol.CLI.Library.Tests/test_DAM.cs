
using RobsSpacePatrol.CLI.Library;

namespace RobsSpacePatrol.CLI.Library.Tests;

public class test_DAM
{
    [Fact]
    public void test_DAM_basics()
    {
        var cl = GameBuilder.getCommandLine();

        var RES = cl.ExecuteCommandString("DAM");

        Assert.Contains(CLI_Messages.DAM_DamageReport, RES);
    }
    [Fact]
    public void test_DAM_showed_damages()
    {
        var cl = GameBuilder.getCommandLine();

        var resp = cl.ExecuteCommandString($"DEBUG SET STARSHIP DEVICE NAV -3");
        var RES = cl.ExecuteCommandString("DAM");

        Console.WriteLine(RES);

        Assert.Contains("DAMAGED", RES.ToUpper());
    }
}
