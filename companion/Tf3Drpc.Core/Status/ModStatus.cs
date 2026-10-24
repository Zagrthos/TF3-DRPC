using System.Text.Json.Serialization;

namespace Tf3Drpc.Core.Status;

/// <summary>
/// The mod's status line (docs/status-line.md); every key is optional.
/// </summary>
internal sealed record ModStatus
{
    [JsonPropertyName("v")]
    public long? Version { get; init; }

    [JsonPropertyName("timestamp")]
    public long? Timestamp { get; init; }

    [JsonPropertyName("saveName")]
    public string? SaveName { get; init; }

    [JsonPropertyName("isMapEditor")]
    public bool? IsMapEditor { get; init; }

    [JsonPropertyName("isSandbox")]
    public bool? IsSandbox { get; init; }

    [JsonPropertyName("isTycoon")]
    public bool? IsTycoon { get; init; }

    [JsonPropertyName("year")]
    public long? Year { get; init; }

    [JsonPropertyName("month")]
    public long? Month { get; init; }

    [JsonPropertyName("day")]
    public long? Day { get; init; }

    [JsonPropertyName("timeOfDaySec")]
    public double? TimeOfDaySec { get; init; }

    [JsonPropertyName("speedup")]
    public long? Speedup { get; init; }

    [JsonPropertyName("rank")]
    public long? Rank { get; init; }

    [JsonPropertyName("rankName")]
    public string? RankName { get; init; }

    [JsonPropertyName("experience")]
    public long? Experience { get; init; }

    [JsonPropertyName("balance")]
    public long? Balance { get; init; }

    [JsonPropertyName("infiniteMoney")]
    public bool? InfiniteMoney { get; init; }

    [JsonPropertyName("loan")]
    public long? Loan { get; init; }

    [JsonPropertyName("passengersTransported")]
    public long? PassengersTransported { get; init; }

    [JsonPropertyName("cargoTransported")]
    public long? CargoTransported { get; init; }

    [JsonPropertyName("company_totalScore")]
    public double? CompanyTotalScore { get; init; }

    [JsonPropertyName("company_totalAssets")]
    public double? CompanyTotalAssets { get; init; }

    [JsonPropertyName("company_debt")]
    public double? CompanyDebt { get; init; }

    [JsonPropertyName("company_railVehicles")]
    public long? CompanyRailVehicles { get; init; }

    [JsonPropertyName("company_trams")]
    public long? CompanyTrams { get; init; }

    [JsonPropertyName("company_roadVehicles")]
    public long? CompanyRoadVehicles { get; init; }

    [JsonPropertyName("company_aircrafts")]
    public long? CompanyAircrafts { get; init; }

    [JsonPropertyName("company_ships")]
    public long? CompanyShips { get; init; }

    [JsonPropertyName("company_numberOfLines")]
    public long? CompanyNumberOfLines { get; init; }

    [JsonPropertyName("company_totalStations")]
    public long? CompanyTotalStations { get; init; }

    [JsonPropertyName("company_railStations")]
    public long? CompanyRailStations { get; init; }

    [JsonPropertyName("company_tramStations")]
    public long? CompanyTramStations { get; init; }

    [JsonPropertyName("company_roadStations")]
    public long? CompanyRoadStations { get; init; }

    [JsonPropertyName("company_aircraftStations")]
    public long? CompanyAircraftStations { get; init; }

    [JsonPropertyName("company_shipStations")]
    public long? CompanyShipStations { get; init; }

    [JsonPropertyName("company_trackTotalLength")]
    public double? CompanyTrackTotalLength { get; init; }

    [JsonPropertyName("company_trackElectricLength")]
    public double? CompanyTrackElectricLength { get; init; }

    [JsonPropertyName("company_roadTotalLength")]
    public double? CompanyRoadTotalLength { get; init; }

    [JsonPropertyName("company_bridgeTotalLength")]
    public double? CompanyBridgeTotalLength { get; init; }

    [JsonPropertyName("company_tunnelTotalLength")]
    public double? CompanyTunnelTotalLength { get; init; }

    [JsonPropertyName("company_suppliedTowns")]
    public long? CompanySuppliedTowns { get; init; }

    [JsonPropertyName("company_connectedIndustries")]
    public long? CompanyConnectedIndustries { get; init; }

    [JsonPropertyName("company_topSpeed")]
    public double? CompanyTopSpeed { get; init; }

    [JsonPropertyName("company_topLength")]
    public double? CompanyTopLength { get; init; }

    [JsonPropertyName("cloudCoverage")]
    public double? CloudCoverage { get; init; }

    [JsonPropertyName("climate")]
    public string? Climate { get; init; }

    [JsonPropertyName("economy")]
    public string? Economy { get; init; }
}
