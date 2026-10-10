using System.Diagnostics;
using RobsSpacePatrol.CLI.Library;
using RobsSpacePatrol.DTO.Library;
using RobsSpacePatrol.Library;
using RobsSpacePatrol.Library.Game;
using RobsSpacePatrol.Library.Models;

namespace RobsSpacePatrol.CLI.Library;


public class DebugCommands_Library : IDebug
{
    private IGame _game;

    public DebugCommands_Library(IGame game)
    {
        _game = game;
    }

    private Coord? getCoord(string x, string y)
    {
        if (!int.TryParse(x, out int _x))
            return null;
        if (!int.TryParse(y, out int _y))
            return null;
        return new Coord(_x, _y);
    }

    private string DebugMenu()
    {
        string stringOut = string.Empty;

        stringOut += "BRIEF\n";
        stringOut += "SET\n";
        stringOut += "\tCLEARCQ\n";
        stringOut += "\t\tALL\n";

        stringOut += "\tCURRENTQUADRANT\n";
        stringOut += "\t\tSECTOR\n";
        stringOut += "\t\tENEMY\n";

        stringOut += "\tQUADRANT\n";
        stringOut += "\t\tNUMENEMIES\n";
        stringOut += "\t\tNUMSTARS\n";
        stringOut += "\t\tNUMENEMIES\n";
        stringOut += "\t\tSTARBASE\n";
        stringOut += "\t\tEXPLORED\n";

        stringOut += "\tSTARSHIP\n";
        stringOut += "\t\tENERGY\n";
        stringOut += "\t\tSHIELD\n";
        stringOut += "\t\tTORP\n";
        stringOut += "\t\tDEVICE\n";
        stringOut += "\t\tSECTOR\n";
        stringOut += "\t\tQUADRANT\n";

        stringOut += "\tGALAXY\n";
        stringOut += "\t\tMISSIONTIME\n";
        stringOut += "\t\tMISSIONTIMEDEADLINE\n";



        stringOut += "\n";
        stringOut += "GET\n";

        stringOut += "\tCURRENTQUADRANT\n";
        stringOut += "\t\tSECTOR\n";
        stringOut += "\t\tENEMY\n";

        stringOut += "\tQUADRANT\n";
        stringOut += "\t\tNUMENEMIES\n";
        stringOut += "\t\tNUMSTARS\n";
        stringOut += "\t\tSTARBASE\n";
        stringOut += "\t\tEXPLORED\n";

        stringOut += "\tSTARSHIP\n";
        stringOut += "\t\tENERGY\n";
        stringOut += "\t\tSHIELD\n";
        stringOut += "\t\tTORP\n";
        stringOut += "\t\tDEVICE\n";
        stringOut += "\t\tSECTOR\n";
        stringOut += "\t\tQUADRANT\n";


        stringOut += "\tGALAXY\n";
        stringOut += "\t\tMISSIONTIME\n";
        stringOut += "\t\tMISSIONTIMEDEADLINE\n";


        stringOut += "\n";
        stringOut += "CHECK\n";
        stringOut += "\t\n";
        stringOut += "\n";
        stringOut += "\n";

        return stringOut;
    }

    public string runDEBUG(string[] commands)
    {
        for (int i = 0; i < commands.Count(); i++)
        {
            commands[i] = commands[i].ToUpper();
        }

        if (commands[1] == "CHECK") return CLI_Messages.DebugCommandOK;

        if (commands.Count() < 2)
        {
            return CLI_Messages.DebugErrorInCommand + "\n" + DebugMenu();
        }

        switch (commands[1])
        {
            case "SET":
                return run_SET_DEBUG(commands);

            case "GET":
                return run_GET_DEBUG(commands);

            default:
                return CLI_Messages.DebugUnknownCommand;
        }
    }

    private string getCoord(Coord coord)
    {
        return $"{coord.X}.{coord.Y}";
    }

    private Sector? getSector(string x, string y)
    {
        var coord = getCoord(x, y);
        if (coord == null) return null;
        var targetSector = _game.Galaxy.CurrentQuadrant.GetSectorByCoord(coord);
        if (targetSector == null) return null;
        return targetSector;
    }

    private Quadrant? getQuadrant(string x, string y)
    {
        var coord = getCoord(x, y);
        if (coord == null) return null;
        var targetQuadrant = _game.Galaxy.GetQuadrantByCoord(coord);
        if (targetQuadrant == null) return null;
        return targetQuadrant;
    }



