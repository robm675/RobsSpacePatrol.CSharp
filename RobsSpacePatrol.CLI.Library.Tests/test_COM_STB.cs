
using RobsSpacePatrol.CLI.Library;

namespace RobsSpacePatrol.CLI.Library.Tests;

public class test_COM_STB
{
    [Fact]
    public void test_COM_STB_Working()
    {
        var cl = GameBuilder.getCommandLine();

        var RES = cl.ExecuteCommandString("COM STB");

        Assert.NotEqual(string.Empty, RES);
    }
    [Fact]
    public void test_COM_STB_Damaged()
    {
        var cl = GameBuilder.getCommandLine();


        var resp = cl.ExecuteCommandString($"DEBUG SET STARSHIP DEVICE COM -3");
        var RES = cl.ExecuteCommandString("COM STB");

        Assert.Equal(CLI_Messages.CPU_Damaged, RES);
    }
    [Fact]
    public void test_COM_STB_VerifyContents()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 0 0 STARSHIP");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 0 1 STARBASE");

        var RES = cl.ExecuteCommandString("COM STB");

        Assert.Contains(CLI_Messages.CPU_STB_Dir, RES);
        Assert.Contains(CLI_Messages.CPU_STB_Dist, RES);
        Assert.Contains(CLI_Messages.CPU_STB_StarbaseFoundInSector, RES);
    }
    [Fact]
    public void test_COM_STB_NoStarbases()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");
        var RES = cl.ExecuteCommandString("COM STB");

        Assert.Contains(CLI_Messages.CPU_STB_NoStarBases, RES);
    }
}
