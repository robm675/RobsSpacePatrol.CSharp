
using RobsSpacePatrol.CLI.Library;

namespace RobsSpacePatrol.CLI.Library.Tests;

public class test_COM_REC
{
    [Fact]
    public void test_COM_REC_Working()
    {
        var cl = GameBuilder.getCommandLine();

        var RES = cl.ExecuteCommandString("COM REC");

        Assert.NotEqual(string.Empty, RES);
    }
    [Fact]
    public void test_COM_REC_Damaged()
    {
        var cl = GameBuilder.getCommandLine();

        var resp = cl.ExecuteCommandString($"DEBUG SET STARSHIP DEVICE COM -3");
        var RES = cl.ExecuteCommandString("COM REC");

        Assert.Equal(CLI_Messages.CPU_Damaged, RES);
    }
}
