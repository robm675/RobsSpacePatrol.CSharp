using RobsSpacePatrol.CLI.Library;
using RobsSpacePatrol.Library;
using RobsSpacePatrol.Library.Game;
using static RobsSpacePatrol.DTO.Library.Result;

namespace RobsSpacePatrol.CLI;

public class GameExecutor
{
    readonly ICommandLine _command;
    readonly IDebug _debug;
    readonly IGame _game;

    public GameExecutor(ICommandLine command, IDebug debug, IGame game)
    {
        this._command = command;
        this._debug = debug;
        this._game = game;
    }

    private double? RepairMissionTimeUnits { get; set; }
    private Action<double>? MakeRepairs { get; set; }
    private bool quitSwitch;
    private bool newQuadrant = false;

    public bool GameOver => _command.GameOver;

    private string showHelpMain()
    {
        string stringOut = string.Empty;

        stringOut += "SRS\t\t - Short Range Sensors - Shows what's in the current quadrant\n";
        stringOut += "LRS\t\t - Long Range Sensors - Shows what's in the adjacent quadrants\n";
        stringOut += "SHE ###\t\t - Shield Control - Set the shield level to ###\n";
        stringOut += "LAS ###\t\t - Laser Control - Fire Lasers ### amount\n";
        stringOut += "TOR ###\t\t - Torpedo Control - Fire torpedos in ### direction\n";
        stringOut += "COM ###\t\t - Computer Control - Run ### computer _command\n";
        stringOut += "NAV ### $$$\t - Navigation Control - move ship in ### direction $$$ distance\n";
        stringOut += "QUIT\t\t - End the game\n";
        stringOut += "LSG\t\t - Lists existing save games\n";
        stringOut += "SAVEGAME XXX\t - Save the game with XXX description\n";
        stringOut += "LOADGAME XXX\t - Load the game with XXX description\n";
        stringOut += "DAM\t\t - Show damages\n";
        stringOut += "HELP\t\t - Show this screen\n";

        return stringOut;
    }

    public string IntroMessage()
    {
        string OutMessage = string.Empty;

        var comSTARes = _game.COM_STA();
        var data = comSTARes.getResults<COM_STA>();
        if (data == null) return CLI_Messages.InternalErrorMessage;

        double TotalMissionTimeUnits = data.MissionTimeDeadLine - data.MissionTime;

        OutMessage += "Hello Captain!\n";
        OutMessage += "\n";
        OutMessage += "Mission:  Destroy " + data.EnemiesRemaining + " enemies in less than " + TotalMissionTimeUnits.ToString("0.0") + " mission time units.\n";
        OutMessage += "There are " + data.StarBaseRemaining + " starbases available to repair and refuel your ship.\n";
        OutMessage += "\n";
        OutMessage += "Good luck!\n";


        if (_game.Galaxy.CurrentQuadrant!.Sectors!.Any(z => z.Enemy != null))
        {
            OutMessage += "\n";
            OutMessage += "\n";
            OutMessage += CLI_Messages.WarningMessageShieldDownInCombatArea;
        }

        return OutMessage;
    }

    public string ExecuteCommandString(string commands)
    {
        return _command.ExecuteCommandString(commands);
    }
}
