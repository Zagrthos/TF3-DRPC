using Tf3Drpc.Core.Enums;
using Tf3Drpc.Core.Resources;

namespace Tf3Drpc.Core.Extensions;

internal static class EnumExtensions
{
    extension(OwnText value)
    {
        /// <summary>
        /// The key of this text in the strongly typed resources.
        /// </summary>
        public string ResourceKey
            => value switch
            {
                OwnText.StartingTheGame => nameof(Texts.StartingTheGame),
                OwnText.InTheMainMenu => nameof(Texts.InTheMainMenu),
                OwnText.Playing => nameof(Texts.Playing),
                OwnText.Language => nameof(Texts.Language),
                OwnText.SameAsTheGame => nameof(Texts.SameAsTheGame),
                OwnText.Settings => nameof(Texts.Settings),
                OwnText.Exit => nameof(Texts.Exit),
                OwnText.NotWritable => nameof(Texts.NotWritable),
                OwnText.Translocated => nameof(Texts.Translocated),
                OwnText.GameNotRunning => nameof(Texts.GameNotRunning),
                OwnText.DiscordConnected => nameof(Texts.DiscordConnected),
                OwnText.DiscordNotConnected => nameof(Texts.DiscordNotConnected),
                OwnText.StatusAppearance => nameof(Texts.StatusAppearance),
                OwnText.PresetCareer => nameof(Texts.PresetCareer),
                OwnText.PresetNetwork => nameof(Texts.PresetNetwork),
                OwnText.PresetEra => nameof(Texts.PresetEra),
                OwnText.PresetAlternating => nameof(Texts.PresetAlternating),
                OwnText.PresetMinimal => nameof(Texts.PresetMinimal),
                OwnText.InGame => nameof(Texts.InGame),
                OwnText.PresetCustom => nameof(Texts.PresetCustom),
                OwnText.TemplateFirstLine => nameof(Texts.TemplateFirstLine),
                OwnText.TemplateSecondLine => nameof(Texts.TemplateSecondLine),
                OwnText.TemplateHoverText => nameof(Texts.TemplateHoverText),
                OwnText.Preview => nameof(Texts.Preview),
                OwnText.PreviewExample => nameof(Texts.PreviewExample),
                OwnText.Placeholders => nameof(Texts.Placeholders),
                OwnText.PrivateHint => nameof(Texts.PrivateHint),
                OwnText.PlaceholderRank => nameof(Texts.PlaceholderRank),
                OwnText.PlaceholderMode => nameof(Texts.PlaceholderMode),
                OwnText.PlaceholderDate => nameof(Texts.PlaceholderDate),
                OwnText.PlaceholderMonth => nameof(Texts.PlaceholderMonth),
                OwnText.PlaceholderYear => nameof(Texts.PlaceholderYear),
                OwnText.PlaceholderClimate => nameof(Texts.PlaceholderClimate),
                OwnText.PlaceholderLines => nameof(Texts.PlaceholderLines),
                OwnText.PlaceholderStations => nameof(Texts.PlaceholderStations),
                OwnText.PlaceholderTrains => nameof(Texts.PlaceholderTrains),
                OwnText.PlaceholderVehicles => nameof(Texts.PlaceholderVehicles),
                OwnText.PlaceholderPassengers => nameof(Texts.PlaceholderPassengers),
                OwnText.PlaceholderCargo => nameof(Texts.PlaceholderCargo),
                OwnText.PlaceholderCompanyValue => nameof(Texts.PlaceholderCompanyValue),
                OwnText.PlaceholderBalance => nameof(Texts.PlaceholderBalance),
                OwnText.PlaceholderSaveName => nameof(Texts.PlaceholderSaveName),
                OwnText.StoreButton => nameof(Texts.StoreButton),
                OwnText.ShowStoreButton => nameof(Texts.ShowStoreButton),
                OwnText.StoreButtonHint => nameof(Texts.StoreButtonHint),
                OwnText.LogNotWritable => nameof(Texts.LogNotWritable),
                OwnText.Theme => nameof(Texts.Theme),
                OwnText.ThemeSystem => nameof(Texts.ThemeSystem),
                OwnText.ThemeLight => nameof(Texts.ThemeLight),
                OwnText.ThemeDark => nameof(Texts.ThemeDark),
                _ => string.Empty
            };
    }

    extension(GameMode value)
    {
        /// <summary>
        /// The msgid of the game's own text for this mode, which is also its English text unless the mode has a separate English text.
        /// </summary>
        public string TranslationKey
            => value switch
            {
                GameMode.Tycoon => "MOD_TYCOON_NAME",
                GameMode.Sandbox => "MOD_SANDBOX_NAME",
                GameMode.MapEditor => "Map Editor",
                _ => string.Empty
            };

        /// <summary>
        /// The English text of this mode.
        /// </summary>
        public string EnglishText
            => value switch
            {
                GameMode.Tycoon => "Tycoon",
                GameMode.Sandbox => "Sandbox Mode",
                GameMode.MapEditor => "Map Editor",
                _ => string.Empty
            };
    }

    extension(GameLabel value)
    {
        /// <summary>
        /// The msgid of the game's own text for this label, which is also its English text.
        /// </summary>
        public string TranslationKey
            => value switch
            {
                GameLabel.Campaign => "Campaign",
                GameLabel.Paused => "Paused",
                GameLabel.Loading => "Loading...",
                GameLabel.Lines => "Lines",
                GameLabel.Vehicles => "Vehicles",
                GameLabel.Stations => "Stations",
                GameLabel.Trains => "Trains",
                GameLabel.Passengers => "Passengers",
                GameLabel.Cargo => "Cargo",
                GameLabel.Year => "Year",
                GameLabel.Climate => "Climate",
                GameLabel.CompanyValue => "Company Value",
                GameLabel.NewGame => "New Game",
                _ => string.Empty
            };
    }

