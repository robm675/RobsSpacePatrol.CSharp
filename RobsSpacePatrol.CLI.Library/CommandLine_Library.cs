using RobsSpacePatrol.CLI.Library;
using RobsSpacePatrol.DTO.Library;
using RobsSpacePatrol.Library;
using RobsSpacePatrol.Library.Utils;
using System.Globalization;
using static RobsSpacePatrol.DTO.Library.Result;


namespace RobsSpacePatrol.CLI.Library;

public class CommandLine_Library : ICommandLine
{
    private readonly IGame _game;
    private readonly IDebug _debug;

    private double? RepairMissionTimeUnits { get; set; }
    private bool StarbaseRepairsAvailableSW { get; set; }
    private bool quitSwitch;
    private bool newQuadrant = false;

    public bool GameOver { get; private set; }

    public CommandLine_Library(IGame game, IDebug debug)
    {
        _game = game;
        _debug = debug;
    }

    private string getSectorFormatted(SectorDTO tempSector)
    {
        switch (tempSector.SectorContents)
        {
            case Global.SectorEnemy:
                return "XXX";

            case Global.SectorStarship:
                return "SSS";

            case Global.SectorEmpty:
                return "   ";

            case Global.SectorStar:
                return "***";

            case Global.SectorStarBase:
                return "BBB";

            default:
                return "???";
        }
    }

    private string getSRSFormatted(List<SectorDTO> Sectors)
    {
        const string XHeader = "  <---------------------- X ---------------------->\n";
        const string Line = "+-----+-----+-----+-----+-----+-----+-----+-----+\n";
        if (Sectors == null) { return " "; }
        string OutStr = "";
        OutStr += XHeader;
        for (int y = 0; y < 8; y++)
        {
            switch (y)
            {
                case 0:
                    OutStr += "^ ";
                    OutStr += Line;
                    break;

                case 4:
                    OutStr += "Y " + Line;
                    break;

                default:
                    OutStr += "| ";
                    OutStr += Line;
                    break;
            }

            OutStr += "| ";
            for (int x = 0; x < 8; x++)
            {
                var CurSector = Sectors.Where(z => z.Coord.X == x && z.Coord.Y == y).FirstOrDefault();
                if (CurSector == null) continue;
                OutStr += "| " + getSectorFormatted(CurSector) + " ";
            }
            OutStr += "|\n";
        }
        OutStr += "v " + Line + "\n";
        return OutStr;
    }

    private string getQuadrantForGalaxyFormatted(QuadrantDTO Quadrant, bool MarkStarship = false)
    {
        string outstr = "";

        if (Quadrant == null) { return "| *** "; }
        if (!Quadrant.HasBeenExplored) { return "| *** "; }

        string ssMarkerStart = " ";
        string ssMarkerEnd = " ";

        if (MarkStarship)
        {
            ssMarkerStart = ">";
            ssMarkerEnd = "<";
        }


        outstr += "|" + ssMarkerStart;
        if (Quadrant.NumberOfEnemies > 0)
        {
            outstr += Quadrant.NumberOfEnemies;
        }
        else
        {
            outstr += Quadrant.NumberOfEnemies;
        }

        outstr += Quadrant.HasStarBase ? "1" : "0";

        outstr += Quadrant.NumberOfStars;
        outstr += ssMarkerEnd;

        return outstr;
    }

    private string getLRSFormatted(List<QuadrantDTO> Quadrants, int ssQX, int ssQY)
    {
        Quadrants = Quadrants.Where(z => z != null).ToList();

        const string XHeader = "  <------- X ------->\n";
        const string Line = "+-----+-----+-----+\n";
        string OutStr = "";
        OutStr += XHeader;
        OutStr += "^ " + Line;
        for (int y = ssQY - 1; y <= ssQY + 1; y++)
        {
            if (y != ssQY - 1)
            {
                OutStr += "| " + Line;
            }

            if (y == ssQY)
            {
                OutStr += "Y ";
            }
            else
            {
                OutStr += "| ";
            }

            for (int x = ssQX - 1; x <= ssQX + 1; x++)
            {
                if (y < 0 || y > 7 || x < 0 || x > 7)
                {
                    OutStr += "| *** ";
                }
                else
                {
                    var CurQuad = Quadrants.Where(z => z.Coord != null && z.Coord.X == x && z.Coord.Y == y).FirstOrDefault();
                    if (CurQuad == null) continue;
                    OutStr += getQuadrantForGalaxyFormatted(CurQuad);
                }
            }
            OutStr += "|\n";
        }
        OutStr += "v " + Line;
        return OutStr;
    }

