# scripts.soundset

Definition file: `base/tealdef/scripts/soundset.d.tl` (in the game folder).

Types used by sound-set update scripts (see `/scripts/soundset_default.script.tl`): the output object
that receives track and event gain/pitch values, and the parameter records of custom and curve-based
update functions.

## SoundTransfOutput (record)

```teal
global record SoundTransfOutput
```

Sound Transformation output, which contains user transformations of events and tracks from transformation lua scripts

- `addTrack : function(soundTransfOutput : SoundTransfOutput, gain : number, pitch : number)`  (call as `soundTransfOutput:addTrack(…) (method)`): Sets gain and pitch of the next sound track of the sound set.
    - param `gain`: Volume factor.
    - param `pitch`: Pitch factor (1 = unchanged).
    - notes: Tracks are filled in order; the default script notes that the order matters for steam train chuffs.
    - used 3x in the base game, e.g. `base/content/scripts/scripts/soundset_default.script.tl:203`: `soundTransfOutput:addTrack(`
- `addEvent : function(soundTransfOutput : SoundTransfOutput, event : string, gain : number, pitch : number)`  (call as `soundTransfOutput:addEvent(…) (method)`): Sets gain and pitch of the named sound event.
    - param `event`: Event key from the sound set.
    - param `gain`: Volume factor.
    - param `pitch`: Pitch factor.
    - used 2x in the base game, e.g. `base/content/scripts/scripts/soundset_default.script.tl:197`: `soundTransfOutput:addEvent(`
- `triggerEvent : function(soundTransfOutput : SoundTransfOutput, event : string, gain : number, pitch : number)`  (call as `soundTransfOutput:triggerEvent(…) (method)`): Triggers the named sound event once with the given gain and pitch.
    - param `event`: Event key from the sound set.
    - param `gain`: Volume factor.
    - param `pitch`: Pitch factor.
    - used 1x in the base game, e.g. `base/content/scripts/scripts/soundset_default.script.tl:174`: `soundTransfOutput:triggerEvent(`

## CustomUpdateScript (record)

```teal
global record CustomUpdateScript
```

Script reference for a `Custom` sound update function that drives a track.

- `updateFn : function(result : GainPitch, customParams : any, currentInfoParams : {string : {string : number}})`: Computes gain and pitch for a track and writes them into `result`.
    - param `result`: Output, prefilled with gain 1 and pitch 1.
    - param `customParams`: The `customParams` given in the sound set.
    - param `currentInfoParams`: Current transformator info (`TransformatorParams.currentInfo`) as nested tables of numbers.

## CustomUpdateEventScript (record)

```teal
global record CustomUpdateEventScript
```

Script reference for a `Custom` sound update function that triggers an event.

- `updateFn : function(result : GainPitchTrigger, customParams : any, currentInfoParams : {string : {string : number}}, previousInfoParams : {string : {string : number}})`: Decides whether to trigger the event and with which gain and pitch.
    - param `result`: Output, prefilled with gain 1, pitch 1 and `trigger = false`; set `trigger = true` to fire the event.
    - param `customParams`: The `customParams` given in the sound set.
    - param `currentInfoParams`: Current transformator info as nested tables of numbers.
    - param `previousInfoParams`: The info of the previous step, to detect changes.

## GainPitch (record)

```teal
global record GainPitch
```

Gain and pitch of one sound track or event.

- `gain : number`: Volume factor.
- `pitch : number`: Pitch factor (1 = unchanged).

## GainPitchTrigger (record)

```teal
global record GainPitchTrigger
```

Gain and pitch of a sound event plus whether to trigger it now.

- `gain : number`: Volume factor.
- `pitch : number`: Pitch factor.
- `trigger : boolean`: True to trigger the event in this step.

## SampleCurveParams (record)

```teal
global record SampleCurveParams
```

Parameters of the `SampleCurve` sound update function, which reads values from the transformator info and maps them through curves.

- `gainCurve : {{number, number}}`: List of `{x, y}` nodes mapping the input value to gain.
- `pitchCurve : {{number, number}}`: List of `{x, y}` nodes mapping the input value to pitch.
- `gainScriptingInfoKey : string`: Key of the info table in `currentInfo` (e.g. `vehicle`) that holds the gain input.
- `gainParamName : string`: Name of the value in that table used as gain input (e.g. `speed`).
- `pitchScriptingInfoKey : string`: Key of the info table in `currentInfo` that holds the pitch input.
- `pitchParamName : string`: Name of the value in that table used as pitch input.
