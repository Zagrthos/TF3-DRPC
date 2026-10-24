using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Tf3Drpc.Core.Constants;
using Tf3Drpc.Core.Structs;

namespace Tf3Drpc.Core.GameLog;

/// <summary>
/// Reads the new complete lines of a growing log file, one poll at a time.
/// </summary>
public sealed class GameLogFollower
{
    private const byte LineFeed = (byte)'\n';
    private const byte CarriageReturn = (byte)'\r';
    private static readonly UTF8Encoding Utf8 = new(encoderShouldEmitUTF8Identifier: false);
    private string? _path;
    private long _position;
    private byte[] _pending = [];
    private byte[] _head = [];
    private bool _atFileStart = true;
    private bool _skippingLine;

    /// <summary>
    /// Gets the number of bytes of the unfinished line kept for the next poll.
    /// </summary>
    internal int PendingBytes
        => _pending.Length;

    /// <summary>
    /// Reads the lines completed since the last poll; I/O errors give an empty result.
    /// </summary>
    public async Task<GameLogReadResultStruct> PollAsync(string path, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        bool restarted = false;

        if (!string.Equals(_path, path, StringComparison.Ordinal))
        {
            restarted = _path is not null;
            _path = path;
            Restart();
        }

        try
        {
            await using FileStream stream = new
            (
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite | FileShare.Delete,
                bufferSize: 1,
                useAsync: true
            );

            long length = stream.Length;

            if (length < _position || !await HeadMatchesAsync(stream, length, cancellationToken))
            {
                restarted = true;
                Restart();
            }

            long available = length - _position;
            int count = (int)Math.Min(available, GameLogConstants.MaxReadBytesPerPoll);

            if (count is 0)
                return new GameLogReadResultStruct() { Lines = [], Restarted = restarted };

            byte[] buffer = new byte[_pending.Length + count];
            _pending.CopyTo(buffer, 0);
            stream.Seek(_position, SeekOrigin.Begin);
            await stream.ReadExactlyAsync(buffer.AsMemory(_pending.Length, count), cancellationToken);

            // Commit the state only once the lines exist, so a failure while splitting does not lose the batch.
            List<string> lines = SplitLines(buffer, out byte[] pending, out bool skippingLine, out bool atFileStart);

            RememberHead(buffer.AsSpan(_pending.Length, count));
            _position += count;
            _pending = pending;
            _skippingLine = skippingLine;
            _atFileStart = atFileStart;

            return new GameLogReadResultStruct() { Lines = lines, Restarted = restarted };
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            return new GameLogReadResultStruct() { Lines = [], Restarted = restarted };
        }
    }

    private async Task<bool> HeadMatchesAsync(FileStream stream, long length, CancellationToken cancellationToken)
    {
        if (_head.Length is 0)
            return true;

        if (length < _head.Length)
            return false;

        byte[] current = new byte[_head.Length];
        stream.Seek(0, SeekOrigin.Begin);
        await stream.ReadExactlyAsync(current, cancellationToken);

        return current.AsSpan().SequenceEqual(_head);
    }

    private void RememberHead(ReadOnlySpan<byte> bytes)
    {
        int missing = GameLogConstants.HeadFingerprintBytes - _head.Length;

        if (missing <= 0)
            return;

        int take = Math.Min(missing, bytes.Length);
        byte[] extended = new byte[_head.Length + take];
        _head.CopyTo(extended, 0);
        bytes[..take].CopyTo(extended.AsSpan(_head.Length));
        _head = extended;
    }

    private void Restart()
    {
        _position = 0;
        _pending = [];
        _head = [];
        _atFileStart = true;
        _skippingLine = false;
    }

    private List<string> SplitLines(byte[] buffer, out byte[] pending, out bool skippingLine, out bool atFileStart)
    {
        ReadOnlySpan<byte> span = buffer;
        skippingLine = _skippingLine;
        atFileStart = _atFileStart;

        if (skippingLine)
        {
            int skipEnd = span.IndexOf(LineFeed);

            if (skipEnd is < 0)
            {
                pending = [];

                return [];
            }

            skippingLine = false;
            span = span[(skipEnd + 1)..];
        }

        int lastLineFeed = span.LastIndexOf(LineFeed);

        if (lastLineFeed is < 0)
        {
            KeepPending(span, out pending, out skippingLine);

            return [];
        }

        ReadOnlySpan<byte> complete = span[..(lastLineFeed + 1)];
        KeepPending(span[(lastLineFeed + 1)..], out pending, out skippingLine);

        // The list grows on demand: the number of line breaks says nothing about the memory a poll may reserve.
        List<string> lines = [];

        while (!complete.IsEmpty)
        {
            int end = complete.IndexOf(LineFeed);
            ReadOnlySpan<byte> line = complete[..end];
            complete = complete[(end + 1)..];

            if (line.Length > GameLogConstants.MaxLineBytes)
                continue;

            if (!line.IsEmpty && (line[^1] is CarriageReturn))
                line = line[..^1];

            string text = Utf8.GetString(line);

            if (atFileStart)
            {
                atFileStart = false;
                text = text.TrimStart('\uFEFF');
            }

            lines.Add(text);
        }

        return lines;
    }

    private static void KeepPending(ReadOnlySpan<byte> unfinished, out byte[] pending, out bool skippingLine)
    {
        if (unfinished.Length > GameLogConstants.MaxLineBytes)
        {
            pending = [];
            skippingLine = true;

            return;
        }

        pending = unfinished.ToArray();
        skippingLine = false;
    }
}
