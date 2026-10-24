# mission.tasks.utility.mission_task_send_command

Definition file: `base/tealdef/mission/tasks/utility/mission_task_send_command.d.tl` (in the game folder).

Task that sends an engine command.

## MissionTaskSendCommand (record)

```teal
global record MissionTaskSendCommand is MissionInterface.TaskScript<MissionTaskSendCommand.State, MissionTaskSendCommand.Params>
```
Load: `local send_command = ug_require "::/mission/tasks/utility/send_command.tl" as MissionTaskSendCommand`

Task (`send_command`) that sends the command returned by `getCommand` and completes when it succeeded (or when no command is returned).

### MissionTaskSendCommand.Params (record)

```teal
record Params
```

Params of the task.

- `getCommand : function() : Command<ICommandData>`: Builds the command to send.
    - returns: Command, or `nil` to complete without sending.

### MissionTaskSendCommand.State (record)

```teal
record State
```

State of the task.

- `success : boolean`: The command succeeded.