    private string showHelpMain()
    {
        string stringOut = string.Empty;

        stringOut += "SRS\t\t - Short Range Sensors - Shows what's in the current quadrant\n";
        stringOut += "LRS\t\t - Long Range Sensors - Shows what's in the adjacent quadrants\n";
        stringOut += "SHE ###\t\t - Shield Control - Set the shield level to ###\n";
        stringOut += "LAS ###\t\t - Laser Control - Fire Lasers ### amount\n";
        stringOut += "TOR ###\t\t - Torpedo Control - Fire torpedos in ### direction\n";
        stringOut += "COM ###\t\t - Computer Control - Run ### computer command\n";
        stringOut += "NAV ### $$$\t - Navigation Control - move ship in ### direction $$$ distance\n";
        stringOut += "QUIT\t\t - End the game\n";
        stringOut += "DAM\t\t - Show damages\n";
        stringOut += "HELP\t\t - Show this screen\n";

        return stringOut;
    }

    private string showHelpNavTor(bool isNav)
    {
        string stringOut = string.Empty;

        stringOut += "\n";
        stringOut += "Direction:\n\n";
        stringOut += "  4    3    2\n";
        stringOut += "   `.  :  .'\n";
        stringOut += "     `.:.'\n";
        stringOut += "  5---<*>---1\n"; ;
        stringOut += "     .':`.\n";
        stringOut += "   .'  :  `.\n";
        stringOut += "  6    7    8\n";

        if (isNav)
        {
            stringOut += "\n";
            stringOut += "Space-drive speed 1 to 8, impulse power 0.1 to 0.9";
        }


        return stringOut;
    }

    private string showHelpComputer()
    {
        string stringOut = string.Empty;

        stringOut += "\n";
        stringOut += "COM REC\t\t\t - Computer, Galactic Record\n";
        stringOut += "COM TOR\t\t\t - Computer, Torpedo calculator, shows direction to each enemy in quadrant\n";
        stringOut += "COM NAV ##.## $$.$$\t - Computer, Navigation calculator, ##.## destination quadrant, $$.$$ destination sector\n";
        stringOut += "COM STB\t\t\t - Computer, Starbase navigation calculator, shows direction and distance to starbase\n";
        stringOut += "\t\t\t   in this quadrant\n";
        stringOut += "COM STA\t\t\t - Computer, Status report\n";

        return stringOut;
    }

    private string getGalaxyFormatted()
    {
        if (_game.Galaxy.CurrentQuadrant == null) return CLI_Messages.InternalErrorMessage;

        string outstr = "";

        const string XHeader = "  <---------------------- X ---------------------->\n";
        const string Line = "+-----+-----+-----+-----+-----+-----+-----+-----+\n";

        outstr += XHeader;
        for (int y = 0; y <= 7; y++)
        {
            switch (y)
            {
                case 0:
                    outstr += "^";
                    break;

                case 4:
                    outstr += "Y";
                    break;

                default:
                    outstr += "|";
                    break;
            }
            outstr += " " + Line;
            outstr += "| ";
            for (int x = 0; x <= 7; x++)
            {
                var QUAD = _game.Galaxy.GetQuadrantByCoordDTO(new CoordDTO(x, y));
                if (QUAD == null) continue;
                if (_game.Galaxy.CurrentQuadrant.Coord.isSame(new CoordDTO(x, y)))
                {
                    outstr += getQuadrantForGalaxyFormatted(QUAD, true);
                }
                else
                {
                    outstr += getQuadrantForGalaxyFormatted(QUAD);
                }
            }
            outstr += "|\n";
        }
        outstr += "v " + Line;
        return outstr;
    }

    private string getCoord(CoordDTO coord)
    {
        return $"{coord.X}.{coord.Y}";
    }

    private string runSRS(string[] commands)
    {
        var result = _game.SRS();
        if (result.eCommandResult == eCommandResult.Damaged)
        {
            return CLI_Messages.SRS_Damaged;
        }
        var data = result.getResults<SRS>();
        if (data == null) return CLI_Messages.InternalErrorMessage;
        return getSRSFormatted(data.Sectors) + getRoutineMaintenance(0.1);
    }