    private string setSectorContents(string[] commands)
    {
        if (commands.Count() < 7) return CLI_Messages.DebugInvalidCommand;
        var targetSector = getSector(commands[4], commands[5]);
        if (targetSector == null) return CLI_Messages.DebugInvalidCommand;
        targetSector.SectorContents = commands[6].ToUpper().Trim();
        return CLI_Messages.DebugCommandOK;
    }

    private string setSectorEnemy(string[] commands)
    {
        if (commands.Count() < 7) return CLI_Messages.DebugInvalidCommand;
        var targetSector = getSector(commands[4], commands[5]);
        if (targetSector == null) return CLI_Messages.DebugInvalidCommand;
        if (!int.TryParse(commands[6], out int shieldLevel)) return CLI_Messages.DebugInvalidCommand;

        var enemy = new Enemy(shieldLevel);

        targetSector.SectorContents = Global.SectorEnemy;
        targetSector.Enemy = enemy;

        return CLI_Messages.DebugCommandOK;
    }

    private string setQuadrantContents(string[] commands)
    {
        if (commands.Count() < 7) return CLI_Messages.DebugInvalidCommand;
        var quadrant = getQuadrant(commands[3], commands[4]);
        if (quadrant == null)
            return CLI_Messages.DebugInvalidCommand;


        if (!int.TryParse(commands[6], out int value)) return CLI_Messages.DebugInvalidCommand;

        switch (commands[5])
        {
            case "NUMENEMIES":
                quadrant.NumberOfEnemies = value;
                return CLI_Messages.DebugCommandOK;

            case "NUMSTARS":
                quadrant.NumberOfStars = value;
                return CLI_Messages.DebugCommandOK;

            case "STARBASE":
                if (value == 1)
                    quadrant.HasStarBase = true;
                else
                    quadrant.HasStarBase = false;
                return CLI_Messages.DebugCommandOK;

            case "EXPLORED":
                if (value == 1)
                    quadrant.HasBeenExplored = true;
                else
                    quadrant.HasBeenExplored = false;
                return CLI_Messages.DebugCommandOK;

            default:
                return CLI_Messages.DebugInvalidCommand;
        }
    }

    private string setStarship(string[] commands)
    {
        if (commands.Count() < 5) return CLI_Messages.DebugInvalidCommand;
        if (_game.Galaxy == null) return CLI_Messages.DebugInvalidCommand;
        if (_game.Galaxy.Starship == null) return CLI_Messages.DebugInvalidCommand;
        if (_game.Galaxy.Starship.Devices == null) return CLI_Messages.DebugInvalidCommand;

        switch (commands[3])
        {
            case "ENERGY":
                if (!int.TryParse(commands[4], out int energyLevel)) return CLI_Messages.DebugInvalidCommand;

                _game.Galaxy.Starship.EnergyLevel = energyLevel;
                return CLI_Messages.DebugCommandOK;

            case "SHIELD":
                if (!int.TryParse(commands[4], out int shield)) return CLI_Messages.DebugInvalidCommand;

                _game.Galaxy.Starship.ShieldLevel = shield;
                return CLI_Messages.DebugCommandOK;

            case "TORP":
                if (!int.TryParse(commands[4], out int torp)) return CLI_Messages.DebugInvalidCommand;

                _game.Galaxy.Starship.TorpedosRemaining = torp;
                return CLI_Messages.DebugCommandOK;

            case "DEVICE":
                if (commands.Count() < 6) return CLI_Messages.DebugInvalidCommand;
                if (!double.TryParse(commands[5], out double damageLvl)) return CLI_Messages.DebugInvalidCommand;

                var device = _game.Galaxy.Starship.Devices.FirstOrDefault(z => z.Name == commands[4]);
                if (device == null) return CLI_Messages.DebugInvalidCommand;
                device.DamageLevel = damageLvl;

                Debug.Assert(_game.Galaxy.Starship.GetDamagedDevices.Count > 0);


                return CLI_Messages.DebugCommandOK;

            case "QUADRANT":
                if (commands.Count() < 6) return CLI_Messages.DebugInvalidCommand;
                var quadrant = getQuadrant(commands[4], commands[5]);
                if (quadrant == null) return CLI_Messages.DebugInvalidCommand;
                Console.WriteLine($"setStarship: quadrant = {getCoord(quadrant!.Coord!)}");
                var sector = _game.Galaxy.CurrentQuadrant.GetSectorByContents(Global.SectorStarship);
                if (quadrant == null) return CLI_Messages.DebugInvalidCommand;
                if (quadrant.Coord == null) return CLI_Messages.DebugInvalidCommand;
                if (sector == null) return CLI_Messages.DebugInvalidCommand;
                _game.Galaxy.MoveStarshipQuadrant(_game.CurrentQuadrantFactory, _game.RandomFactory, quadrant, sector.Coord);



                return CLI_Messages.DebugCommandOK;

            default:
                return CLI_Messages.DebugInvalidCommand;
        }
    }

