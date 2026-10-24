namespace Tf3Drpc.App.Messages;

/// <summary>
/// Message sent when the daily log file turned out to be unwritable while the app runs.
/// </summary>
internal sealed record class LogFileUnavailableMessage;
