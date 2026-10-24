using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Tf3Drpc.Core.Translation;

using Xunit;

namespace Tf3Drpc.Core.Tests.Translation;

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
        WriteUInt32(file.AsSpan(12), (uint)originalTable, bigEndian);
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

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ValidFile_YieldsOnlyPlainEntries(bool bigEndian)
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

    [Fact]
    public void EmptyFileWithZeroEntries_Parses()
    {
        byte[] file = Build([], bigEndian: false);

        Assert.True(MoReader.TryParse(file, out IReadOnlyDictionary<string, string>? entries));
        Assert.Empty(entries);
    }

    [Fact]
    public void EveryTruncation_FailsWithoutThrowing()
    {
        byte[] file = Build(SampleEntries, bigEndian: false);

        for (int length = 0; length < file.Length; length++)
        {
            bool parsed = MoReader.TryParse(file.AsSpan(0, length), out IReadOnlyDictionary<string, string>? entries);

            if (!parsed)
                Assert.Null(entries);
        }

        Assert.False(MoReader.TryParse(file.AsSpan(0, 20), out IReadOnlyDictionary<string, string>? _));
        Assert.False(MoReader.TryParse(file.AsSpan(0, 40), out IReadOnlyDictionary<string, string>? _));
    }

    [Fact]
    public void BadMagic_Fails()
    {
        byte[] file = Build(SampleEntries, bigEndian: false);
        file[0] ^= 0xFF;

        Assert.False(MoReader.TryParse(file, out IReadOnlyDictionary<string, string>? _));
    }

    [Fact]
    public void UnsupportedMajorRevision_Fails()
    {
        byte[] file = Build(SampleEntries, bigEndian: false);
        BinaryPrimitives.WriteUInt32LittleEndian(file.AsSpan(4), 1u << 16);

        Assert.False(MoReader.TryParse(file, out IReadOnlyDictionary<string, string>? _));
    }

    [Fact]
    public void HugeEntryCount_Fails()
    {
        byte[] file = Build(SampleEntries, bigEndian: false);
        BinaryPrimitives.WriteUInt32LittleEndian(file.AsSpan(8), uint.MaxValue);

        Assert.False(MoReader.TryParse(file, out IReadOnlyDictionary<string, string>? _));
    }

    [Fact]
    public void TableOffsetPastTheEnd_Fails()
    {
        byte[] file = Build(SampleEntries, bigEndian: false);
        BinaryPrimitives.WriteUInt32LittleEndian(file.AsSpan(12), (uint)file.Length);

        Assert.False(MoReader.TryParse(file, out IReadOnlyDictionary<string, string>? _));
    }

    [Fact]
    public void StringOffsetPastTheEnd_Fails()
    {
        byte[] file = Build(SampleEntries, bigEndian: false);
        BinaryPrimitives.WriteUInt32LittleEndian(file.AsSpan(28 + 8 + 4), (uint)file.Length);

        Assert.False(MoReader.TryParse(file, out IReadOnlyDictionary<string, string>? _));
    }

    [Theory]
    [InlineData(uint.MaxValue)]
    [InlineData(int.MaxValue)]
    public void HugeStringLength_Fails(uint length)
    {
        byte[] file = Build(SampleEntries, bigEndian: false);
        BinaryPrimitives.WriteUInt32LittleEndian(file.AsSpan(28 + 8), length);

        Assert.False(MoReader.TryParse(file, out IReadOnlyDictionary<string, string>? _));
    }

    [Fact]
    public void OversizedInput_Fails()
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

    [Fact]
    public void AliasedTranslationUnderOneOriginal_IsDecodedOnceAndStaysCheap()
    {
        byte[] file = BuildAliased(4096, distinctOriginals: false);

        long before = GC.GetAllocatedBytesForCurrentThread();
        bool parsed = MoReader.TryParse(file, out IReadOnlyDictionary<string, string>? entries);
        long allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.True(parsed);
        Assert.NotNull(entries);
        Assert.Single(entries);
        Assert.True(allocated < 8L * 1024 * 1024, $"Allocated {allocated} bytes.");
    }

    [Fact]
    public void AliasedTranslationUnderDistinctOriginals_ExceedsTheDecodingBudgetAndFails()
    {
        byte[] file = BuildAliased(4096, distinctOriginals: true);

        long before = GC.GetAllocatedBytesForCurrentThread();
        bool parsed = MoReader.TryParse(file, out IReadOnlyDictionary<string, string>? _);
        long allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.False(parsed);
        Assert.True(allocated < 8L * 1024 * 1024, $"Allocated {allocated} bytes.");
    }

    [Fact]
    public void DuplicateOriginal_KeepsTheFirstTranslation()
    {
        byte[] file = Build([("Mechanic", "Mechaniker"), ("Mechanic", "Schrauber")], bigEndian: false);

        Assert.True(MoReader.TryParse(file, out IReadOnlyDictionary<string, string>? entries));
        Assert.NotNull(entries);
        Assert.Equal("Mechaniker", entries["Mechanic"]);
    }

    [Fact]
    public void MissingFile_Fails()
    {
        string path = Path.Combine(Path.GetTempPath(), "tf3drpc-missing-" + Guid.NewGuid().ToString("N") + ".mo");

        Assert.False(MoReader.TryReadFile(path, out IReadOnlyDictionary<string, string>? _));
    }

    [Fact]
    public void RealGermanFile_ContainsKnownPairs()
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