    private string runLRS(string[] Commands)
    {
        var result = _game.LRS();
        if (result.eCommandResult == eCommandResult.Damaged)
        {
            return CLI_Messages.LRS_Damaged;
        }
        if (_game.Galaxy.Quadrants == null) return CLI_Messages.InternalErrorMessage;
        if (_game.Galaxy.CurrentQuadrant == null) return CLI_Messages.InternalErrorMessage;
        if (_game.Galaxy.CurrentQuadrant.Coord == null) return CLI_Messages.InternalErrorMessage;
        var cqCoord = _game.Galaxy.CurrentQuadrant.Coord;
        return getLRSFormatted(_game.Galaxy.toQuadrantDTO, cqCoord.X, cqCoord.Y) + getRoutineMaintenance(0.1);
    }

    private string runSHE(string[] Commands)
    {
        if (Commands.Length == 1) return CLI_Messages.SHE_MissingAmount;
        if (!int.TryParse(Commands[1], out int newShieldLevel)) { return CLI_Messages.SHE_InvalidAmount; }
        var sheRES = _game.SHE(newShieldLevel);
        if (sheRES.eCommandResult == eCommandResult.Damaged)
        {
            return CLI_Messages.SHE_Damaged;
        }
        var data = sheRES.getResults<SHE>();
        if (sheRES.eCommandResult == eCommandResult.Error)
        {
            return CLI_Messages.SHE_InvalidAmount + newShieldLevel.ToString() + "\n" + data!.ErrorMessage;
        }
        return CLI_Messages.SHE_Changed + newShieldLevel + getRoutineMaintenance(0.1);
    }

    private string runLAS(string[] Commands)
    {
        if (Commands.Length == 1) return CLI_Messages.LAS_MissingAmount;

        if (!int.TryParse(Commands[1], out int LaserEnergyAmount)) return CLI_Messages.LAS_InvalidAmount;

        var lasRES = _game.LAS(LaserEnergyAmount);

        if (lasRES.eCommandResult == eCommandResult.Damaged) return CLI_Messages.LAS_Damaged;

        if (lasRES.eCommandResult == eCommandResult.No_Enemies_Present)
            return CLI_Messages.LAS_NoEnemies;

        if (lasRES.eCommandResult == eCommandResult.InsufficientInventory)
            return CLI_Messages.LAS_InsufficientEnergy;

        var data = lasRES.getResults<LAS>();
        if (data == null) return CLI_Messages.InternalErrorMessage;


        string OutMsg = string.Empty;
        if (lasRES.eCommandResult == eCommandResult.OK)
        {
            foreach (var objHit in data.ObjectHitReport)
            {
                string objHitCoord = $"{objHit.Coord!.X}.{objHit.Coord.Y}";
                if (objHit.Destroyed)
                {
                    if (OutMsg != string.Empty) OutMsg += "\n";
                    OutMsg += CLI_Messages.LAS_Fired + objHit.UnitHit + CLI_Messages.LAS_Hit + objHitCoord + CLI_Messages.LAS_EnemyDestroyed;
                }
                else
                {
                    if (OutMsg != string.Empty) OutMsg += "\n";
                    OutMsg += CLI_Messages.LAS_Fired + objHit.UnitHit + CLI_Messages.LAS_Hit + objHitCoord + CLI_Messages.LAS_SensorsIndicate + objHit.ShieldRemaining + CLI_Messages.LAS_Remaining;
                }
            }
        }

        return OutMsg + getRoutineMaintenance(0.1);
    }

