using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security;
using System.Text;

using Tf3Drpc.Core.Constants;

namespace Tf3Drpc.Core.Translation;

/// <summary>
/// Reads the msgid to msgstr pairs of a GNU gettext .mo file.
/// </summary>
public static class MoReader
{
    /// <summary>
    /// Parses .mo data; corrupt or oversized data, or more decoded text than the file holds, gives <see langword="false"/>.
    /// </summary>
    public static bool TryParse(ReadOnlySpan<byte> data, [NotNullWhen(true)] out IReadOnlyDictionary<string, string>? entries)
    {
        entries = null;

        if (data.Length is < TranslationConstants.MoHeaderBytes or > TranslationConstants.MaxMoFileBytes)
            return false;

        uint magic = BinaryPrimitives.ReadUInt32LittleEndian(data);
        bool littleEndian = magic is TranslationConstants.MoMagicLittleEndian;

        if (!littleEndian && (magic is not TranslationConstants.MoMagicBigEndian))
            return false;

        uint revision = ReadUInt32(data, 4, littleEndian);
        uint count = ReadUInt32(data, 8, littleEndian);
        uint originalTable = ReadUInt32(data, 12, littleEndian);
        uint translationTable = ReadUInt32(data, 16, littleEndian);
        long tableBytes = (long)count * TranslationConstants.MoTableEntryBytes;

        bool tablesFit = ((long)originalTable + tableBytes <= data.Length) &&
            ((long)translationTable + tableBytes <= data.Length);

        if (((revision >> 16) is not 0) || !tablesFit)
            return false;

        long decodedBytes = 0;
        Dictionary<string, string> result = new((int)Math.Min(count, TranslationConstants.MaxInitialEntryCapacity), StringComparer.Ordinal);

        for (uint i = 0; i < count; i++)
        {
            long entryOffset = i * (long)TranslationConstants.MoTableEntryBytes;

            if (!TryReadString(data, originalTable + entryOffset, littleEndian, out ReadOnlySpan<byte> original) ||
                !TryReadString(data, translationTable + entryOffset, littleEndian, out ReadOnlySpan<byte> translation))
            {
                return false;
            }

            bool skip = original.IsEmpty ||
                translation.IsEmpty ||
                (original.IndexOfAny(TranslationConstants.NulByte, TranslationConstants.ContextSeparatorByte) is >= 0);

            if (skip)
                continue;

            decodedBytes += original.Length;

            if (decodedBytes > data.Length)
                return false;

            string key = Encoding.UTF8.GetString(original);

            if (result.ContainsKey(key))
                continue;

            decodedBytes += translation.Length;

            if (decodedBytes > data.Length)
                return false;

            result[key] = Encoding.UTF8.GetString(translation);
        }

        entries = result;

        return true;
    }

    /// <summary>
    /// Reads a .mo file; a missing, unreadable, oversized or corrupt file gives <see langword="false"/>.
    /// </summary>
    public static bool TryReadFile(string path, [NotNullWhen(true)] out IReadOnlyDictionary<string, string>? entries)
    {
        entries = null;

        try
        {
            FileInfo info = new(path);

            if (!info.Exists || (info.Length is > TranslationConstants.MaxMoFileBytes))
                return false;

            byte[] data = File.ReadAllBytes(path);

            return TryParse(data, out entries);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or SecurityException or ArgumentException or NotSupportedException)
        {
            return false;
        }
    }

    private static uint ReadUInt32(ReadOnlySpan<byte> data, int offset, bool littleEndian)
        => (littleEndian)
            ? BinaryPrimitives.ReadUInt32LittleEndian(data[offset..])
            : BinaryPrimitives.ReadUInt32BigEndian(data[offset..]);

    private static bool TryReadString(ReadOnlySpan<byte> data, long tableEntry, bool littleEndian, out ReadOnlySpan<byte> text)
    {
        text = default;
        int entry = (int)tableEntry;
        uint length = ReadUInt32(data, entry, littleEndian);
        uint offset = ReadUInt32(data, entry + 4, littleEndian);

        if (((long)offset + length) > data.Length)
            return false;

        text = data.Slice((int)offset, (int)length);

        return true;
    }
}
