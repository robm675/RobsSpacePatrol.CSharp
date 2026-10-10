
using RobsSpacePatrol.CLI.Library;

namespace RobsSpacePatrol.CLI.Library.Tests;

public class test_NAV
{
    [Fact]
    public void test_NAV_InvalidNumberOfParms()
    {
        var cl = GameBuilder.getCommandLine();

        var RES = cl.ExecuteCommandString("NAV");

        Assert.Contains(CLI_Messages.NAV_InvalidCommand, RES);
    }

    [Fact]
    public void test_NAV_InvalidDIRParm_Range()
    {
        var cl = GameBuilder.getCommandLine();

        var RES = cl.ExecuteCommandString("NAV 9 1");


        Assert.Contains(CLI_Messages.NAV_Invalid_DIR + "9", RES);
    }

    [Fact]
    public void test_NAV_InvalidDIRParm_Alpha()
    {
        var cl = GameBuilder.getCommandLine();

        var RES = cl.ExecuteCommandString("NAV a 1");

        Assert.Equal(CLI_Messages.NAV_InvalidDIRValue + "a", RES);
    }

    [Fact]
    public void test_NAV_InvalidDISTParm_Range()
    {
        var cl = GameBuilder.getCommandLine();

        var RES = cl.ExecuteCommandString("NAV 1 9");

        Assert.Contains(CLI_Messages.NAV_Invalid_DIST + "9", RES);
    }

    [Fact]
    public void test_NAV_InvalidDISTParm_Alpha()
    {
        var cl = GameBuilder.getCommandLine();

        var RES = cl.ExecuteCommandString("NAV 1 a");

        Assert.Equal(CLI_Messages.NAV_InvalidDISTValue + "a", RES);
    }

    [Fact]
    public void test_NAV_Working()
    {
        var cl = GameBuilder.getCommandLine();
        cl.ExecuteCommandString($"DEBUG SET SKIPENEMYMOVE ON");

        cl.ExecuteCommandString("DEBUG SET CLEARCQ");

        var RES = cl.ExecuteCommandString("NAV 1 2");
        Assert.NotEqual(string.Empty, RES);
    }

    [Fact]
    public void test_NAV_Working_VerifyLocation_EW()
    {
        var cl = GameBuilder.getCommandLine();


        cl.ExecuteCommandString($"DEBUG SET STARSHIP QUADRANT 0 0");
        cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 0 0 STARSHIP");
        cl.ExecuteCommandString($"DEBUG SET SKIPENEMYMOVE ON");

        var RES = cl.ExecuteCommandString("NAV 1 2");
        var resp2 = cl.ExecuteCommandString("DEBUG GET STARSHIP QUADRANT BRIEF");

        Console.WriteLine(RES);
        Console.WriteLine(resp2);

        Assert.Equal("2.0", resp2);
    }

    [Fact]
    public void test_NAV_Working_VerifyLocation_NS()
    {
        var cl = GameBuilder.getCommandLine();


        cl.ExecuteCommandString($"DEBUG SET STARSHIP QUADRANT 0 0");
        cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 0 0 STARSHIP");


        var RES = cl.ExecuteCommandString("NAV 7 2");
        var resp2 = cl.ExecuteCommandString("DEBUG GET STARSHIP QUADRANT BRIEF");

        Console.WriteLine(RES);
        Console.WriteLine(resp2);

        Assert.Equal("0.2", resp2);
    }

    [Fact]
    public void test_NAV_Damaged_DistTooLarge()
    {
        var cl = GameBuilder.getCommandLine();

        var resp = cl.ExecuteCommandString($"DEBUG SET STARSHIP DEVICE NAV -3");
        var RES = cl.ExecuteCommandString("NAV 1 2");

        Assert.Equal(CLI_Messages.NAV_SpaceDriveDamaged, RES);
    }

    [Fact]
    public void test_NAV_Damaged_DistOK()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString("DEBUG SET STARSHIP QUADRANT 0 0");
        cl.ExecuteCommandString("DEBUG SET CLEARCQ");

