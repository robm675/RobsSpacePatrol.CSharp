# Rob's Space Patrol — C#

A command-line space strategy game and personal learning project. Navigate an
8-by-8 galaxy, manage energy and shields, and defeat enemy ships before the
mission deadline. Each quadrant contains an 8-by-8 sector grid.

I independently wrote this C# implementation, inspired by classic text-based
space games. I later ported my own implementation to other languages.

## Requirements

- Windows with the .NET 10 SDK. The checked-in `global.json` selects SDK
  10.0.401 and allows later 10.0.4xx patches.
- PowerShell for the commands below.
- Internet access on the first restore for public NuGet dependencies.

Visual Studio is optional. The repository includes its private
`RobsSpacePatrol.DTO` and `RobsSpacePatrol.Library` packages under
`local-packages`, so no private package feed or separate CoreLogic checkout is
required.

## Build and run

From the repository directory:

```powershell
dotnet restore .\RobsSpacePatrol.CSharp.sln
dotnet build .\RobsSpacePatrol.CSharp.sln --configuration Release --no-restore
dotnet run --project .\RobsSpacePatrol.CLI\RobsSpacePatrol.CLI.csproj --configuration Release
```

## Tests

```powershell
dotnet test .\RobsSpacePatrol.CSharp.sln --configuration Release
```

The test project uses xUnit and Moq. Its deterministic fixtures provide
specified random values where game behavior depends on randomness.

## Controls

Commands are case-insensitive. Type `HELP` for the main command list.

| Command | Action |
| --- | --- |
| `SRS` / `LRS` | Short-range / long-range scan |
| `SHE 500` | Set shields to 500, transferring energy between the ship and shields |
| `LAS 100` | Fire lasers using 100 energy |
| `TOR 1` | Fire a torpedo in direction 1 |
| `NAV 1 0.1` | Move in direction 1 by 0.1 quadrant widths |
| `COM STA` | Mission time, deadline, resources, and remaining enemies |
| `COM REC` | Recorded galaxy information |
| `COM TOR` / `COM STB` | Directions to local enemies / a local starbase |
| `COM NAV 2 3 4 5` | Calculate a course to quadrant (2,3), sector (4,5) |
| `DAM` | Device damage report |
| `QUIT`, then `YES` | Abandon the mission |

Directions run around a compass: 1 east, 3 north, 5 west, 7 south, with
fractional directions between them. Coordinates range from 0 to 7. Navigation
distance ranges from 0.1 to 8 quadrant widths; damaged engines restrict travel.
Sector symbols are `SSS` (Starship), `XXX` (Enemy), `BBB` (Starbase), and `***`
(Star). Press Ctrl+C to interrupt the process.

A short example session:

```text
HELP
COM STA
SHE 500
COM TOR
NAV 1 0.1
QUIT
YES
```

Galaxies and combat outcomes vary between games.

## Current rules and limitations

- A new mission's time allowance is based on the number of enemies. Reaching
  the exact deadline is allowed; exceeding it fails the mission.
- Navigation charges time and energy for actual traveled distance. Scans,
  shield changes, and weapon actions can also advance time, and enemies may
  attack during routine maintenance.
- Detailed obstacle movement occurs within the current quadrant. Inter-quadrant
  travel generates the destination without simulating every intermediate
  quadrant.
- Device repairs round damage to one decimal place after each maintenance step.
  Very small fractional repair increments can therefore be lost; this is an
  accepted gameplay rule.
- The `DEBUG` command exists for manual testing and can create states that
  ordinary gameplay would not produce. Game balance remains a learning-project
  limitation.

## Project layout

- `RobsSpacePatrol.CLI/`: executable command loop and game startup.
- `RobsSpacePatrol.CLI.Library/`: command parsing, formatting, and messages.
- `RobsSpacePatrol.CLI.Library.Tests/`: xUnit tests and deterministic fixtures.
- `local-packages/`: private CoreLogic NuGet packages required by this solution.
- `NuGet.config`: restores private packages locally and public dependencies from
  NuGet.org.
- `global.json`: pins the .NET SDK feature band used by the repository.

The CoreLogic source is maintained separately. Its compiled private packages
are intentionally committed here so this repository can be cloned, built, and
tested independently. When CoreLogic changes, its package version and every
corresponding `PackageReference` must be updated together.

## Authorship, inspiration, and license

The original C# game is my own implementation. It was inspired by classic
text-based space games, including the historical Star Trek game associated
with Mike Mayfield; code was not copied from that project. This acknowledgment
describes inspiration, not affiliation or endorsement.

My code is distributed under the [MIT license](LICENSE), copyright 2026 Rob.
NuGet dependencies remain subject to their respective licenses.
