
# Cargo, names, people and sounds

Some content types are small configuration files without a model or construction of their own: cargo types, languages, name lists, character models, music playlists and sound sets. This page shows each one with a file from the base game and links the official wiki page that describes it.

Paths below are quoted from the unpacked base game, where each content archive became a folder of the same name. That is why they look doubled, for example `base/content/cargos/iron_ore/iron_ore/iron_ore.cargo.lua`; the game itself addresses that file as `/cargos/iron_ore/iron_ore.cargo` (see [Mod structure](mod-structure.md#how-the-zips-map-to-folders)). A mod puts the same files under its own `content/` folder.

| Content | File type | Base game folder | Wiki page |
|---|---|---|---|
| Cargo types | `.cargo.lua` | `content/cargos/<cargo>/` | [Cargos](https://wiki.transportfever3.com/doku.php?id=modding:misc:cargo) |
| Cargo classes | `.cargoclass.lua` | `content/cargos/classes/` | [Cargos](https://wiki.transportfever3.com/doku.php?id=modding:misc:cargo) |
| Cargo model formats | `.cmf.lua` | `content/cargos/formats/` | [Cargos](https://wiki.transportfever3.com/doku.php?id=modding:misc:cargo) |
| Languages | `.lang.lua` | `content/locale/` | [Localizations](https://wiki.transportfever3.com/doku.php?id=modding:misc:localizations) |
| Mod translations | `strings.json` | next to `mod.json` | [Mod definition](https://wiki.transportfever3.com/doku.php?id=modding:general:moddefinition) |
| Name sets | `.names.lua` | `content/names/<set>/` | [Names](https://wiki.transportfever3.com/doku.php?id=modding:misc:names) |
| Characters | `.mdl` with `metadata.person` | `content/characters/` | [People](https://wiki.transportfever3.com/doku.php?id=modding:misc:people) |
| Playlists | `.plist.lua` | `content/music/<playlist>/` | [Playlists](https://wiki.transportfever3.com/doku.php?id=modding:misc:playlists) |
| Sound sets | `.snd.lua` | `sound/` folders next to the models | [Sound sets](https://wiki.transportfever3.com/doku.php?id=modding:misc:soundsets) |

Like other resource files, each of these is a Lua file with a `data()` function that returns a table. Display names are wrapped in `_()` so they can be translated (see [Translations](#translations)).

## Cargo

Cargo has three layers. A cargo type is one good, such as iron ore. Cargo classes group types with common properties, such as `BULK`. Cargo model formats are standard bounding boxes that let a vehicle or warehouse show the right model for whatever it carries.

### Cargo types

The base game has 37 cargo types, one folder each under `base/content/cargos/`. From `base/content/cargos/iron_ore/iron_ore/iron_ore.cargo.lua`:

```lua
function data()
return
	{
		name = _("Iron Ore"),
		weightFactor = 1.0,
		order = 210,
		icon = "iron_ore.tga",
		cargoClasses = { "UNIVERSAL", "BULK" },
		categoryList = {
			categories = {
				"temperate.eco",
				"dry.eco",
				"tropic.eco",
				"subarctic.eco"
			}
		},
		scriptRef = {
			fileName = "::/economy/cargotype.script@incomeFactors.constantFn",
			params = {
				initialPrice = 4,
				finalPrice = 3.2,
				decayRate = 0.005,
				decayMod = 1,
				decayStart = 200,
			}
		},
		color = { 0.94, 0.53, 0.31},
		loadSpeedFactor = 0.0625,
		timeToDeliverInSeconds = 4500,
	}
end
```

| Field | Meaning (wiki) |
|---|---|
| `name` | Display name, translatable. |
| `weightFactor` | Density. 1.0 for heavy cargo, lower for light cargo; passengers use `1/6`. |
| `order` | Sort position in cargo lists. |
| `icon` | Cargo icon. Icons are 50 px high at double resolution (`@2x.tga`), flat, with a black outline. |
| `cargoClasses` | Class tags the type belongs to. |
| `categoryList.categories` | Economies (`.eco`) in which the type exists, see [Economies](game-mechanics.md#economies). |
| `scriptRef` | Income function and its `params` (see below). |
| `color` | RGB color (0 to 1) for charts with several cargo types. |
| `loadSpeedFactor` | Loading speed per item. Passengers have 1, other cargo 0.0625. |
| `timeToDeliverInSeconds` | How long delivery may take before customers are unhappy. |

The wiki's own iron ore example has `initialPrice = 3.5` and `finalPrice = 2.9`; the shipped file above has 4 and 3.2, so treat the wiki numbers as an older balance. The base files also use two fields the wiki doesn't describe: `category` (a lower-case key such as `"coal"` or `"passengers"`, in 27 of the 37 types) and `id` (only in `passengers.cargo.lua` with `"PASSENGERS"` and `rubber.cargo.lua` with `"RUBBER"`). Their effect is not documented. The economies and climates behind `categoryList` are described under [`ModelMetadata.CategoryList`](../api/api/type.md) (symbol: `ModelMetadata.CategoryList`, see [symbol index](../index.tsv)), and what an economy file holds is on [Game mechanics](game-mechanics.md#economies).

Scripts look up loaded cargo types in [`api.res.cargoTypeRep`](../api/api/res.md) (symbol: `Res.cargoTypeRep`, see [symbol index](../index.tsv)) (see [Repositories](scripting.md#repositories)). `find` takes the resource name, which is the game path without `.lua`, as in `api.res.cargoTypeRep.find("::/cargos/fish/fish.cargo")` from `base/content/mission/mission/tutorial/tutorial_story_branch_b.tl`, and returns a negative id for an unknown name. `getName` turns an id back into that name, and [`getPassengerCargoTypeId()`](../api/api/res.md) (symbol: `CargoTypeRep.getPassengerCargoTypeId`, see [symbol index](../index.tsv)) returns the id of passengers.

### The income function

`scriptRef.fileName` points to a function as `file@path.to.function`. The wiki says the game calls it with the `params` table and `timeInMs`, the time between pickup and drop-off, and multiplies the returned factor with the straight-line distance between pickup and drop-off. If the drop-off is higher, eight times the height difference is added to the distance, and the difficulty level scales the result.

All base cargo types except passengers use `incomeFactors.constantFn` from `base/content/economy/economy/cargotype.script.lua`, and that function ignores the time:

```lua
constantFn = function(captureParams, timeInMs)
	if captureParams.initialPrice and captureParams.finalPrice then
		return captureParams.initialPrice + (captureParams.finalPrice - captureParams.initialPrice) * 0.25
	else
		return 6
	end
end,
```

For iron ore that gives 4 + (3.2 − 4) × 0.25 = 3.8 per distance unit, however long the trip takes. `decayRate`, `decayMod` and `decayStart` only matter for `logisticDecayFn` in the same file, which no base cargo uses. Passengers use `passengersFn`, which returns a fixed factor too. The same file also has `stoneFn`, `constructionMaterialsFn` and `baseFactorFn`. To make a cargo lose value with time, point `scriptRef` at `logisticDecayFn` or at a function of your own with the same two parameters.

### Cargo classes

The six classes are in `base/content/cargos/classes/classes/`. From `bulk.cargoclass.lua`:

```lua
function data()
	return {
		tag = "BULK",
		name = _("Bulk"),
		description = "",
		icon = "bulk.tga",
		order = 3,
		color = { 254/255, 202/255, 23/255 },
	}
end
```

`tag` is the key other files use (capital letters `A-Z` only). `color` colors the class labels in the Vehicle Store and build menu. According to the wiki, `description` and `icon` are not shown anywhere yet; the base file leaves `description` empty where the wiki example has a translated text.

| Class | Use (wiki) |
|---|---|
| `UNIVERSAL` | Every normal cargo type. Types in this class need models for the `BIG` and `SMALL` formats. |
| `PASSENGERS` | Passengers only. |
| `GOODS` | Common goods carried in box cars. |
| `FLATBED` | Large, heavy cargo on flat vehicles. |
| `BULK` | Piled cargo such as stone, ore and coal, in open high-sided vehicles. Needs the bulk formats. |
| `LIQUID` | Liquids in tank cars and tank trucks. Needs the `CIRCLE_*` formats. |

Mods can add classes, but Urban Games asks modders to contact them first so that mods stay compatible with each other.

### Cargo model formats

A format is a `tag` and a `size` in meters, measured from the model's root node outwards and upwards. From `base/content/cargos/formats/formats/rect_20x5.cmf.lua`:

```lua
function data()
    return
        {
            tag = "RECT_20x5",
            size = { 20, 5, .3 }
        }
    end
```

The wiki has a table of the standard formats and which class needs which. The base game folder has 28 formats and differs from that table in two places:

- The wiki lists `MEDIUM2x1` and `MEDIUM4x1` as 11.0 × 21.0 × 5.0 (the same as `BUNKER_11x21`). The base files say `size = { 3.37, 2.2, .3 }` for `medium2x1.cmf.lua` and `{ 8.34, 2.8, .3 }` for `medium4x1.cmf.lua`.
- The base game has five formats the wiki doesn't list: `PILE_15x15`, `PILE_30x30`, `PILE_50x20`, `SQUARE_1_2` and `SQUARE_1_7`. Iron ore ships models for `SQUARE_1_2` and `SQUARE_1_7`, so bulk cargo types use them.

To see which formats a bulk cargo needs in practice, list a base cargo folder: `base/content/cargos/iron_ore/iron_ore/` has `iron_ore_big.mdl`, `iron_ore_small.mdl`, `iron_ore_bunker_11x21.mdl`, eleven `iron_ore_rect_*.mdl` files and the two `square` models.

### Cargo type sets

Vehicles, stations and cargo models select cargo with a [`CargoTypeSet`](../api/api/type.md) (symbol: `CargoTypeSet`, see [symbol index](../index.tsv)). The wiki gives the order in which the four lists apply: start empty, add all types of `cargoClassesIncluded`, remove those of `cargoClassesExcluded`, add `cargoTypesIncluded`, remove `cargoTypesExcluded`. Classes are given by tag, types by file name without `.lua`:

```lua
cargoTypeSet = {
	cargoClassesIncluded = { "PASSENGERS", },
	cargoClassesExcluded = { },
	cargoTypesIncluded = { },
	cargoTypesExcluded = { },
},
```

### Cargo models

A cargo model is an ordinary asset model with a `cargoModel` block in its metadata (see [Models and assets](models-and-assets.md)). From `base/content/cargos/iron_ore/iron_ore/iron_ore_big.mdl`:

```lua
cargoModel = {
	cargoTypeSet = {
		cargoClassesExcluded = { },
		cargoClassesIncluded = { },
		cargoTypesExcluded = { },
		cargoTypesIncluded = { "iron_ore.cargo", },
	},
	formats = { "BIG", },
},
```

`formats` lists the formats the model fills. An optional `availability` with `yearFrom` and `yearTo` ([`ModelMetadata.Availability`](../api/api/type.md) (symbol: `ModelMetadata.Availability`, see [symbol index](../index.tsv))) limits the model to a time span; load indicators that pick random cargo models respect it.

## Translations

Translated text has two parts: the keys in the files, and the tables or catalogs that turn keys into text.

### Keys in script files

Any string shown to the player is wrapped in [`_()`](../api/main.md) (symbol: `_`, see [symbol index](../index.tsv)): `name = _("Iron Ore")`. The argument is either English text (`_("Iron Ore")`, `_("English")`) or a key (`_("MISSION_08_NAME")` in `mods/release/urbangames_campaign_mission_08/content/info.mission.lua`). If no translation exists, the argument itself is shown, so English text as the key needs no English entry.

For plural forms and context there are [`nGetText`](../api/main.md) (symbol: `nGetText`, see [symbol index](../index.tsv)), [`pGetText`](../api/main.md) (symbol: `pGetText`, see [symbol index](../index.tsv)) and [`npGetText`](../api/main.md) (symbol: `npGetText`, see [symbol index](../index.tsv)). Context strings use the separator `<|ctx|>`, as in `"map-size<|ctx|>Small"` in `base/mod.json`. Numbers and percentages are formatted for the current language with [`LangUtil`](../base/scripts/lang_util.md) (symbol: `LangUtil`, see [symbol index](../index.tsv)) and [`api.util.lang`](../api/api/util.md) (symbol: `ApiUtil.lang.formatNumber`, see [symbol index](../index.tsv)).

### Translations in a mod: strings.json

A mod translates its keys in a `strings.json` next to `mod.json`, with one object per language code. The wiki example:

```json
{
	"en" : {
		"brake_intensity_name" : "Brake Intensity",
		"brake_intensity_tooltip" : "Adjust the global brake intensity."
	},
	"de" : {
		"brake_intensity_name" : "Bremsintensität",
		"brake_intensity_tooltip" : "Passen Sie die globale Bremsintensität an."
	}
}
```

The wiki's example keys are the name and tooltip of a parameter, so `strings.json` is meant for the `name`, `tooltip` and `values` of your [mod parameters](mod-structure.md#mod-parameters) (`params` in `mod.json`) as well as for the `_("...")` keys in your models and scripts. No shipped mod has a `strings.json` to confirm this. When a key is missing for the current language, the game uses the English text; when that is missing too, it shows the key. None of the official mods in `mods/release/` or `dlcs/` ship a `strings.json`, so their `MISSION_*` and `M8_*` texts are not in the game files these docs are built from.

Which code to use: the wiki says to add one object per `locale` identifier, `<language>_<region>` such as `de_CH`, where the plain `de` is the fallback for every German variant. Its `strings.json` example uses the short codes `en` and `de`, and the shipped `.lang.lua` files have regional codes such as `de_DE` and `en_US` (table below). Short codes are the safe choice for a mod; add a regional object only for text that differs, such as Swiss German.

### Language files

A language itself is a `.lang.lua` file. From `base/content/locale/locale/de.lang.lua`:

```lua
function data()
return {
	name = "Deutsch",
	locale = "de_DE",
	fontMap = {
		regular = "Lato2OFL/Lato-Regular.ttf",
		bold = "Lato2OFL/Lato-Bold.ttf",
		light = "Lato2OFL/Lato-Light.ttf",
		medium = "Lato2OFL/Lato-Medium.ttf",
		monoRegular = "Noto/NotoSansMono-Regular.ttf",
		monoBold = "Noto/NotoSansMono-Bold.ttf",
	},
	unicodeFontMap = {
		regular = "Noto/NotoSansCJKsc-Regular.otf",
		bold = "Noto/NotoSansCJKsc-Bold.otf",
		monoRegular = "Noto/NotoSansMonoCJKsc-Regular.otf",
		monoBold = "Noto/NotoSansMonoCJKsc-Bold.otf",
		scaling = 0.9,
	},
}
end
```

`name` is the entry in the Language setting (Settings > Interface), `locale` the code that mods match their translations against and that sets number formatting, and the two font maps pick the fonts. The fonts are in `base/content/locale/locale/Lato2OFL/` and `Noto/`.

| File | `name` | `locale` |
|---|---|---|
| `de.lang.lua` | Deutsch | `de_DE` |
| `en.lang.lua` | English | `en_US` |
| `es.lang.lua` | Español | `es_ES` |
| `fr.lang.lua` | Français | `fr_FR` |
| `it.lang.lua` | Italiano | `it` |
| `ja.lang.lua` | 日本語 | `ja_JP` |
| `ko.lang.lua` | 한국어 | `ko` |
| `nl.lang.lua` | Nederlands | `nl_NL` |
| `pl.lang.lua` | Polski | `pl_PL` |
| `pt_BR.lang.lua` | Português | `pt_BR` |
| `ru.lang.lua` | Pусский | `ru_RU` |
| `zh_CN.lang.lua` | 简体中文 | `zh_CN` |
| `zh_TW.lang.lua` | 繁體中文 | `zh_TW` |

The `.lang.lua` file holds no text. The wiki says the strings of a language are compiled gettext catalogs in `strings/<locale>/LC_MESSAGES/base.mo`. Those files are not part of the base game sources used here. To translate the game into a new language, the wiki describes the usual gettext round trip:

1. Convert an existing `base.mo` to an editable `.po` file, with `msgunfmt` from GNU gettext (`msgunfmt "xyz.mo" -o "xyz.po"`) or the Python library polib.
2. Translate the `.po` in a text editor or Poedit, after setting the target language under Catalogue > Properties. Keep placeholders such as `%1%` or `{townName}` and fill in every plural form.
3. Compile back to `.mo` with Poedit's File > Compile to MO. (The wiki gives `msgunfmt` for this step too; the gettext tool that compiles is `msgfmt`.)
4. After a game update, merge new keys with Catalog > Update from POT file.

The [Missions](missions.md#translations) guide covers per-language keyframe files for subtitles and voice-over.

## Names

A name set provides person, town and street names, and the player picks one under Names in the new game settings. The base game has 18 sets in `base/content/names/names/`. From `england/england.names.lua`:

```lua
function data()
return
	{
		name = _("English"),
        personNamesScript = {
			fileName = "/names/names.script@personNameScriptFn",
			params = {
                path = "unitedKingdom"
			}
		},
        townNamesScript = {
			fileName = "/names/names.script@townsNameScriptFn",
			params = {
				languages = {
					en = "en",
					fallback = "en",
				},
                path = "england"
			}
		},
		streetNamesScript = {
			fileName = "/names/names.script@streetsNameScriptFn",
			params = {
				languages = {
					en = "en",
					fallback = "en",
				},
                path = "england"
			}
		},
	}
end
```

Each `...Script` entry points to a function. The game calls it with the `params` given here (the wiki calls them `captureParams`) and a second table:

| Function | Second table | Returns |
|---|---|---|
| Person names | `lang`, `isMale` | one string, e.g. `"Madeline Lyla"` |
| Town names | `lang`, `num` | a list of `num` names; all of them when `num` is -1 |
| Street names | `lang`, `num` | a list of `num` names; all of them when `num` is -1 |

The wiki asks town name functions to avoid duplicates and to have enough names for a full map. Its sample code names the functions `townsNameScriptFn` and `streetNameScriptFn`; the base script has `townsNameScriptFn` and `streetsNameScriptFn` (with an `s`).

The shared functions in `base/content/names/names/names.script.lua` work like this:

- `personNameScriptFn` takes first and last names from `personnameutil.lua` in the same folder, from `names[path][lang]` or `names[path].en`. It picks a male or female first name and a last name. With `lastNameFirst = true`, as in `china.names.lua`, the last name comes first. Instead of `path`, a set can give a list `paths` with a matching list `lastNameFirst`, and one of them is picked per person.
- `townsNameScriptFn` and `streetsNameScriptFn` load `<modId>::/names/<path>/<language>/towns.lua` or `streets.lua`. The language comes from the `languages` table: the current `lang` if it has an entry, otherwise `fallback`. Each of these files returns a plain list of strings; `england/en/towns.lua` starts with `"Aberdeen", "Belfast", "Birmingham"`.

The `languages` table in `china.names.lua` has the keys `de`, `en`, `zh_CN`, `zh_TW` and `fallback`, so `lang` can be a regional code; the wiki only mentions short codes such as `en`.

This gives a mod an easy path to new town and street names: reuse the base script, pass your mod id, and ship the lists. The base set above can write `/names/...` because it is part of the base game; a mod points at the base script with `::/names/...`.

```lua
-- content/names/mynames/mynames.names.lua in mod "author_mynames"
function data()
return {
	name = _("My names"),
	personNamesScript = {
		fileName = "::/names/names.script@personNameScriptFn",
		params = { path = "unitedKingdom" },
	},
	townNamesScript = {
		fileName = "::/names/names.script@townsNameScriptFn",
		params = { modId = "author_mynames", path = "mynames", languages = { fallback = "en" } },
	},
	streetNamesScript = {
		fileName = "::/names/names.script@streetsNameScriptFn",
		params = { modId = "author_mynames", path = "mynames", languages = { fallback = "en" } },
	},
}
end
```

The lists then go to `content/names/mynames/en/towns.lua` and `streets.lua`. The `modId` parameter is read by the base script, but none of the base sets use it, so this exact setup is untested. Person names can't be extended this way, because the person function only reads the tables in the base `personnameutil.lua`; new person names need a function of your own.

## People

Pedestrians, passengers and vehicle crews are character models: normal `.mdl` files with a `person` block in their metadata. The base game has 37 of them in `base/content/characters/characters/`, for example `era_a_wom_01` or `era_b_driver_rail`. From `era_a_driver_water/era_a_driver_water.mdl` (shortened):

```lua
metadata = {
	availability = {
		yearFrom = 1850,
		yearTo = 1935,
	},
	colorConfig = {
		configs = {
			{
				{ 0.360784, 0.357884, 0.352295, },
				{ 0.266667, 0.266667, 0.266667, },
				{ 0.596078, 0.596078, 0.596078, },
				{ 0.623529, 0.554269, 0.542837, },
			},
			-- more color sets
		},
	},
	person = {
		drivingLicenses = { "WATER", },
		gender = "MALE",
	},
	soundConfig = {
		effects = {
			select = {
				"::/characters/shared/sound/selected_male_0.wav",
				-- four more
			}
		}
	},
},
```

- `availability` limits the character to an era. `yearFrom` or `yearTo` can be left out or set to 0 for no limit.
- `colorConfig.configs` is a list of color sets for recoloring. Each set has four RGB colors, one per channel of the material's color blend map.
- `person` marks the model as a character. With an empty `drivingLicenses`, it is a pedestrian or passenger. With licenses, it is crew for those vehicles. `gender` is `"FEMALE"` or `"MALE"` ([`Gender`](../api/api/type.md) (symbol: `Gender`, see [symbol index](../index.tsv))) and matches the character to female or male resident names.

The wiki lists the licenses `BUS`, `TRUCK`, `TRAM`, `RAIL`, `WATER`, `AIR` and `AIR_OUTDOOR`. The base game also uses `HELICOPTER`, in `era_c_driver_helicopter.mdl`. A vehicle names the license its crew needs in `seatProvider.drivingLicense` (see [Seats](vehicles.md#seats)). Of the 37 base characters, 24 have no license; road drivers carry `BUS`, `TRUCK` and `TRAM` together. The type definition [`ModelMetadata.Person`](../api/api/type.md) (symbol: `ModelMetadata.Person`, see [symbol index](../index.tsv)) only declares `gender`, not `drivingLicenses`.

The `soundConfig.effects.select` list is not in the wiki. It holds the voice clips heard when the player selects the person.

Character models need these animations, given per bone as `animations = { <name> = { type = "FILE_REF", params = { id = "/characters/shared/ani/..." } } }`:

| Animation | Used for (wiki) |
|---|---|
| `driving` | Sitting in a low driver's seat, as in a car. |
| `driving_upright` | Sitting in a higher driver's seat, as in most other vehicles. |
| `idle` | Standing, e.g. waiting at traffic lights or stations, or standing in a vehicle. |
| `sitting` | Sitting, usually as a passenger. |
| `walk` | Walking. |

All five loop. The base characters share their animation files under `/characters/shared/ani/`.

## Music playlists

A playlist is a list of tracks that the player can pick as Default Playlist in the Audio settings. From `base/content/music/music/default/default.plist.lua` (shortened):

```lua
function data()
return {
	name = _("Transport Fever 3"),
	tracks = {
		{
			name = "J.J. Ipsen - A Promise Made Is a Promise Kept",
			fileName = "a_promise_made_is_a_promise_kept.ogg",
		},
		{
			name = "J.J. Ipsen - Cup of Joe",
			fileName = "cup_of_joe.ogg",
		},
		-- more tracks
	},
	order = 1,
}
end
```

`name` is the playlist's name in the Default Playlist list and `order` its position there. Each track has a `fileName` relative to the `.plist.lua` file (the `.ogg` files are in the same folder) and a `name` for the Music Player. The Deluxe Upgrade Pack ships four more playlists in `dlcs/urbangames_deluxe_upgrade_pack/content/music/music/`, with `order` values such as 300 and tracks that have only a `fileName`, so `name` can be left out.

`base/content/music/music/menu.plist.lua` is the main menu music and uses fields the wiki doesn't cover: `hidden = true` keeps it out of the Default Playlist selection, tracks have `enableRepeat` and `instantTransition`, `fileName` is an absolute path such as `"::/music/tf3_theme_a_loop.ogg"`, and `variants` swaps the file in a given `context` (here `"Credits"`).

## Sound sets

A sound set (`.snd.lua`) groups the sounds of one object: a vehicle, industry, station, depot or town building. The base game has 100. A model or construction points to its sound set in `soundConfig.soundSet.name` ([`ModelMetadata.SoundConfig`](../api/api/type.md) (symbol: `ModelMetadata.SoundConfig`, see [symbol index](../index.tsv))), without the `.lua`. From `base/content/vehicle/train/br75_4/br75_4/br75_4.mdl`:

```lua
soundConfig = {
	soundSet = {
		name = "/vehicle/train/shared/sound/train_steam_modern.snd",
	},
},
```

`base/content/industries/coal_mine/coal_mine/coal_mine.con.lua` uses a relative name, `"sound/coal_mine.snd"`.

### Structure

A sound set returns `tracks`, `events` and an `updateScript`. Tracks loop; events play once each time they are triggered. Every frame the update script sets gain and pitch per track and decides which events play.

The wiki shows `refDist` (the distance the sound's loudness is normalized to) directly on each track and event. The shipped `soundsetutil.lua` writes it into an `attrs` table instead: `{ name = name, attrs = { refDist = refDist } }` for a track and `{ names = names, attrs = { refDist = refDist } }` for an event. Use `soundsetutil` and you don't need to choose.

The update script receives the parameters from the `.snd.lua`, a `params` table with the current situation (`params.currentInfo`) and an output object, a [`SoundTransfOutput`](../base/scripts/soundset.md) (symbol: `SoundTransfOutput`, see [symbol index](../index.tsv)). The wiki names its two methods `addTrack` (once per track, in track order) and `addEvent`. The type definition in `base/tealdef/scripts/soundset.d.tl` has a third, `triggerEvent`, and the base `soundset_default.script.tl` calls `triggerEvent` for custom events and `addEvent` for the others.

### soundsetutil

Every base sound set is built with `/scripts/soundsetutil.lua`, which fills in the default update script. From `base/content/industries/coal_mine/coal_mine/sound/coal_mine.snd.lua`:

```lua
local soundsetutil = require "/scripts/soundsetutil.lua"
-- in a mod: require "::/scripts/soundsetutil.lua"

function data()

local data = soundsetutil.makeSoundSet()

soundsetutil.addSimpleTrackParam01(data, "coal_mine_producing.wav", 50.0, {"industry", "production01"})
soundsetutil.addSimpleTrackParam01(data, "coal_mine_boosted.wav", 60.0, {"industry", "boosted01"})
soundsetutil.addSimpleTrackParam01(data, "coal_mine_double_boosted.wav", 70.0, {"industry", "doubleBoosted01"})

return data

end
```

Sound file names are relative to the `.snd.lua` file. Call `makeSoundSet()` first, then the other functions:

| Function | Adds |
|---|---|
| `addEvent(data, key, names, refDist)` | An event that always plays when triggered; with several files one is chosen at random. |
| `addEventParam01(data, key, names, refDist, gainCurve, pitchCurve, infoGain, infoPitch)` | An event whose gain and pitch follow curves. |
| `addEventCustom(..., customUpdateScript)` | An event with your own update function. |
| `addTrackDefault(data, name, refDist)` | A track at constant gain and pitch (in the file, not in the wiki). |
| `addTrackParam01(data, name, refDist, gainCurve, pitchCurve, infoGain, infoPitch)` | A track whose gain and pitch follow curves. |
| `addSimpleTrackParam01(data, name, refDist, infoGain)` | A track at pitch 1 whose gain is the input value. |
| `addTrackSqueal(data, name, refDist)` | Wheel squeal from speed and side force, e.g. over switches. |
| `addTrackBrake(data, name, refDist, maxGain)` | Brake noise from speed and braking; modern brakes get a lower `maxGain`. |
| `addEventClacks(data, names, refDist, axleRefWeight)` | Rail joint clacks from speed, weight and axle count. |
| `addTrackCustom(..., customUpdateScript)` | A track with your own update function. |
| `makeRoadVehicle2(data, speeds, idle, idleSpeed, idleGain0, drive, driveSpeed, refDist, infoGain, infoPitch)` | Idle and drive sounds of a road vehicle. |
| `makeSteamTrain(data, idle, fast, tracksRefDist, chuffNames, chuffsRefDist, chuffsFastFreq, refWeight)` | Idle, running and chuff sounds of a steam engine. |

A curve is a list of `{ input, value }` points ([`SampleCurveParams`](../base/scripts/soundset.md) (symbol: `SampleCurveParams`, see [symbol index](../index.tsv))); values in between are interpolated, values outside are clamped. The input is a pair of keys into `currentInfo`. These are the pairs the base sound sets use most:

| Input | Uses |
|---|---|
| `{"vehicle", "speed01"}` | 81 |
| `{"industry", "production01"}` | 34 |
| `{"industry", "boosted01"}` | 34 |
| `{"vehicle", "power01"}` | 32 |
| `{"industry", "idle01"}` | 19 |
| `{"industry", "doubleBoosted01"}` | 15 |
| `{"station", "crowd01"}` | 12 |
| `{"station", "cargo01"}` | 10 |

A custom update function receives a `result` with `gain` and `pitch` (both start at 1), your curve parameters and `currentInfo`; for events also the previous frame's info, and it sets `result.trigger = true` to play the event ([`CustomUpdateScript`](../base/scripts/soundset.md) (symbol: `CustomUpdateScript`, see [symbol index](../index.tsv)), [`CustomUpdateEventScript`](../base/scripts/soundset.md) (symbol: `CustomUpdateEventScript`, see [symbol index](../index.tsv))). Set `trigger` for one frame only, or the sound repeats. `base/content/vehicle/bus/shared/shared/sound/bus_electric.snd.lua` uses `addTrackCustom` with `"::/vehicle/shared/sound/accelerationOnlyTrackUpdate.script@updateFn"`, so its idle hum plays only while the bus accelerates.

### Event keys

The game triggers these event keys (from the wiki):

| Key | Objects | When |
|---|---|---|
| `random1` ... `random64` (powers of two) | town buildings, stations, industries | about every n seconds |
| `openDoors` | road, rail, water and air vehicles | after arriving at a station |
| `closeDoors` | road, rail, water and air vehicles | before leaving a station |
| `horn` | rail and water vehicles | when leaving a station |
| `clacks` | rail vehicles | regularly while moving (axle clacks) |
| `chuffs` | rail vehicles | regularly while moving (steam chuffs) |
| `sonicBoom` | air vehicles | when passing the speed of sound |
| `land` | air vehicles | when the wheels touch the ground |

The base sound sets define `horn` for more than the wiki lists: buses, cars, trucks, trams, planes and helicopters have one too (for example `bus_electric.snd.lua` above). The firework sound sets in `base/content/game_mechanics/game_mechanics/fun_elements/firework/` use `launch` and `bang`, driven by `{"state", "launch"}` and `{"state", "explode"}`. According to the wiki, other custom events can only be triggered from edge objects.

### Builder audio sets

A builder audio set (`.builder_audio.lua`) holds the sounds the build menu plays while the player builds, drags or bulldozes. It is not a sound set: it has no tracks and no update script, only lists of `.wav` files per action. All 29 base files are in `base/content/gui/gui/construction/sound/`, and the clips sit in subfolders there (`build/`, `bulldoze/`, `terraform/`, `tool/`), with paths relative to the file. From `buildoze_tree.builder_audio.lua` (the base file names really spell it "buildoze"):

```lua
function data()
	return {
		noMoney = {
			"build/no_money.wav",
		},
		build = {
			"build/asset.wav",
		},
		bulldoze = {
			"bulldoze/tree.wav",
		},
		terraform = {
			"terraform/brush_trees.wav",
		},
	}
end
```

The table is a [`BuilderAudioSet`](../api/api/type.md) (symbol: `BuilderAudioSet`, see [symbol index](../index.tsv)). These are the keys the 29 files use. What each one is for comes from the key names and the files that set it; the game files don't describe them.

| Key | Value | Files | Used in |
|---|---|---|---|
| `build` | list of clips | 23 | placing a construction, module, asset, street, track or tool action |
| `bulldoze` | list of clips | 24 | removing it; the tool sets use `tool/cancel.wav` |
| `noMoney` | list of clips | 20 | building without enough money |
| `terraform` | list of clips | 9 | the terrain brushes and the tree, rock and plant brushes |
| `dragStart`, `dragEnd` | one clip | 4 | start and end of dragging a street, track or lane |
| `drag` | list of clips | 4 | while dragging |
| `dragBridge`, `dragTunnel` | list of clips | 3 | while dragging over a bridge or through a tunnel |
| `dragOffset` | number | 4 | `-0.2` in all four; its effect is not documented |

The four sets with drag keys are `buildoze_street`, `buildoze_track`, `tool_lane_modifier` and `fallback`. The type also has an `attrs` field ([`SoundAttributes`](../api/api/type.md) (symbol: `SoundAttributes`, see [symbol index](../index.tsv))) that no base file sets.

Content points at a set with `builderAudioRes` and the game path without `.lua`: `.con.lua`, `.module.lua`, `.street_template.lua` and `.edge.lua` files at the top level, models in `metadata.soundConfig.builderAudioRes` ([`ModelMetadata.SoundConfig`](../api/api/type.md) (symbol: `ModelMetadata.SoundConfig.builderAudioRes`, see [symbol index](../index.tsv)), used by the trees, rocks and plants), and the build menu tools in their definition. The modules use `"::/gui/construction/sound/buildoze_module.builder_audio"`, which is also the form a mod writes for a base set. A mod can ship its own set under `content/` and point at it the same way. Loaded sets are in [`api.res.builderAudioSetRep`](../api/api/res.md) (symbol: `Res.builderAudioSetRep`, see [symbol index](../index.tsv)); the resource modifier key is `loadBuilderAudioSet` and the file filter category `builderAudioSet` (see [Resource modifiers](scripting.md#resource-modifiers)).

Official wiki: [Cargos](https://wiki.transportfever3.com/doku.php?id=modding:misc:cargo), [Localizations](https://wiki.transportfever3.com/doku.php?id=modding:misc:localizations), [Mod definition](https://wiki.transportfever3.com/doku.php?id=modding:general:moddefinition) (`strings.json`), [Names](https://wiki.transportfever3.com/doku.php?id=modding:misc:names), [People](https://wiki.transportfever3.com/doku.php?id=modding:misc:people), [Playlists](https://wiki.transportfever3.com/doku.php?id=modding:misc:playlists), [Sound sets](https://wiki.transportfever3.com/doku.php?id=modding:misc:soundsets).