        var resp = cl.ExecuteCommandString($"DEBUG SET STARSHIP DEVICE NAV -3");
        var RES = cl.ExecuteCommandString("NAV 1 .2");
        Console.WriteLine($"resp = {resp}");
        Console.WriteLine($"RES = {RES}");

        Assert.Contains(CLI_Messages.NAV_ImpulseEngaged, RES);
    }

    [Fact]
    public void test_NAV_VerifyMissionTimeChanged_SpaceDrive()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString($"DEBUG SET STARSHIP QUADRANT 0 0");
        cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 0 0 STARSHIP");

        var STBefore = cl.ExecuteCommandString("DEBUG GET GALAXY MISSIONTIME BRIEF");
        var RES = cl.ExecuteCommandString("NAV 1 2");
        var STAfter = cl.ExecuteCommandString("DEBUG GET GALAXY MISSIONTIME BRIEF");

        Console.WriteLine(RES);

        Console.WriteLine($"STBefore = {STBefore}");
        Console.WriteLine($"STAfter = {STAfter}");

        Assert.NotEqual(STBefore, STAfter);
    }

    [Fact]
    public void test_NAV_VerifyMissionTimeChanged_ImpulseDrive()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 0 0 STARSHIP");
        var resp1 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 7 100");
        var resp2 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 6 100");
        var resp3 = cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT ENEMY 0 5 100");
        cl.ExecuteCommandString($"DEBUG SET SKIPENEMYMOVE ON");

        var STBefore = cl.ExecuteCommandString("DEBUG GET GALAXY MISSIONTIME BRIEF");
        var RES = cl.ExecuteCommandString("NAV 1 .2");
        var STAfter = cl.ExecuteCommandString("DEBUG GET GALAXY MISSIONTIME BRIEF");

        Console.WriteLine(RES);

        Console.WriteLine($"STBefore = {STBefore}");
        Console.WriteLine($"STAfter = {STAfter}");

        Assert.NotEqual(STBefore, STAfter);
    }

    [Fact]
    public void test_NAV_OutsideGalaxy()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString($"DEBUG SET STARSHIP QUADRANT 0 0");
        cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 0 0 STARSHIP");


        var RES = cl.ExecuteCommandString("NAV 5 1");

        Assert.Equal(CLI_Messages.NAV_OutsideGalaxy, RES);
    }

    [Fact]
    public void test_NAV_SpaceDriveEngaged()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString($"DEBUG SET STARSHIP QUADRANT 0 0");
        cl.ExecuteCommandString("DEBUG SET CLEARCQ");


        var RES = cl.ExecuteCommandString("NAV 1 2");

        Console.WriteLine(RES);

        Assert.Contains(CLI_Messages.NAV_SpaceDriveEngaged, RES);
    }

    [Fact]
    public void test_NAV_ImpulseDrivesShutDown()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 0 0 STARSHIP");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 7 0 STAR");

        var RES = cl.ExecuteCommandString("NAV 1 2");

        Assert.Contains(CLI_Messages.NAV_ImpulseEnginesShutDown, RES);
        Assert.Contains(CLI_Messages.NAV_BadNavigation, RES);
    }

    [Fact]
    public void test_NAV_NotEnoughEnergyForTrip_ShieldEnergyAvailable()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 0 0 STARSHIP");

        cl.ExecuteCommandString("DEBUG SET STARSHIP QUADRANT 3 3");
        cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");
        cl.ExecuteCommandString("DEBUG SET CURRENTQUADRANT SECTOR 0 0 STARSHIP");
        cl.ExecuteCommandString($"DEBUG SET STARSHIP ENERGY 0");
        cl.ExecuteCommandString($"DEBUG SET STARSHIP SHIELD 1000");

        var RES = cl.ExecuteCommandString("NAV 1 1");

        Console.WriteLine(RES);

        Assert.Contains(CLI_Messages.NAV_NotEnoughEnergy_ShieldEnergyAvail, RES);
    }

    [Fact]
    public void test_Nav_ArriveInNewQuadrant_EnemyPresent_NoShieldsGetWarningMessage()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString($"DEBUG SET STARSHIP QUADRANT 0 0");
        cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 0 0 STARSHIP");
        cl.ExecuteCommandString($"DEBUG SET QUADRANT 1 0 NUMENEMIES 3");
        cl.ExecuteCommandString($"DEBUG SET STOPENEMYFIRE ON");


        var RES = cl.ExecuteCommandString("NAV 1 1");

        Console.WriteLine(RES);

        Assert.Contains(CLI_Messages.WarningMessageShieldDownInCombatArea, RES);
    }

    [Fact]
    public void test_Nav_ArriveInNewQuadrant_EnemyPresent_GetRedAlertMessage()
    {
        var cl = GameBuilder.getCommandLine();

        cl.ExecuteCommandString($"DEBUG SET STARSHIP QUADRANT 0 0");
        cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");
        cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 0 0 STARSHIP");
        cl.ExecuteCommandString($"DEBUG SET QUADRANT 1 0 NUMENEMIES 3");
        cl.ExecuteCommandString($"DEBUG SET STARSHIP SHIELD 1000");
        cl.ExecuteCommandString($"DEBUG SET SKIPENEMYMOVE ON");


        var RES = cl.ExecuteCommandString("NAV 1 1");

        Console.WriteLine(RES);

        Assert.Contains(CLI_Messages.MAINT_NewQuadrantEnemiesRedAlert, RES);
    }

    //void showData(GameV2 game)
    //{
    //    Console.WriteLine(new String('*',80));
    //    Console.WriteLine(game.getGalaxyFormatted());
    //    Console.WriteLine(game.getCurrentQuadrantFormatted());
    //    Console.WriteLine();

    //    Console.WriteLine($"Starship.StarshipSectorCoord:  {game.Galaxy.Starship.StarshipSectorCoord.X}.{game.Galaxy.Starship.StarshipSectorCoord.Y}");
    //    var cqSect = game.Galaxy.CurrentQuadrant.Sectors.First(z => z.SectorContents.SectorContents == Global.SectorStarship);
    //    Console.WriteLine($"CurrentQuadrant Starship Sector: " + cqSect.Coord.X + "." + cqSect.Coord.Y);

    //    Console.WriteLine();
    //    int starCount = game.Galaxy.CurrentQuadrant.Sectors.Count(z => z.SectorContents.SectorContents == RobsSpacePatrol.Global.Global.SectorStar);
    //    Console.WriteLine($"CurrentQuadrant StarCount: {starCount}");
    //    int galStarCount = game.Galaxy.Quadrants.First(z => Global.getComparer().Compare(z.Coord, game.Galaxy.CurrentQuadrant.Coord).Equal).NumberOfStars;
    //    Console.WriteLine($"Galaxy StarCount: {galStarCount}");


    //    Console.WriteLine(new String('#',80));
    //}
    //[Fact]
    //public void test_NAV_LocationAfterSpace1()
    //{
    //    var game = GameBuilder.getGame();
    //    var cl = new CommandLineV3(game, false);

    //    showData(game);
    //    cl.ExecuteCommandString("DEBUG SET STARSHIP QUADRANT 0 0");
    //    //cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");
    //    cl.ExecuteCommandString($"DEBUG SET STARSHIP SECTOR 0 0");
    //    cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 0 0 STARSHIP");
    //    showData(game);

    //    //Console.WriteLine("DEBUG GET STARSHIP QUADRANT BRIEF: " + cl.ExecuteCommandString("DEBUG GET STARSHIP QUADRANT BRIEF"));
    //    //Console.WriteLine("DEBUG GET STARSHIP SECTOR: " + cl.ExecuteCommandString($"DEBUG GET STARSHIP SECTOR"));

    //}
    //[Fact]
    //public void test_Verify_SRS_AfterNewQuadrant()
    //{
    //    var game = GameBuilder.getGame();
    //    var cl = new CommandLineV3(game, false);

    //    //cl.ExecuteCommandString($"DEBUG SET QUADRANT 0 0 NUMSTARS 5");
    //    //cl.ExecuteCommandString($"DEBUG SET QUADRANT 0 0 STARBASE 0");
    //    //cl.ExecuteCommandString($"DEBUG SET QUADRANT 0 0 NUMENEMIES 3");
    //    //cl.ExecuteCommandString($"DEBUG SET QUADRANT 0 0 EXPLORED 1");
    //    //cl.ExecuteCommandString($"DEBUG SET STARSHIP QUADRANT 0 0");
    //    //cl.ExecuteCommandString($"DEBUG SET CURRENTQUADRANT SECTOR 0 0 STARSHIP");


    //    //cl.ExecuteCommandString($"DEBUG SET QUADRANT 1 0 NUMSTARS 0");
    //    //cl.ExecuteCommandString($"DEBUG SET QUADRANT 1 0 STARBASE 0");
    //    //cl.ExecuteCommandString($"DEBUG SET QUADRANT 1 0 NUMENEMIES 0");
    //    //cl.ExecuteCommandString($"DEBUG SET QUADRANT 1 0 EXPLORED 1");

    //    //cl.ExecuteCommandString($"DEBUG SET QUADRANT 1 1 NUMSTARS 0");
    //    //cl.ExecuteCommandString($"DEBUG SET QUADRANT 1 1 STARBASE 0");
    //    //cl.ExecuteCommandString($"DEBUG SET QUADRANT 1 1 NUMENEMIES 0");
    //    //cl.ExecuteCommandString($"DEBUG SET QUADRANT 1 1 EXPLORED 1");

    //    //cl.ExecuteCommandString($"DEBUG SET QUADRANT 0 1 NUMSTARS 0");
    //    //cl.ExecuteCommandString($"DEBUG SET QUADRANT 0 1 STARBASE 0");
    //    //cl.ExecuteCommandString($"DEBUG SET QUADRANT 0 1 NUMENEMIES 0");
    //    //cl.ExecuteCommandString($"DEBUG SET QUADRANT 0 1 EXPLORED 1");


    //    //cl.ExecuteCommandString($"DEBUG SET QUADRANT 2 0 NUMSTARS 0");
    //    //cl.ExecuteCommandString($"DEBUG SET QUADRANT 2 0 STARBASE 0");
    //    //cl.ExecuteCommandString($"DEBUG SET QUADRANT 2 0 NUMENEMIES 0");
    //    //cl.ExecuteCommandString($"DEBUG SET QUADRANT 2 0 EXPLORED 1");

    //    //cl.ExecuteCommandString($"DEBUG SET QUADRANT 2 1 NUMSTARS 0");
    //    //cl.ExecuteCommandString($"DEBUG SET QUADRANT 2 1 STARBASE 0");
    //    //cl.ExecuteCommandString($"DEBUG SET QUADRANT 2 1 NUMENEMIES 0");
    //    //cl.ExecuteCommandString($"DEBUG SET QUADRANT 2 1 EXPLORED 1");

    //    //cl.ExecuteCommandString($"DEBUG SET STARSHIP QUADRANT 0 0");
    //    //cl.ExecuteCommandString("DEBUG SET CLEARCQ ALL");

    //    var res1 = cl.ExecuteCommandString("COM REC");
    //    Console.WriteLine(res1);
    //    Console.WriteLine(game.getCurrentQuadrantFormatted());
    //    Console.WriteLine($"{game.Galaxy.Starship.StarshipSectorCoord.X}.{game.Galaxy.Starship.StarshipSectorCoord.Y}");
    //    var srsRES1 = cl.ExecuteCommandString("SRS");
    //    Console.WriteLine(srsRES1);


    //    cl.ExecuteCommandString("NAV 1 1");

    //    var res2 = cl.ExecuteCommandString("COM REC");
    //    Console.WriteLine(res2);
    //    Console.WriteLine(game.getCurrentQuadrantFormatted());
    //    Console.WriteLine($"{game.Galaxy.Starship.StarshipSectorCoord.X}.{game.Galaxy.Starship.StarshipSectorCoord.Y}");
    //    var srsRES2 = cl.ExecuteCommandString("SRS");
    //    Console.WriteLine(srsRES2);
    //}
}