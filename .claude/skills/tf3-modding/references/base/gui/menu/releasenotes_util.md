# gui.menu.releasenotes_util

Definition file: `base/tealdef/gui/menu/releasenotes_util.d.tl` (in the game folder).

Builds the pages of the release-notes window from `ReleaseNoteData`.

## ReleaseNotesUtil (record)

```teal
global record ReleaseNotesUtil
```

Util table of `releasenotes_util.tl`.

- `createElement : function(info : ReleaseNoteData.InfoItem, ref? : ReactRefWrap) : TreeNodeId`: Builds the component of one release-note item (title, text and image or logo).
    - param `info`: The item.
    - param `ref`: Ref attached to the text's scroll area when `info.scrollable` is true. Optional.
    - returns: The component.
- `createElements : function(releaseNotes : ReleaseNoteData) : {TreeNodeId}`: Builds the components of all items of the release notes, leaving out items filtered for the current vendor or platform.
    - returns: One component per shown item, in order.
- `createEmpty : function() : TreeNodeId`: Returns an empty release-note component, used to fill up the last tab page of the window.
