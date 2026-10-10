using RobsSpacePatrol.CLI.Library;

namespace RobsSpacePatrol.CLI.Library.Tests;

public class test_COM_STA
{
    [Fact]
    public void test_COM_STA_Working()
    {
        var cl = GameBuilder.getCommandLine();

        var RES = cl.ExecuteCommandString("COM STA");

        Assert.NotEqual(string.Empty, RES);
    }
    [Fact]
    public void test_COM_STA_Damaged()
    {
        var cl = GameBuilder.getCommandLine();

        var resp = cl.ExecuteCommandString($"DEBUG SET STARSHIP DEVICE COM -3");
        var RES = cl.ExecuteCommandString("COM STA");

        Assert.Equal(CLI_Messages.CPU_Damaged, RES);
    }
    [Fact]
    public void test_COM_STA_VerifyContents()
    {
        var cl = GameBuilder.getCommandLine();

        var RES = cl.ExecuteCommandString("COM STA");
        cl.ExecuteCommandString($"DEBUG SET SKIPENEMYMOVE ON");

        Assert.Contains(CLI_Messages.CPU_STA_DevicesDamaged, RES);
        Assert.Contains(CLI_Messages.CPU_STA_Docked, RES);
        Assert.Contains(CLI_Messages.CPU_STA_EnemiesRemaining, RES);
        Assert.Contains(CLI_Messages.CPU_STA_EnergyRemaining, RES);
        Assert.Contains(CLI_Messages.CPU_STA_Quadrant, RES);
        Assert.Contains(CLI_Messages.CPU_STA_Sector, RES);
        Assert.Contains(CLI_Messages.CPU_STA_ShieldLevel, RES);
        Assert.Contains(CLI_Messages.CPU_STA_StarbasesRemaining, RES);
        Assert.Contains(CLI_Messages.CPU_STA_MissionTime, RES);
        Assert.Contains(CLI_Messages.CPU_STA_MissionTimeUnitsRemaining, RES);
        Assert.Contains(CLI_Messages.CPU_STA_TorpedoRemaining, RES);
    }
}
