# gui.menu.paradox_react_util

Definition file: `base/tealdef/gui/menu/paradox_react_util.d.tl` (in the game folder).

Paradox account recipes and texts (login, account creation, account linking, profile name), loaded with `ug_require "/gui/menu/paradox_react_util.tl"`.

## ParadoxReactUtil (record)

```teal
global record ParadoxReactUtil
```
Load: `local paradox_react_util = ug_require "::/gui/menu/paradox_react_util.tl" as ParadoxReactUtil`

Recipes and helper texts for the Paradox account.

- `DatePicker : Recipe<DatePickerParams>`: Three combo boxes for year (current year down to 1900), month and day, used for the date of birth.
  In game: "Year", "Month", "Day" (German: „Jahr“, „Monat“, „Tag“)
- `PdxAccountLogin : Recipe<PdxAccountLoginParams>`: Paradox account login form. Handles automatic login for linked platform accounts, account linking and creation of a missing profile.
  In game: "Paradox Account", "Username or Email", "Create New Account" (German: „Paradox-Konto“, „Benutzername oder E-Mail“, „Neues Konto erstellen“)
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:3194`: `children[#children + 1] = paradox_react_util.PdxAccountLogin({`
- `PdxAccountName : Recipe0`: Paradox icon with the profile name of the logged-in account, or its email address if no profile exists.
    - notes: The name is fetched again on the `pdxAccountNameChanged` React event.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:1203`: `and paradox_react_util.PdxAccountName{ wc = params.mmCtx.wc }`
- `PdxLinkButton : Recipe<PdxLinkButtonParams>`: Button to link or unlink the platform account and the Paradox account. Shows nothing if the platform does not support account linking.
  In game: "Link Account", "Unlink Account", "Paradox Account: {email}" (German: „Konto verknüpfen“, „Verknüpftes Konto trennen“, „Paradox-Konto: {email}“)
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:1246`: `paradox_react_util.PdxLinkButton{`
- `PdxRenameProfileButton : Recipe<PdxRenameProfileButtonParams>`: "Rename Profile" button that opens the profile rename window. Shows nothing if no account is logged in.
  In game: "Rename Profile" (German: „Profil umbenennen“)
- `getLinkText : function(linkState : PdxThirdPartyLinkState) : string`  (call as `paradox_react_util.getLinkText(…)`): Returns the translated explanation or question for an account link state, worded for the current platform (Steam, PlayStation Network, Xbox network or generic).
  In game: "Paradox Interactive will link your platform account to allow you to get automatically logged in to your Paradox Account when starting the game. Should you no longer wish to use this feature, go to the settings screen and unlink.", "This Paradox Account is already linked to a different platform account. Would you like to overwrite the link?", "The currently logged in Paradox Account and the platform account are linked. Do you want to unlink?" (German: „Paradox Interactive wird dein Plattformkonto verknüpfen, damit du beim Spielstart automatisch bei deinem Paradox-Konto angemeldet werden kannst. Solltest du diese Funktion nicht mehr wünschen, kannst du die Verknüpfung in den Einstellungen trennen.“, „Dieses Paradox-Konto ist bereits mit einem anderen Plattformkonto verknüpft. Möchtest du diese Verknüpfung überschreiben?“, „Das derzeit angemeldete Paradox-Konto und das Plattformkonto sind miteinander verknüpft. Möchtest du diese Verknüpfung trennen?“)
    - returns: The text, or `nil` for other states.
- `getAutoLoginPromptStateText : function() : string`  (call as `paradox_react_util.getAutoLoginPromptStateText(…)`): Returns the translated question whether to log in automatically with the linked platform account.
  In game: "Your platform account is linked to a Paradox Account. Do you wish to automatically log in to your Paradox Account?" (German: „Dein Plattformkonto ist mit einem Paradox-Konto verknüpft. Möchtest du dich automatisch bei deinem Paradox-Konto anmelden?“)
- `getErrorMessageAndField : function(err : PdxErrorCodes) : string, string`  (call as `paradox_react_util.getErrorMessageAndField(…)`): Translates a Paradox backend error code into a message.
  In game: "The email address you entered is invalid. Please check your input and try again.", "The password you entered is invalid. Please check your input and try again.", "The profile name you entered is already taken. Please choose a different name." (German: „Die eingegebene E-Mail-Adresse ist ungültig. Bitte überprüfe deine Eingabe und versuche es erneut.“, „Das eingegebene Passwort ist ungültig. Bitte überprüfe deine Eingabe und versuche es erneut.“, „Der Profilname, den du eingegeben hast, ist bereits vergeben. Bitte wähle einen anderen Namen.“)
    - returns: The message and the name of the affected form field (`"email"`, `"password"`, `"dateOfBirth"`, `"profileName"`), or `nil` as field for general errors.

### ParadoxReactUtil.DatePickerParams (record)

```teal
record DatePickerParams
```

Parameters of `DatePicker`.

- `value : string`: Initial date as `YYYY-MM-DD`.
- `onValueChange : function(string)`: Called with the date as `YYYY-MM-DD` whenever a part changes, or with `""` while year, month or day is not chosen.

### ParadoxReactUtil.PdxAccountLoginParams (record)

```teal
record PdxAccountLoginParams
```

Parameters of `PdxAccountLogin`.

- `onProceed : function()`: Called when login (and profile creation, if needed) has finished successfully.
- `onStart : function()`: Called when a login process starts.
- `onAbort : function()`: Called when the player aborts the login.
- `wc : ReactRefWrapApiT<Builtin.WindowContainerDelegateParam, Builtin.WindowAPI>`: Window container used to open the account creation window.

### ParadoxReactUtil.PdxLinkButtonParams (record)

```teal
record PdxLinkButtonParams
```

Parameters of `PdxLinkButton`.

- `wc : ReactRefWrapApiT<Builtin.WindowContainerDelegateParam, Builtin.WindowAPI>`: Window container used to open the account linking window.

### ParadoxReactUtil.PdxRenameProfileButtonParams (record)

```teal
record PdxRenameProfileButtonParams
```

Parameters of `PdxRenameProfileButton`.

- `wc : ReactRefWrapApiT<Builtin.WindowContainerDelegateParam, Builtin.WindowAPI>`: Window container used to open the rename window.
- `onStart : function()`: Called when the rename window is opened.
- `onEnd : function()`: Called when the rename window is closed.
