# gui.line_vehicle_mgmt.feedback_list_util

Definition file: `base/tealdef/gui/line_vehicle_mgmt/feedback_list_util.d.tl` (in the game folder).

A feedback strip that shows one timed message at a time (info, warning, error or a question with an accept button), used by the line and vehicle manager and the vehicle entity window.

## FeedbackListUtil (record)

```teal
global record FeedbackListUtil
```
Load: `local feedback_list_util = ug_require "::/gui/line_vehicle_mgmt/feedback_list_util.tl" as FeedbackListUtil`

Feedback strip for short command results and confirmation questions; the recipe is `FeedbackList`.

- `FeedbackList : RecipeWithApi<FeedbackListParams, FeedBackViewerAPI>`: Recipe of the feedback strip; its node ref gives access to `FeedBackViewerAPI`.
  In game: "{countFeedback} Additional Feedback Messages", "Dismiss" (German: „{countFeedback} zusätzliche Feedback-Nachrichten“, „Fertig“)

    ```lua
    local refFeedbackViewer = react.useNodeRef(feedback_list_util.FeedbackList)
    local feedbackViewer = feedback_list_util.FeedbackList(react.ref(refFeedbackViewer), {})
    -- later
    refFeedbackViewer:get():getApi().addFeedback(_("Vehicle cannot be sold at this time."))
    ```

    - used 4x in the base game, e.g. `base/content/gui/gui/entity_window/vehicle/vehicle.tl:302`: `local refFeedbackViewer = react.useNodeRef(feedback_list_util.FeedbackList)`

### FeedbackListUtil.FeedbackDialogParam (record)

```teal
record FeedbackDialogParam
```

Accept button of a `Question` or `Info` feedback message.

- `onAccept : function()`: Called when the accept button is clicked; the message is dismissed afterwards.
- `acceptText : string`: Label of the accept button (e.g. `_("Delete")`).
- `soundOnAccept : SoundOnAccept`: Optional sound style of the accept button.

#### FeedbackListUtil.FeedbackDialogParam.SoundOnAccept (enum)

```teal
enum SoundOnAccept
```

Sound style applied to the accept button.

- `"SellVehicle"`: Accept button gets the `sell-vehicle-sfx` style class (sell sound).
- `"DeleteLine"`: Accept button gets the `delete-line-sfx` style class (delete-line sound).

### FeedbackListUtil.CommandFeedback (record)

```teal
record CommandFeedback
```

One queued feedback message, as stored by `FeedbackList`.

- `time : number`: Application time (from `api.util.getApplicationTime()`) at which the message expires; set to 0 to dismiss it.
- `message : string`: Text shown to the player.
- `mode : Mode`: Kind of message.
- `dialogData : FeedbackDialogParam`: Accept button settings for `Info` and `Question` messages.
- `timeout : number`: Display duration in seconds (default 5).
- `id : number`: Optional id; adding a message with the id of a queued message replaces that message instead of queueing a new one.
- `dismissible : boolean`: If true, a Dismiss button (Cancel for questions) is shown.

#### FeedbackListUtil.CommandFeedback.Mode (enum)

```teal
enum Mode
```

Kind of message; selects the tape style and whether an accept button is shown.

- `"Info"`: Informational message (`info-tape` style); shows the accept button from `dialogData`.
- `"Warning"`: Warning message (`warning-tape` style).
- `"Error"`: Error message (`error-tape` style).
- `"Question"`: Question (`info-tape` style) with the accept button from `dialogData` and a Cancel button.

### FeedbackListUtil.FeedBackViewerAPI (record)

```teal
record FeedBackViewerAPI
```

API of a `FeedbackList` node.

- `addFeedback : function(message : string, mode? : string, dialogData? : FeedbackDialogParam, timeout? : number, id? : number, dismissible? : boolean)`: Queues a feedback message and plays `LvmSFX.Feedback`.
    - param `message`: Text to show.
    - param `mode`: `Info`, `Warning`, `Error` or `Question`. Without a mode the message gets the warning style.
    - param `dialogData`: Accept button for `Info` and `Question` messages.
    - param `timeout`: Display time in seconds; default 5.
    - param `id`: Replaces a queued message with the same id instead of adding one.
    - param `dismissible`: Shows a Dismiss / Cancel button.
    - notes: Only the first queued message is displayed, with a `+N` counter for the rest. Messages behind the first one have their expiry pushed back every frame, so each message gets its full `timeout` once it becomes visible.

    ```lua
    -- from manager_window.tl (commonParams.addFeedback forwards to this API)
    addFeedback(_("Deleting the line will sell all vehicles on the line."), "Question", {
        onAccept = function() wrappedCommitDelete() end,
        acceptText = _("Delete"),
        soundOnAccept = "DeleteLine",
    }, 2)
    ```

- `hasFeedback : function() : boolean`: Tells whether any message is queued.
    - returns: True if at least one message is queued.

### FeedbackListUtil.FeedbackListParams (record)

```teal
record FeedbackListParams
```

Parameters of `FeedbackList` (none; pass `{}`).
