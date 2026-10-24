namespace Tf3Drpc.Core.Constants;

/// <summary>
/// Limits and fixed values of the gettext reader and the game texts.
/// </summary>
internal static class TranslationConstants
{
    public const int MaxMoFileBytes = 32 * 1024 * 1024;

    public const uint MoMagicLittleEndian = 0x950412de;

    public const uint MoMagicBigEndian = 0xde120495;

    public const int MoHeaderBytes = 28;

    public const int MoTableEntryBytes = 8;

    public const int MaxInitialEntryCapacity = 16384;

    public const byte NulByte = 0;

    public const byte ContextSeparatorByte = 4;

    public const int RankCount = 15;

    public const int MissionCount = 8;

    public const string ClimatePrefix = "::/climates/";

    public const string ClimateSuffix = ".clima";
}
