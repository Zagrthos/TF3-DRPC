using System.Globalization;
using System.Threading;

using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Status;
using Tf3Drpc.Core.Structs;
using Tf3Drpc.Core.Translation;

namespace Tf3Drpc.Core.Presence;

/// <summary>
/// Builds a preview of the presence from the last status seen, or from an example when there was none yet.
/// </summary>
/// <remarks>
/// The monitor loop remembers, the UI thread builds.
/// </remarks>
public sealed class PresencePreview
{
    // The first status line of docs/log-samples/session.log.
    private static readonly ModStatus ExampleStatus = new()
    {
        Version = 1,
        SaveName = "Archipelago2",
        IsMapEditor = false,
        IsSandbox = false,
        IsTycoon = false,
        Year = 1905,
        Month = 9,
        Day = 30,
        Speedup = 4,
        Rank = 2,
        Experience = 2673,
        Balance = 14422756,
        InfiniteMoney = false,
        Loan = 0,
        PassengersTransported = 3466,
        CargoTransported = 9424,
        CompanyTotalAssets = 55956242,
        CompanyDebt = 0,
        CompanyRailVehicles = 0,
        CompanyTrams = 54,
        CompanyRoadVehicles = 20,
        CompanyAircrafts = 0,
        CompanyShips = 44,
        CompanyNumberOfLines = 44,
        CompanyTotalStations = 74,
        Climate = "::/climates/tropical/tropical.clima",
        Economy = "::/economy/tropic.eco"
    };

    private ModStatus? _status;

    /// <summary>
    /// Keeps the status of a snapshot; a snapshot without status (the main menu, for example) leaves the last one in place.
    /// </summary>
    public void Remember(in GameStateSnapshotStruct snapshot)
    {
        ModStatus? status = snapshot.Status;

        if (status is not null)
            Volatile.Write(ref _status, status);
    }

    /// <summary>
    /// Builds the presence while playing, without pause and timer.
    /// </summary>
    /// <param name="culture">The culture of the texts and numbers.</param>
    /// <param name="gameTexts">The game's own texts.</param>
    /// <param name="options">The preset, rotation step and templates.</param>
    /// <param name="usesExample">Tells whether no status was seen yet, so that the example values were used.</param>
    public PresenceContentStruct Build(CultureInfo culture, GameTexts gameTexts, in PresenceOptionsStruct options, out bool usesExample)
    {
        ModStatus? remembered = Volatile.Read(ref _status);
        usesExample = remembered is null;
        GameStateSnapshotStruct snapshot = new() { State = GameState.Playing, Status = remembered ?? ExampleStatus };

        _ = PresenceBuilder.TryBuild(snapshot, culture, gameTexts, options, out PresenceContentStruct content);

        return content;
    }
}
