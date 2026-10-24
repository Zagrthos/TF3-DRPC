using System;
using System.Globalization;
using System.IO;
using System.Text.Json;

using Tf3Drpc.Core.GameLog;
using Tf3Drpc.Core.Presence;
using Tf3Drpc.Core.Status;
using Tf3Drpc.Core.Structs;
using Tf3Drpc.Core.Translation;

if (args.Length is not 2)
    throw new ArgumentException("Expected the messages JSON path and the output JSON path.");

using JsonDocument input = JsonDocument.Parse(File.ReadAllBytes(args[0]));
using FileStream output = File.Create(args[1]);
using Utf8JsonWriter writer = new(output, new JsonWriterOptions() { Indented = true });
writer.WriteStartArray();
int count = 0;

foreach (JsonElement item in input.RootElement.EnumerateArray())
{
    string id = item.GetProperty("id").GetString()!;
    string message = item.GetProperty("message").GetString()!;
    bool expected = item.GetProperty("expectedAccepted").GetBoolean();
    GameLogEntryStruct entry = new() { Message = message };
    bool accepted = GameLogEventClassifier.TryClassify(entry, out GameLogEventStruct gameEvent);

    if (accepted != expected)
        throw new InvalidOperationException($"{id}: expected accepted={expected}, observed {accepted}.");

    string? month = null;

    if (gameEvent.Status is ModStatus status)
        month = PresenceTemplate.Render("{month}", status, CultureInfo.GetCultureInfo("en-GB"), GameTexts.English);

    if ((id is "calendar_year") && (month is not null))
        throw new InvalidOperationException("The year-getter reproduction should also have lost the readable month.");

    if ((id is "calendar_year_retained_month_control") && (month is not "September"))
        throw new InvalidOperationException("The retained-month control should render September without a year.");

    writer.WriteStartObject();
    writer.WriteString("id", id);
    writer.WriteBoolean("accepted", accepted);
    writer.WriteString("monthTemplate", month);
    writer.WriteEndObject();
    Console.WriteLine($"PASS {id}: accepted={accepted}, month={month ?? "<absent>"}");
    count++;
}

writer.WriteEndArray();
Console.WriteLine($"Receiver checks: {count} passed.");