    private string runTOR(string[] Commands)
    {
        if (Commands.Length == 1)
            return CLI_Messages.TOR_MissingDirection + showHelpNavTor(false);

        if (!double.TryParse(Commands[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double DIR) || !double.IsFinite(DIR))
            return CLI_Messages.TOR_InvalidDirection + Commands[1];

        DIR = Math.Round(DIR, 1);

        if (DIR < 0.1 || DIR >= 9)
            return CLI_Messages.TOR_InvalidDirection + showHelpNavTor(false);

        var torRES = _game.TOR(DIR);

        if (torRES.eCommandResult == eCommandResult.Damaged)
            return CLI_Messages.TOR_Damaged;

        if (torRES.eCommandResult == eCommandResult.No_Enemies_Present)
            return CLI_Messages.TOR_NoEnemies;

        if (torRES.eCommandResult == eCommandResult.InsufficientInventory)
            return CLI_Messages.TOR_Expended;

        if (torRES.eCommandResult == eCommandResult.TOR_Missed)
            return CLI_Messages.TOR_Missed;

        var data = torRES.getResults<TOR>();
        if (data == null)
            return CLI_Messages.InternalErrorMessage;

        string torOutMsg = string.Empty;
        if (torRES.eCommandResult == eCommandResult.OK)
        {
            switch (data.ObjectHitReport.ObjectHit)
            {
                case eObjectHit.Enemy:
                    torOutMsg += CLI_Messages.TOR_Fired + CLI_Messages.TOR_EnemyAtSector + getCoord(data.ObjectHitReport.Coord!) + CLI_Messages.TOR_Destroyed;
                    break;

                case eObjectHit.Star:
                    if (data.ObjectHitReport.Destroyed)
                    {
                        torOutMsg += CLI_Messages.TOR_Fired + CLI_Messages.TOR_StarDestroyed;
                    }
                    else
                    {
                        torOutMsg += CLI_Messages.TOR_Fired + CLI_Messages.TOR_StarAbsorbedTorp;
                    }
                    break;

                case eObjectHit.Starbase:
                    torOutMsg += CLI_Messages.TOR_Fired + CLI_Messages.TOR_StarBaseDestroyed;
                    break;
            }
        }


        return torOutMsg + getRoutineMaintenance(0.1);
    }

    private string runCOM_REC(string[] Commands)
    {
        var resultCPUREC = _game.COM_REC();
        if (resultCPUREC.eCommandResult == eCommandResult.Damaged)
            return CLI_Messages.CPU_Damaged;

        var data = resultCPUREC.getResults<COM_REC>();

        return getGalaxyFormatted();
    }

    private string runCOM_TOR(string[] Commands)
    {
        var cputorRES = _game.COM_TOR();
        if (cputorRES.eCommandResult == eCommandResult.Damaged)
            return CLI_Messages.CPU_Damaged;
        if (cputorRES.eCommandResult == eCommandResult.No_Enemies_Present)
            return CLI_Messages.CPU_TOR_NoEnemies;

        var data = cputorRES.getResults<COM_TOR>();
        if (data == null) return CLI_Messages.InternalErrorMessage;

        string cpuOutMsg = string.Empty;
        foreach (var DTE in data.DirectionToEnemies)
        {
            if (cpuOutMsg != string.Empty) { cpuOutMsg += "\n"; }
            string coord = DTE.Coord.X + "." + DTE.Coord.Y;
            cpuOutMsg += CLI_Messages.CPU_TOR_EnemyAtSector + coord + CLI_Messages.CPU_TOR_IsBearing + DTE.Direction.ToString("0.0");
        }

        return cpuOutMsg;
    }

    private string runCOM_NAV(string[] Commands)
    {
        if (Commands.Length < 6)
            return CLI_Messages.CPU_NAV_Invalid + showHelpComputer();

        if (!int.TryParse(Commands[2], out var QX))
            return CLI_Messages.CPU_NAV_InvalidQX;

        if (QX < 0 || QX >= Global.MaxQuadrantsXY)
            return CLI_Messages.CPU_NAV_InvalidCoord;

        if (!int.TryParse(Commands[3], out var QY))
            return CLI_Messages.CPU_NAV_InvalidQY;

        if (QY < 0 || QY >= Global.MaxQuadrantsXY)
            return CLI_Messages.CPU_NAV_InvalidCoord;

        if (!int.TryParse(Commands[4], out var SX))
            return CLI_Messages.CPU_NAV_InvalidSX;

        if (SX < 0 || SX >= Global.MaxSectorsXY)
            return CLI_Messages.CPU_NAV_InvalidCoord;

        if (!int.TryParse(Commands[5], out var SY))
            return CLI_Messages.CPU_NAV_InvalidSY;

        if (SY < 0 || SY >= Global.MaxSectorsXY)
            return CLI_Messages.CPU_NAV_InvalidCoord;


        var cpunavRES = _game.COM_NAV(new CoordDTO(QX, QY), new CoordDTO(SX, SY));

        if (cpunavRES.eCommandResult == eCommandResult.Damaged)
            return CLI_Messages.CPU_Damaged;

        var data = cpunavRES.getResults<COM_NAV>();
        if (data == null) return CLI_Messages.InternalErrorMessage;


        return CLI_Messages.CPU_NAV_Dir + data.Direction + CLI_Messages.CPU_NAV_Dist + data.Distance;
    }

    private string runCOM_STB(string[] Commands)
    {
        var cpustbRES = _game.COM_STB();
        if (cpustbRES.eCommandResult == eCommandResult.Damaged)
            return CLI_Messages.CPU_Damaged;

        if (cpustbRES.eCommandResult == eCommandResult.CPU_STB_No_Starbase_Present)
            return CLI_Messages.CPU_STB_NoStarBases;

        var data = cpustbRES.getResults<COM_STB>();

        if (data == null)
            return CLI_Messages.InternalErrorMessage;

        return CLI_Messages.CPU_STB_StarbaseFoundInSector +
            data.StarBaseCoord +
            "] " +
            CLI_Messages.CPU_STB_Dir +
            data.Direction.ToString("0.0") +
            CLI_Messages.CPU_STB_Dist +
            data.Distance.ToString("0.0");
    }

    private string runCOM_STA(string[] Commands)
    {
        if (_game.Galaxy.Starship == null) return CLI_Messages.InternalErrorMessage;
        if (_game.Galaxy.Starship.Devices == null) return CLI_Messages.InternalErrorMessage;

        var result = _game.COM_STA();
        if (result.eCommandResult == eCommandResult.Damaged)
            return CLI_Messages.CPU_Damaged;

        string cpuStaOutMsg = string.Empty;

        var data = result.getResults<COM_STA>();
        if (data == null) return CLI_Messages.InternalErrorMessage;

        cpuStaOutMsg += CLI_Messages.CPU_STA_MissionTime + data.MissionTime.ToString("0000.0") + "\n";
        cpuStaOutMsg += CLI_Messages.CPU_STA_Quadrant + data.QuadrantCoord + "\n";
        cpuStaOutMsg += CLI_Messages.CPU_STA_Sector + data.SectorCoord + "\n";
        cpuStaOutMsg += CLI_Messages.CPU_STA_DevicesDamaged + "\n";
        foreach (var device in _game.Galaxy.Starship.Devices)
        {
            if (device == null) continue;
            if (device.Name == null) continue;

            var msg = string.Empty;
            if (device.DamageLevel < 0)
                msg = "Damaged - " + Math.Abs(device.DamageLevel).ToString("#.0") + (Math.Abs(device.DamageLevel) == 1 ? " mission time unit" : " mission time units");
            else
                msg = "Fully Functional";

            cpuStaOutMsg += "\t" + device.Name.PadRight(30) + msg + "\n";
        }
        cpuStaOutMsg += CLI_Messages.CPU_STA_EnemiesRemaining + data.EnemiesRemaining + "\n";
        cpuStaOutMsg += CLI_Messages.CPU_STA_StarbasesRemaining + data.StarBaseRemaining + "\n";
        cpuStaOutMsg += CLI_Messages.CPU_STA_EnergyRemaining + data.EnergyRemaining + "\n";
        cpuStaOutMsg += CLI_Messages.CPU_STA_ShieldLevel + data.ShieldLevel + "\n";
        cpuStaOutMsg += CLI_Messages.CPU_STA_TorpedoRemaining + data.TorpedosRemaining + "\n";
        cpuStaOutMsg += CLI_Messages.CPU_STA_Docked + (data.Docked ? "Docked" : "Not Docked") + "\n";
        cpuStaOutMsg += CLI_Messages.CPU_STA_MissionTimeUnitsRemaining + (data.MissionTimeDeadLine - data.MissionTime).ToString("#.0") + "\n";

        return cpuStaOutMsg;

    }

    private string runCOM(string[] Commands)
    {
        if (Commands.Length < 2)
        {
            return CLI_Messages.CPU_InvalidCommand + showHelpComputer();
        }
        switch (Commands[1].ToUpper().Trim())
        {
            case "REC":
                return runCOM_REC(Commands);

            case "TOR":
                return runCOM_TOR(Commands);

            case "NAV":
                return runCOM_NAV(Commands);

            case "STB":
                return runCOM_STB(Commands);

            case "STA":
                return runCOM_STA(Commands);

            default:
                return CLI_Messages.CPU_InvalidCommand;
        }
    }

    private string runNAV(string[] Commands)
    {
        if (_game.Galaxy.Starship == null) return CLI_Messages.InternalErrorMessage;
        if (_game.Galaxy.CurrentQuadrant == null) return CLI_Messages.InternalErrorMessage;
        if (_game.Galaxy.CurrentQuadrant.Coord == null) return CLI_Messages.InternalErrorMessage;

        if (Commands.Length != 3)
            return CLI_Messages.NAV_InvalidCommand + showHelpNavTor(true);

        if (!double.TryParse(Commands[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double DIR) || !double.IsFinite(DIR))
            return CLI_Messages.NAV_InvalidDIRValue + Commands[1];

        DIR = Math.Round(DIR, 1);

        if (DIR < 0.1 || DIR >= 9)
            return CLI_Messages.NAV_Invalid_DIR + DIR + showHelpNavTor(true);

        if (!double.TryParse(Commands[2], NumberStyles.Float, CultureInfo.InvariantCulture, out double DIST) || !double.IsFinite(DIST))
            return CLI_Messages.NAV_InvalidDISTValue + Commands[2];

        DIST = Math.Round(DIST, 1);

        if (DIST < 0.1 || DIST > 8)
            return CLI_Messages.NAV_Invalid_DIST + DIST + showHelpNavTor(true);

        CoordDTO beforeCoord = _game.Galaxy.CurrentQuadrant.Coord.toDTO;
        var RES = _game.NAV(DIR, DIST);
        CoordDTO afterCoord = _game.Galaxy.CurrentQuadrant.Coord.toDTO;

        if (beforeCoord != afterCoord)
            newQuadrant = true;

        if (RES.eCommandResult == eCommandResult.Damaged && DIST > 0.2)
            return CLI_Messages.NAV_SpaceDriveDamaged;

        var data = RES.getResults<NAV>();
        if (data == null) return
                CLI_Messages.InternalErrorMessage;

        if (RES.eCommandResult == eCommandResult.Error)
            if (data.NavMessage == eNavMessage.InsufficientEnergy_ShieldEnergyAvailable)
                return CLI_Messages.NAV_NotEnoughEnergy_ShieldEnergyAvail;


        if (RES.eCommandResult == eCommandResult.Error &&
                data.NavMessage == eNavMessage.BadInput_OutsideGalaxy)
            return CLI_Messages.NAV_OutsideGalaxy;

        if (RES.eCommandResult == eCommandResult.Error && data.NavMessage == eNavMessage.BadInput_ObjectHit)
        {
            var ssSect = _game.Galaxy.CurrentQuadrant.GetSectorByContents(Global.SectorStarship);
            if (ssSect == null)
                return CLI_Messages.InternalErrorMessage;
            return CLI_Messages.NAV_ImpulseEnginesShutDown + ssSect.Coord.ToString() + CLI_Messages.NAV_BadNavigation;
        }

        if (RES.eCommandResult == eCommandResult.Damaged)
            return CLI_Messages.NAV_SpaceDriveDamaged;

        if (RES.eCommandResult == eCommandResult.OK && DIST < 1)
            return CLI_Messages.NAV_ImpulseEngaged + getRoutineMaintenance(CalculationUtilitiesV2.Distance2Time(data.DistanceTraveled));

        if (RES.eCommandResult == eCommandResult.OK && DIST >= 1)
            return CLI_Messages.NAV_SpaceDriveEngaged + getRoutineMaintenance(CalculationUtilitiesV2.Distance2Time(data.DistanceTraveled));

        return string.Empty;
    }

    private string runDAM(string[] Commands)
    {
        var res = _game.GetDamageReport();
        if (res.eCommandResult != eCommandResult.OK)
            return CLI_Messages.InternalErrorMessage;

        var data = res.getResults<DAM>();
        if (data == null)
            return CLI_Messages.InternalErrorMessage;

        string damOutMsg = string.Empty;
        damOutMsg += CLI_Messages.DAM_DamageReport + "\n";

        foreach (var device in data.Devices)
        {
            if (device == null || device.Name == null)
                return CLI_Messages.InternalErrorMessage;

            string Msg = (device.DamageLevel < 0 ? "Damaged - " + Math.Abs(device.DamageLevel).ToString("0.0") + (Math.Abs(device.DamageLevel) == 1 ? " mission time unit" : " mission time units") : "Fully Functional");
            damOutMsg += "\t" + device.Name.PadRight(30) + Msg + "\n";
        }
        if (data.Devices.Count == 0)
            damOutMsg += $"\t{CLI_Messages.DAM_DamageReport_NoDamages}\n";

        return damOutMsg;
    }

    private string getRoutineMaintenance(double MissionTimeUnits)
    {
        if (_game.Galaxy == null) return CLI_Messages.InternalErrorMessage;
        if (_game.Galaxy.Starship == null) return CLI_Messages.InternalErrorMessage;
        if (_game.Galaxy.CurrentQuadrant == null) return CLI_Messages.InternalErrorMessage;
        if (_game.Galaxy.CurrentQuadrant.Sectors == null) return CLI_Messages.InternalErrorMessage;


        string OutString = string.Empty;
        var RES = _game.RunRoutineMaintenance(MissionTimeUnits);
        var data = RES.getResults<MaintResult>();
        if (data == null) return CLI_Messages.InternalErrorMessage;

        OutString += "\n";

        if (data == null) return String.Empty;
        if (data.DockingStatus == null) return CLI_Messages.InternalErrorMessage;

        if (data.GameStatus != eGameStatus.Normal)
        {
            switch (data.GameStatus)
            {
                case eGameStatus.StarshipDestroyed:
                    GameOver = true;
                    return CLI_Messages.END_MissionFailed_StarshipDestroyed;

                case eGameStatus.RanOutOfEnergy:
                    GameOver = true;
                    return CLI_Messages.END_MissionFailed_RanOutOfEnergy;

                case eGameStatus.RanOutOfTime:
                    GameOver = true;
                    return CLI_Messages.END_MissionFailed_RanOutOfTime;

                case eGameStatus.OutOfEnergyShieldEnergyAvailable:
                    OutString += CLI_Messages.END_EnergyGoneShieldEnergyAvail;
                    break;

                case eGameStatus.MissionOver:
                    GameOver = true;
                    return "\n" + CLI_Messages.END_MissionAccomplished;

                default:
                    break;
            }
        }


        if (!data.DockingStatus.PreviousStatus && data.DockingStatus.CurrentStatus)
        {
            OutString += CLI_Messages.MAINT_Docked + "\n";
        }

        if (data.StarbaseRepairsMissionTimeUnits != null)
        {
            OutString += CLI_Messages.MAINT_TechWaiting + CLI_Messages.MAINT_ItWillTake + Math.Abs(data.StarbaseRepairsMissionTimeUnits.Value) + CLI_Messages.MAINT_MissionTimeUnits + "\n" + CLI_Messages.MAINT_ReplyYES;
            RepairMissionTimeUnits = data.StarbaseRepairsMissionTimeUnits;
            StarbaseRepairsAvailableSW = true;
        }

        if (data.DevicesRepairStatusChanged != null)
        {
            foreach (var Device in data.DevicesRepairStatusChanged)
            {
                if (Device.DamageLevel == 100)
                {
                    OutString += CLI_Messages.MAINT_RepairsComplete + Device.Name + "\n";

                    Device.DamageLevel = 0;
                }
            }
        }

        bool StarshipDestroyedMessageSent = false;

        if (data.EnemiesMoved != null)
        {
            foreach (var enemy in data.EnemiesMoved)
            {
                string OutLine = string.Empty;
                string msg = string.Format(CLI_Messages.MAINT_EnemyMoved, enemy.EnemyCoord_Orig, enemy.EnemyCoord_Dest);
                OutLine += msg;
                OutString += "\n" + OutLine;
            }
        }

        if (data.EnemiesFired != null)
        {
            string OutLine = string.Empty;

            foreach (var EnemyFired in data.EnemiesFired)
            {
                if (StarshipDestroyedMessageSent) { continue; }
                if (OutLine.Trim() != string.Empty) { OutLine += "\n"; }

                if (!data.DockingStatus.CurrentStatus)
                {
                    if (EnemyFired.StarshipDestroyed)
                    {
                        OutLine += string.Format(CLI_Messages.MAINT_EnemyFiredStarshipDestroyed, EnemyFired.EnemyFiredAmount, EnemyFired.EnemyCoord);
                        StarshipDestroyedMessageSent = true;
                    }
                    else
                    {
                        OutLine += string.Format(CLI_Messages.MAINT_EnemyFiredShieldHit, EnemyFired.EnemyFiredAmount, EnemyFired.EnemyCoord, EnemyFired.StarshipNewShieldAmount);
                        if (EnemyFired.DeviceDamaged != null)
                        {
                            OutLine += "\n" + EnemyFired.DeviceDamaged.Name + CLI_Messages.MAINT_DeviceDamaged + Math.Round(Math.Abs(EnemyFired.DeviceDamangedAmount), 1) + CLI_Messages.MAINT_MissionTimeUnitsToRepair;
                        }
                    }
                }
                else
                {
                    OutLine += string.Format(CLI_Messages.MAINT_EnemyFiredProtected, EnemyFired.EnemyFiredAmount, EnemyFired.EnemyCoord, 0) + CLI_Messages.MAINT_StarBaseShieldsProtected;
                }
            }
            OutString += "\n" + OutLine;
        }

        var EnemyList = _game.Galaxy.CurrentQuadrant.Sectors.Where(z => z.Enemy != null).Select(y => y.Enemy).ToList().AsReadOnly();
        if (!data.DockingStatus.CurrentStatus && !StarshipDestroyedMessageSent)
        {
            var ShieldLevel = _game.Galaxy.Starship.ShieldLevel;

            if (ShieldLevel < 50 && EnemyList.Count > 0)
                OutString += "\n" + CLI_Messages.WarningMessageShieldDownInCombatArea;
        }
        if (EnemyList.Count > 0 && newQuadrant)
        {
            OutString += "\n" + CLI_Messages.MAINT_NewQuadrantEnemiesRedAlert;
            newQuadrant = false;
        }

        return OutString;
    }

    public string ExecuteCommandString(string CommandLine)
    {
        if (CommandLine.Trim() == string.Empty) { return ""; }

        string[] Commands = CommandLine.Split(new char[] { ' ', '[', ']', ',', '=', ':' }, StringSplitOptions.RemoveEmptyEntries);
        string Command = Commands[0].ToUpper().Trim();
        string OutString = string.Empty;

        if (Command != "YES" && StarbaseRepairsAvailableSW) { StarbaseRepairsAvailableSW = false; RepairMissionTimeUnits = 0; }  //resets the function to make repairs
        if (Command != "YES" && quitSwitch)
        {
            quitSwitch = false;
            return string.Empty; // returns for the next entry since we're assuming this is a cancellation of quit
        }  //resets when the user says anything besides YES

        switch (Command.Trim())
        {
            case "YES":
                if (!StarbaseRepairsAvailableSW && !quitSwitch) { return CLI_Messages.UnknownCommand + Command; }
                if (StarbaseRepairsAvailableSW && RepairMissionTimeUnits != null)
                {
                    StarbaseRepairsAvailableSW = false;
                    _game.ExecuteStarbaseRepairs(RepairMissionTimeUnits.Value);
                    RepairMissionTimeUnits = 0;
                    return CLI_Messages.MAINT_RepairsMade;
                }
                if (quitSwitch)
                {
                    OutString += CLI_Messages.END_MissionFailed_Quit;
                    GameOver = true;
                }
                break;

            case "SRS":
                return runSRS(Commands);

            case "LRS":
                return runLRS(Commands);

            case "SHE":
                return runSHE(Commands);

            case "LAS":
                return runLAS(Commands);

            case "TOR":
                return runTOR(Commands);

            case "COM":
                return runCOM(Commands);

            case "NAV":
                return runNAV(Commands);

            case "DAM":
                return runDAM(Commands);

            case "DEBUG":
                return _debug.runDEBUG(Commands);

            case "QUIT":
                OutString += "Are you abandoning your mission? (YES/NO)";
                quitSwitch = true;
                break;

            case "HELP":
                return showHelpMain();

            default:
                if (Command.Trim() != "")
                {
                    OutString += CLI_Messages.UnknownCommand;
                    OutString += "\n";
                    OutString += showHelpMain();
                    OutString += "\n";
                }
                return OutString + getRoutineMaintenance(0);
        }
        return OutString;
    }
}