    private string setGalaxy(string[] commands)
    {
        if (commands.Count() < 5) return CLI_Messages.DebugInvalidCommand;
        switch (commands[3])
        {
            case "MISSIONTIME":
                if (!double.TryParse(commands[4], out double missionTime)) return CLI_Messages.DebugInvalidCommand;
                _game.Galaxy.MissionTime = missionTime;
                return CLI_Messages.DebugCommandOK;

            case "MISSIONTIMEDEADLINE":
                if (!double.TryParse(commands[4], out double missionTimeDeadLine)) return CLI_Messages.DebugInvalidCommand;
                _game.Galaxy.MissionTimeDeadLine = missionTimeDeadLine;
                return CLI_Messages.DebugCommandOK;

            default:
                return CLI_Messages.DebugInvalidCommand;
        }
    }

    private string run_SET_DEBUG(string[] commands)
    {
        if (commands.Count() < 3) return CLI_Messages.DebugInvalidCommand;
        switch (commands[2])
        {
            case "STOPENEMYFIRE":
                _game.Galaxy.debugSet(stopEnemyFire: true);
                return "OK";

            case "SKIPENEMYMOVE":
                if (commands.Count() < 4) return CLI_Messages.DebugInvalidCommand;
                switch (commands[3])
                {
                    case "ON":
                        _game.Galaxy.debugSet(skipEnemyMove: true);
                        return CLI_Messages.DebugCommandOK;

                    case "OFF":
                        _game.Galaxy.debugSet(skipEnemyMove: false);
                        return CLI_Messages.DebugCommandOK;

                    case "FORCE":
                        _game.Galaxy.debugSet(skipEnemyMove: false, forceEnemyMove: true);
                        return CLI_Messages.DebugCommandOK;

                    default:
                        return CLI_Messages.DebugUnknownCommand;
                }
            case "CLEARCQ":
                bool allSW = false;
                if (commands.Length >= 4 && commands[3] == "ALL") allSW = true;

                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        var coord = new Coord(x, y);
                        var targetSector = _game.Galaxy.CurrentQuadrant.GetSectorByCoord(coord);
                        if (targetSector == null) return CLI_Messages.DebugInvalidCommand;
                        if (allSW)
                            targetSector.SectorContents = Global.SectorEmpty;
                        else
                        {
                            if (targetSector.SectorContents != Global.SectorStarship)
                                targetSector.SectorContents = Global.SectorEmpty;
                        }
                    }
                }

                var data = _game.Galaxy.CurrentQuadrant!.Sectors!.Where(z => z.SectorContents != Global.SectorEmpty).ToList();
                if (data.Count > 1)
                    return CLI_Messages.DebugErrorInCommand;

                return CLI_Messages.DebugCommandOK;

            case "CURRENTQUADRANT":
                if (commands.Count() < 4) return CLI_Messages.DebugInvalidCommand;
                switch (commands[3])
                {
                    case "SECTOR":
                        return setSectorContents(commands);

                    case "ENEMY":
                        return setSectorEnemy(commands);

                    default:
                        return CLI_Messages.DebugUnknownCommand;
                }
            case "QUADRANT":
                return setQuadrantContents(commands);

            case "STARSHIP":
                return setStarship(commands);

            case "GALAXY":
                return setGalaxy(commands);

