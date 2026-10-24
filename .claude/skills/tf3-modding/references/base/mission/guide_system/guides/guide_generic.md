# mission.guide_system.guides.guide_generic

Definition file: `base/tealdef/mission/guide_system/guides/guide_generic.d.tl` (in the game folder).

Guide task triggered by a custom condition function.

## GuideGeneric (record)

```teal
global record GuideGeneric is MissionInterface.TaskScript<GuideGeneric.State, GuideGeneric.Params>
```
Load: `local guide_generic = ug_require "::/mission/guide_system/guides/guide_generic.tl" as GuideGeneric`

Guide task triggered when a custom function returns true.

Example:
```lua
-- base game: mission/guide_system/main/guide_system_story.tl
if taskKey == "create_line" then
    return {
        taskScript = guide_generic as MissionInterface.TaskScript<any, any>,
        taskParams = {
            guiWaitFn = function() : boolean
                return api.gui.byId.isVisibleRecursive("menu.management")
            end,
        } as GuideGeneric.Params,
        guideInfo = {
            text = _("To create new lines, ..."),
            name = _("Create a Line"),
            targets = { { idOrTag = "menu.management.newLineButton", position = "Right" } },
        },
    }
end
```

### GuideGeneric.Params (record)

```teal
record Params
```

Trigger conditions.

- `waitFn : function(readOnlyApi : MissionInterface.MissionTaskReadOnlyApi) : boolean`: Engine-side condition, checked every update.
    - param `readOnlyApi`: Read-only mission API.
    - returns: `true` to trigger the guide.
- `guiWaitFn : function(readOnlyApi : MissionInterface.DeferredTaskApi) : boolean`: GUI-side condition, checked every GUI update.
    - param `readOnlyApi`: Deferred API.
    - returns: `true` to trigger the guide.

### GuideGeneric.State (record)

```teal
record State is IGuideState
```

Guide state.

- `state : any`: Free state slot for the guide.