    extension(PresencePlaceholder value)
    {
        /// <summary>
        /// The case-sensitive name of this placeholder in a template.
        /// </summary>
        public string Name
            => value switch
            {
                PresencePlaceholder.Rank => "rank",
                PresencePlaceholder.Mode => "mode",
                PresencePlaceholder.Date => "date",
                PresencePlaceholder.Month => "month",
                PresencePlaceholder.Year => "year",
                PresencePlaceholder.Climate => "climate",
                PresencePlaceholder.Lines => "lines",
                PresencePlaceholder.Stations => "stations",
                PresencePlaceholder.Trains => "trains",
                PresencePlaceholder.Vehicles => "vehicles",
                PresencePlaceholder.Passengers => "passengers",
                PresencePlaceholder.Cargo => "cargo",
                PresencePlaceholder.CompanyValue => "companyValue",
                PresencePlaceholder.Balance => "balance",
                PresencePlaceholder.SaveName => "saveName",
                _ => string.Empty
            };

        /// <summary>
        /// Tells whether this value is private, such as money or the name of the save game.
        /// </summary>
        public bool IsPrivate
            => value is PresencePlaceholder.CompanyValue or PresencePlaceholder.Balance or PresencePlaceholder.SaveName;

        /// <summary>
        /// The own text that explains this placeholder.
        /// </summary>
        public OwnText Description
            => value switch
            {
                PresencePlaceholder.Rank => OwnText.PlaceholderRank,
                PresencePlaceholder.Mode => OwnText.PlaceholderMode,
                PresencePlaceholder.Date => OwnText.PlaceholderDate,
                PresencePlaceholder.Month => OwnText.PlaceholderMonth,
                PresencePlaceholder.Year => OwnText.PlaceholderYear,
                PresencePlaceholder.Climate => OwnText.PlaceholderClimate,
                PresencePlaceholder.Lines => OwnText.PlaceholderLines,
                PresencePlaceholder.Stations => OwnText.PlaceholderStations,
                PresencePlaceholder.Trains => OwnText.PlaceholderTrains,
                PresencePlaceholder.Vehicles => OwnText.PlaceholderVehicles,
                PresencePlaceholder.Passengers => OwnText.PlaceholderPassengers,
                PresencePlaceholder.Cargo => OwnText.PlaceholderCargo,
                PresencePlaceholder.CompanyValue => OwnText.PlaceholderCompanyValue,
                PresencePlaceholder.Balance => OwnText.PlaceholderBalance,
                PresencePlaceholder.SaveName => OwnText.PlaceholderSaveName,
                _ => OwnText.PlaceholderSaveName
            };
    }

    extension(PresencePreset value)
    {
        /// <summary>
        /// The value of this preset in the settings file.
        /// </summary>
        public string SettingValue
            => value switch
            {
                PresencePreset.Career => "career",
                PresencePreset.Network => "network",
                PresencePreset.Era => "era",
                PresencePreset.Alternating => "alternating",
                PresencePreset.Minimal => "minimal",
                PresencePreset.Custom => "custom",
                _ => string.Empty
            };

        /// <summary>
        /// The own text that names this preset.
        /// </summary>
        public OwnText DisplayText
            => value switch
            {
                PresencePreset.Career => OwnText.PresetCareer,
                PresencePreset.Network => OwnText.PresetNetwork,
                PresencePreset.Era => OwnText.PresetEra,
                PresencePreset.Alternating => OwnText.PresetAlternating,
                PresencePreset.Custom => OwnText.PresetCustom,
                _ => OwnText.PresetMinimal
            };
    }

    extension(PresencePreset)
    {
        /// <summary>
        /// Finds the preset of a settings value (ordinal, case-sensitive).
        /// </summary>
        public static bool TryParseSetting(string? setting, out PresencePreset preset)
        {
            PresencePreset? parsed = setting switch
            {
                "career" => PresencePreset.Career,
                "network" => PresencePreset.Network,
                "era" => PresencePreset.Era,
                "alternating" => PresencePreset.Alternating,
                "minimal" => PresencePreset.Minimal,
                "custom" => PresencePreset.Custom,
                _ => null
            };

            preset = parsed ?? PresencePreset.Career;

            return parsed is not null;
        }
    }

    extension(ThemeMode value)
    {
        /// <summary>
        /// The value of this theme in the settings file.
        /// </summary>
        public string SettingValue
            => value switch
            {
                ThemeMode.System => "system",
                ThemeMode.Light => "light",
                ThemeMode.Dark => "dark",
                _ => string.Empty
            };

        /// <summary>
        /// The own text that names this theme.
        /// </summary>
        public OwnText DisplayText
            => value switch
            {
                ThemeMode.Light => OwnText.ThemeLight,
                ThemeMode.Dark => OwnText.ThemeDark,
                _ => OwnText.ThemeSystem
            };
    }

    extension(ThemeMode)
    {
        /// <summary>
        /// Finds the theme of a settings value (ordinal, case-sensitive).
        /// </summary>
        public static bool TryParseSetting(string? setting, out ThemeMode theme)
        {
            ThemeMode? parsed = setting switch
            {
                "system" => ThemeMode.System,
                "light" => ThemeMode.Light,
                "dark" => ThemeMode.Dark,
                _ => null
            };

            theme = parsed ?? ThemeMode.System;

            return parsed is not null;
        }
    }
}