            default:
                return CLI_Messages.DebugInvalidCommand;
        }
    }


    private string getSectorContents(string[] commands)
    {
        if (commands.Count() < 6) return CLI_Messages.DebugInvalidCommand;
        var targetSector = getSector(commands[4], commands[5]);
        if (targetSector == null) return CLI_Messages.DebugInvalidCommand;

        return targetSector.SectorContents;
    }

    private string getSectorEnemy(string[] commands)
    {
        if (commands.Count() < 6) return CLI_Messages.DebugInvalidCommand;
        var targetSector = getSector(commands[4], commands[5]);
        if (targetSector == null) return CLI_Messages.DebugInvalidCommand;
        if (targetSector.Enemy == null) return CLI_Messages.DebugInvalidCommand;

        return targetSector.Enemy.ShieldLevel.ToString();
    }

    private string getQuadrantContents(string[] commands)
    {
        if (commands.Count() < 6) return CLI_Messages.DebugInvalidCommand;
        var quadrant = getQuadrant(commands[3], commands[4]);
        if (quadrant == null) return CLI_Messages.DebugInvalidCommand;

        switch (commands[5])
        {
            case "NUMENEMIES":
                return quadrant.NumberOfEnemies.ToString();

            case "NUMSTARS":
                return quadrant.NumberOfStars.ToString();

            case "STARBASE":
                var valueStrSB = quadrant.HasStarBase ? "1" : "0";
                return valueStrSB.ToString();

            case "EXPLORED":
                var valueStrEX = quadrant.HasBeenExplored ? "1" : "0";
                return valueStrEX.ToString();

            default:
                return CLI_Messages.DebugInvalidCommand;
        }
    }

    private string getStarship(string[] commands)
    {
        if (commands.Count() < 4) return CLI_Messages.DebugInvalidCommand;
        if (_game.Galaxy == null) return CLI_Messages.DebugInvalidCommand;
        if (_game.Galaxy.Starship == null) return CLI_Messages.DebugInvalidCommand;
        if (_game.Galaxy.Starship.Devices == null) return CLI_Messages.DebugInvalidCommand;

        switch (commands[3])
        {
            case "ENERGY":
                var energyLevel = _game.Galaxy.Starship.EnergyLevel;
                return energyLevel.ToString();

            case "SHIELD":
                var shield = _game.Galaxy.Starship.ShieldLevel;
                return shield.ToString();

            case "TORP":
                var torp = _game.Galaxy.Starship.TorpedosRemaining;
                return torp.ToString();

            case "DEVICE":
                if (commands.Count() < 5) return CLI_Messages.DebugInvalidCommand;
                var device = _game.Galaxy.Starship.Devices.FirstOrDefault(z => z.Name == commands[4]);
                if (device == null) return CLI_Messages.DebugInvalidCommand;
                return device.DamageLevel.ToString();

            case "SECTOR":
                var sector = _game.Galaxy.CurrentQuadrant.GetSectorByContents(Global.SectorStarship);
                if (sector == null) return CLI_Messages.DebugInvalidCommand;
                return $"{sector.Coord.X}.{sector.Coord.Y}";

            case "QUADRANT":
                if (_game.Galaxy == null) return CLI_Messages.DebugInvalidCommand;
                if (_game.Galaxy.CurrentQuadrant == null) return CLI_Messages.DebugInvalidCommand;
                if (_game.Galaxy.CurrentQuadrant.Coord == null) return CLI_Messages.DebugInvalidCommand;

                var quad = _game.Galaxy.CurrentQuadrant.Coord;
                return $"{quad.X}.{quad.Y}";

            default:
                return CLI_Messages.DebugInvalidCommand;
        }
    }

    private string getGalaxy(string[] commands)
    {
        if (commands.Count() < 4) return CLI_Messages.DebugInvalidCommand;
        switch (commands[3])
        {
            case "MISSIONTIME":
                var sd = _game.Galaxy.MissionTime;
                return sd.ToString();

            case "MISSIONTIMEDEADLINE":
                var sddl = _game.Galaxy.MissionTimeDeadLine;
                return sddl.ToString();

            default:
                return CLI_Messages.DebugInvalidCommand;
        }
    }

    private string run_GET_DEBUG(string[] commands)
    {
        if (commands.Count() < 3) return CLI_Messages.DebugInvalidCommand;

        switch (commands[2])
        {
            case "CURRENTQUADRANT":
                if (commands.Count() < 4) return CLI_Messages.DebugInvalidCommand;
                switch (commands[3])
                {
                    case "SECTOR":
                        return getSectorContents(commands);

                    case "ENEMY":
                        return getSectorEnemy(commands);

                    default:
                        return CLI_Messages.DebugUnknownCommand;
                }
            case "QUADRANT":
                return getQuadrantContents(commands);

            case "STARSHIP":
                return getStarship(commands);

            case "GALAXY":
                return getGalaxy(commands);

            default:
                return CLI_Messages.DebugInvalidCommand;
        }
    }
}