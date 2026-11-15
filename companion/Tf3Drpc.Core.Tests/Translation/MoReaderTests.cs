using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

using Tf3Drpc.Core.Constants;
using Tf3Drpc.Core.Translation;

using Xunit;

namespace Tf3Drpc.Core.Tests.Translation;

/// <summary>
/// Tests of <see cref="MoReader"/>, which parses the gettext MO translation files of the game and fails cleanly on malformed or hostile input.
/// </summary>
public sealed class MoReaderTests
{
    private static readonly string RealGermanFile = (OperatingSystem.IsWindows())
        ? @"C:\Program Files (x86)\Steam\steamapps\common\Transport Fever 3\base\strings\de\LC_MESSAGES\base.mo"
        : Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "Library/Application Support/Steam/steamapps/common/Transport Fever 3/base/strings/de/LC_MESSAGES/base.mo"
        );

    private static readonly (string Original, string Translation)[] SampleEntries =
    [
        (string.Empty, "Project-Id-Version: test\nContent-Type: text/plain; charset=UTF-8\n"),
        ("Mechanic", "Mechaniker"),
        ("apple\0apples", "Apfel\0Äpfel"),
        ("menu\u0004File", "Datei"),
        ("Loading...", "Laden..."),
        ("Holzfäller", "Holzfällerlager")
    ];

    internal static byte[] Build(IReadOnlyList<(string Original, string Translation)> entries, bool bigEndian)
    {
        int count = entries.Count;
        int tableBytes = count * 8;
        const int originalTable = 28;
        int translationTable = originalTable + tableBytes;
        int position = translationTable + tableBytes;
        using MemoryStream strings = new();
        byte[] table = new byte[tableBytes * 2];

        for (int i = 0; i < count; i++)
        {
            byte[] original = Encoding.UTF8.GetBytes(entries[i].Original);
            byte[] translation = Encoding.UTF8.GetBytes(entries[i].Translation);

            WriteUInt32(table.AsSpan(i * 8), (uint)original.Length, bigEndian);
            WriteUInt32(table.AsSpan((i * 8) + 4), (uint)(position + strings.Length), bigEndian);
            strings.Write(original);
            strings.WriteByte(0);

            WriteUInt32(table.AsSpan(tableBytes + (i * 8)), (uint)translation.Length, bigEndian);
            WriteUInt32(table.AsSpan(tableBytes + (i * 8) + 4), (uint)(position + strings.Length), bigEndian);
            strings.Write(translation);
            strings.WriteByte(0);
        }

        byte[] file = new byte[position + (int)strings.Length];
        WriteUInt32(file.AsSpan(0), 0x950412de, bigEndian);
        WriteUInt32(file.AsSpan(4), 0, bigEndian);
        WriteUInt32(file.AsSpan(8), (uint)count, bigEndian);
        WriteUInt32(file.AsSpan(12), originalTable, bigEndian);
        WriteUInt32(file.AsSpan(16), (uint)translationTable, bigEndian);
        table.CopyTo(file, originalTable);
        strings.ToArray().CopyTo(file, position);

        return file;
    }

    private static void WriteUInt32(Span<byte> target, uint value, bool bigEndian)
    {
        if (bigEndian)
        {
            BinaryPrimitives.WriteUInt32BigEndian(target, value);
        }
        else
        {
            BinaryPrimitives.WriteUInt32LittleEndian(target, value);
        }
    }

    /// <summary>
    /// A valid file in either byte order yields only the plain entries, leaving out the header entry with the empty original, plural entries and entries with a context.
    /// </summary>
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ValidFileYieldsOnlyPlainEntries(bool bigEndian)
    {
        byte[] file = Build(SampleEntries, bigEndian);

        bool parsed = MoReader.TryParse(file, out IReadOnlyDictionary<string, string>? entries);

        Assert.True(parsed);
        Assert.NotNull(entries);
        Assert.Equal(3, entries.Count);
        Assert.Equal("Mechaniker", entries["Mechanic"]);
        Assert.Equal("Laden...", entries["Loading..."]);
        Assert.Equal("Holzfällerlager", entries["Holzfäller"]);
        Assert.False(entries.ContainsKey(string.Empty));
        Assert.False(entries.ContainsKey("apple\0apples"));
        Assert.False(entries.ContainsKey("menu\u0004File"));
    }

    /// <summary>
    /// A valid file with zero entries parses to an empty dictionary.
    /// </summary>
    [Fact]
    public void EmptyFileWithZeroEntriesParses()
    {
        byte[] file = Build([], bigEndian: false);

        Assert.True(MoReader.TryParse(file, out IReadOnlyDictionary<string, string>? entries));
        Assert.Empty(entries);
    }

    /// <summary>
    /// A truncated file fails with no entries exactly when the cut removes a byte that a table entry points to, and otherwise parses to the entries of the full file.
    /// </summary>
    [Fact]
    public void TruncationFailsExactlyWhenItCutsDataTheParserNeeds()
    {
        byte[] file = Build(SampleEntries, bigEndian: false);
        Assert.True(MoReader.TryParse(file, out IReadOnlyDictionary<string, string>? expected));
        Assert.NotNull(expected);

        // The parser reads the header, both tables and every string the tables point to, but never the NUL after a string.
        int needed = TranslationConstants.MoHeaderBytes + (SampleEntries.Length * TranslationConstants.MoTableEntryBytes * 2);

        for (int table = 0; table < 2; table++)
        {
            int tableStart = BinaryPrimitives.ReadInt32LittleEndian(file.AsSpan(12 + (table * 4)));

            for (int i = 0; i < SampleEntries.Length; i++)
            {
                int length = BinaryPrimitives.ReadInt32LittleEndian(file.AsSpan(tableStart + (i * TranslationConstants.MoTableEntryBytes)));
                int offset = BinaryPrimitives.ReadInt32LittleEndian(file.AsSpan(tableStart + (i * TranslationConstants.MoTableEntryBytes) + 4));
                needed = Math.Max(needed, offset + length);
            }
        }

        Assert.True(needed < file.Length);

        for (int length = 0; length < file.Length; length++)
        {
            bool parsed = MoReader.TryParse(file.AsSpan(0, length), out IReadOnlyDictionary<string, string>? entries);

            if (length < needed)
            {
                Assert.False(parsed);
                Assert.Null(entries);
            }
            else
            {
                Assert.True(parsed);
                Assert.Equal(expected, entries);
            }
        }
    }

    /// <summary>
    /// A file whose magic number is corrupted fails to parse.
    /// </summary>
    [Fact]
    public void BadMagicFails()
    {
        byte[] file = Build(SampleEntries, bigEndian: false);
        file[0] ^= 0xFF;

        Assert.False(MoReader.TryParse(file, out IReadOnlyDictionary<string, string>? _));
    }

    /// <summary>
    /// A file with an unsupported major revision (1 in the high 16 bits) fails to parse.
    /// </summary>
    [Fact]
    public void UnsupportedMajorRevisionFails()
    {
        byte[] file = Build(SampleEntries, bigEndian: false);
        BinaryPrimitives.WriteUInt32LittleEndian(file.AsSpan(4), 1u << 16);

        Assert.False(MoReader.TryParse(file, out IReadOnlyDictionary<string, string>? _));
    }

    /// <summary>
    /// A file that claims the maximum entry count fails to parse instead of reading past the data.
    /// </summary>
    [Fact]
    public void HugeEntryCountFails()
    {
        byte[] file = Build(SampleEntries, bigEndian: false);
        BinaryPrimitives.WriteUInt32LittleEndian(file.AsSpan(8), uint.MaxValue);

        Assert.False(MoReader.TryParse(file, out IReadOnlyDictionary<string, string>? _));
    }

    /// <summary>
    /// A file whose string table offset points to the end of the data fails to parse.
    /// </summary>
    [Fact]
    public void TableOffsetPastTheEndFails()
    {
        byte[] file = Build(SampleEntries, bigEndian: false);
        BinaryPrimitives.WriteUInt32LittleEndian(file.AsSpan(12), (uint)file.Length);

        Assert.False(MoReader.TryParse(file, out IReadOnlyDictionary<string, string>? _));
    }

    /// <summary>
    /// A file with a string offset past the end of the data fails to parse.
    /// </summary>
    [Fact]
    public void StringOffsetPastTheEndFails()
    {
        byte[] file = Build(SampleEntries, bigEndian: false);
        BinaryPrimitives.WriteUInt32LittleEndian(file.AsSpan(28 + 8 + 4), (uint)file.Length);

        Assert.False(MoReader.TryParse(file, out IReadOnlyDictionary<string, string>? _));
    }

    /// <summary>
    /// A string length of 4,294,967,295 or 2,147,483,647 fails to parse instead of reading or allocating that much.
    /// </summary>
    [Theory]
    [InlineData(uint.MaxValue)]
    [InlineData(int.MaxValue)]
    public void HugeStringLengthFails(uint length)
    {
        byte[] file = Build(SampleEntries, bigEndian: false);
        BinaryPrimitives.WriteUInt32LittleEndian(file.AsSpan(28 + 8), length);

        Assert.False(MoReader.TryParse(file, out IReadOnlyDictionary<string, string>? _));
    }

    /// <summary>
    /// An input just over 32 MiB is rejected and fails to parse.
    /// </summary>
    [Fact]
    public void OversizedInputFails()
    {
        byte[] file = new byte[(32 * 1024 * 1024) + 1];
        BinaryPrimitives.WriteUInt32LittleEndian(file, 0x950412de);

        Assert.False(MoReader.TryParse(file, out IReadOnlyDictionary<string, string>? _));
    }

    private static byte[] BuildAliased(int count, bool distinctOriginals)
    {
        const int OriginalTable = 28;
        int tableBytes = count * 8;
        int translationTable = OriginalTable + tableBytes;
        int strings = translationTable + tableBytes;
        byte[] translation = new byte[64 * 1024];
        Array.Fill(translation, (byte)'t');
        byte[] originals = new byte[count * 8];
        byte[] file = new byte[strings + originals.Length + translation.Length + 1];
        int translationOffset = strings + originals.Length;

        WriteUInt32(file.AsSpan(0), 0x950412de, bigEndian: false);
        WriteUInt32(file.AsSpan(8), (uint)count, bigEndian: false);
        WriteUInt32(file.AsSpan(12), OriginalTable, bigEndian: false);
        WriteUInt32(file.AsSpan(16), (uint)translationTable, bigEndian: false);
        translation.CopyTo(file, translationOffset);

        for (int i = 0; i < count; i++)
        {
            byte[] name = Encoding.ASCII.GetBytes((distinctOriginals) ? i.ToString("D7", System.Globalization.CultureInfo.InvariantCulture) : "same");
            int nameOffset = strings + (i * 8);
            name.CopyTo(file, nameOffset);

            WriteUInt32(file.AsSpan(OriginalTable + (i * 8)), (uint)name.Length, bigEndian: false);
            WriteUInt32(file.AsSpan(OriginalTable + (i * 8) + 4), (uint)nameOffset, bigEndian: false);
            WriteUInt32(file.AsSpan(translationTable + (i * 8)), (uint)translation.Length, bigEndian: false);
            WriteUInt32(file.AsSpan(translationTable + (i * 8) + 4), (uint)translationOffset, bigEndian: false);
        }

        return file;
    }

    /// <summary>
    /// 4,096 entries with the same original that all point to one 64 KiB translation parse into a single entry, decoding the translation once and allocating under 8 MiB.
    /// </summary>
    [Fact]
    public void AliasedTranslationUnderOneOriginalIsDecodedOnceAndStaysCheap()
    {
        byte[] file = BuildAliased(4096, distinctOriginals: false);

        long before = GC.GetAllocatedBytesForCurrentThread();
        bool parsed = MoReader.TryParse(file, out IReadOnlyDictionary<string, string>? entries);
        long allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.True(parsed);
        Assert.NotNull(entries);
        Assert.Single(entries);
        Assert.True(allocated < 8L * 1024 * 1024, string.Create(CultureInfo.InvariantCulture, $"Allocated {allocated} bytes."));
    }

    /// <summary>
    /// 4,096 distinct originals that all point to one 64 KiB translation exceed the decoding budget and fail to parse, allocating under 8 MiB before giving up.
    /// </summary>
    [Fact]
    public void AliasedTranslationUnderDistinctOriginalsExceedsTheDecodingBudgetAndFails()
    {
        byte[] file = BuildAliased(4096, distinctOriginals: true);

        long before = GC.GetAllocatedBytesForCurrentThread();
        bool parsed = MoReader.TryParse(file, out IReadOnlyDictionary<string, string>? _);
        long allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.False(parsed);
        Assert.True(allocated < 8L * 1024 * 1024, string.Create(CultureInfo.InvariantCulture, $"Allocated {allocated} bytes."));
    }

    /// <summary>
    /// When an original appears twice, the first translation is kept.
    /// </summary>
    [Fact]
    public void DuplicateOriginalKeepsTheFirstTranslation()
    {
        byte[] file = Build([("Mechanic", "Mechaniker"), ("Mechanic", "Schrauber")], bigEndian: false);

        Assert.True(MoReader.TryParse(file, out IReadOnlyDictionary<string, string>? entries));
        Assert.NotNull(entries);
        Assert.Equal("Mechaniker", entries["Mechanic"]);
    }

    /// <summary>
    /// Reading a file that does not exist fails.
    /// </summary>
    [Fact]
    public void MissingFileFails()
    {
        string path = Path.Combine(Path.GetTempPath(), "tf3drpc-missing-" + Guid.NewGuid().ToString("N") + ".mo");

        Assert.False(MoReader.TryReadFile(path, out IReadOnlyDictionary<string, string>? _));
    }

    /// <summary>
    /// The real German file of an installed game contains known pairs such as "Mechanic" and "Mechaniker", and the test is skipped when the game is not at the default Steam location.
    /// </summary>
    [Fact]
    public void RealGermanFileContainsKnownPairs()
    {
        Assert.SkipUnless(File.Exists(RealGermanFile), "The game is not installed at the default Steam location.");

        bool parsed = MoReader.TryReadFile(RealGermanFile, out IReadOnlyDictionary<string, string>? entries);

        Assert.True(parsed);
        Assert.NotNull(entries);
        Assert.Equal("Mechaniker", entries["Mechanic"]);
        Assert.Equal("Sandkastenmodus", entries["MOD_SANDBOX_NAME"]);
        Assert.Equal("Laden...", entries["Loading..."]);
        Assert.Equal("Mardi Gras retten", entries["MISSION_01_NAME"]);
    }
}
