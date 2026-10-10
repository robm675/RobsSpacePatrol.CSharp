

using RobsSpacePatrol.CLI.Library;

namespace RobsSpacePatrol.CLI.Library.Tests;

public class test_COM_NAV
{
    [Fact]
    public void test_COM_NAV_InvalidQX_Range()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");

        var RES = cl.ExecuteCommandString("COM NAV 9 2 3 4");

        Assert.Contains(CLI_Messages.CPU_NAV_InvalidCoord, RES);
    }

    [Fact]
    public void test_COM_NAV_InvalidQX_Alpha()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");

        var RES = cl.ExecuteCommandString("COM NAV a 2 3 4");

        Assert.Contains(CLI_Messages.CPU_NAV_InvalidQX, RES);
    }

    [Fact]
    public void test_COM_NAV_InvalidQY_Range()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");

        var RES = cl.ExecuteCommandString("COM NAV 1 9 3 4");

        Assert.Contains(CLI_Messages.CPU_NAV_InvalidCoord, RES);
    }

    [Fact]
    public void test_COM_NAV_InvalidQY_Alpha()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");

        var RES = cl.ExecuteCommandString("COM NAV 1 a 3 4");

        Assert.Contains(CLI_Messages.CPU_NAV_InvalidQY, RES);
    }

    [Fact]
    public void test_COM_NAV_InvalidSX_Range()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");

        var RES = cl.ExecuteCommandString("COM NAV 1 2 9 4");

        Assert.Contains(CLI_Messages.CPU_NAV_InvalidCoord, RES);
    }

    [Fact]
    public void test_COM_NAV_InvalidSX_Alpha()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");

        var RES = cl.ExecuteCommandString("COM NAV 1 2 a 4");

        Assert.Contains(CLI_Messages.CPU_NAV_InvalidSX, RES);
    }

    [Fact]
    public void test_COM_NAV_InvalidSY_Range()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");

        var RES = cl.ExecuteCommandString("COM NAV 1 2 3 9");

        Assert.Contains(CLI_Messages.CPU_NAV_InvalidCoord, RES);
    }

    [Fact]
    public void test_COM_NAV_InvalidSY_Alpha()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");

        var RES = cl.ExecuteCommandString("COM NAV 1 2 3 a");

        Assert.Contains(CLI_Messages.CPU_NAV_InvalidSY, RES);
    }

    [Fact]
    public void test_COM_NAV_Working()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString("DEBUG SET CLEARCQ");

        var RES = cl.ExecuteCommandString("COM NAV 1 2 3 4");

        Assert.NotEqual(string.Empty, RES);
    }

    [Fact]
    public void test_COM_NAV_Damaged()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");

        cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");
        var resp = cl.ExecuteCommandString($"DEBUG SET STARSHIP DEVICE COM -3");
        var RES = cl.ExecuteCommandString("COM NAV 1 2 3 4");

        Assert.Equal(CLI_Messages.CPU_Damaged, RES);
    }

    [Fact]
    public void test_COM_NAV_VerifyContents()
    {
        var cl = GameBuilder.getCommandLine();

        var RES = cl.ExecuteCommandString("COM NAV 0 7 0 0");

        Assert.Contains(CLI_Messages.CPU_NAV_Dir, RES);
        Assert.Contains(CLI_Messages.CPU_NAV_Dist, RES);
    }

    [Theory]
    [InlineData(7, 0, 0, 0)]
    [InlineData(0, 7, 0, 0)]
    [InlineData(0, 0, 7, 0)]
    [InlineData(0, 0, 0, 7)]
    public void test_COM_NAV_VerifyEdgeCase(int qx, int qy, int sx, int sy)
    {
        var cl = GameBuilder.getCommandLine();

        var RES = cl.ExecuteCommandString($"COM NAV {qx} {qy} {sx} {sy}");

        Assert.Contains(CLI_Messages.CPU_NAV_Dir, RES);
        Assert.Contains(CLI_Messages.CPU_NAV_Dist, RES);
    }

}