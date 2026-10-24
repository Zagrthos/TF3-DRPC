using System;
using System.IO;
using System.IO.Pipes;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

using Tf3Drpc.Core.Constants;

namespace Tf3Drpc.Core.Discord;

/// <summary>
/// Connects to Discord through a named pipe on Windows and a Unix socket elsewhere.
/// </summary>
internal sealed class DiscordConnector : IDiscordConnector
{
    private const string PipeDirectoryPrefix = @"\\.\pipe\";

    /// <inheritdoc/>
    public async ValueTask<Stream?> ConnectAsync(CancellationToken cancellationToken)
    {
        for (int index = 0; index is < DiscordIpcConstants.EndpointCount; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string name = DiscordIpcConstants.EndpointPrefix + index.ToString(System.Globalization.CultureInfo.InvariantCulture);
            Stream? stream = (OperatingSystem.IsWindows())
                ? await TryConnectPipeAsync(name, cancellationToken)
                : await TryConnectSocketAsync(name, cancellationToken);

            if (stream is not null)
                return stream;
        }

        return null;
    }

    private static async Task<Stream?> TryConnectPipeAsync(string name, CancellationToken cancellationToken)
    {
        // Connecting to a pipe that does not exist waits for the whole timeout, so skip it.
        if (!File.Exists(PipeDirectoryPrefix + name))
            return null;

        NamedPipeClientStream pipe = new(".", name, PipeDirection.InOut, PipeOptions.Asynchronous);

        try
        {
            await pipe.ConnectAsync(DiscordIpcConstants.ConnectTimeoutMilliseconds, cancellationToken);

            return pipe;
        }
        catch (Exception exception) when (IsConnectFailure(exception) && !cancellationToken.IsCancellationRequested)
        {
            await pipe.DisposeAsync();

            return null;
        }
        catch (OperationCanceledException)
        {
            await pipe.DisposeAsync();

            throw;
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "The socket is owned by the returned NetworkStream or disposed on failure.")]
    private static async Task<Stream?> TryConnectSocketAsync(string name, CancellationToken cancellationToken)
    {
        Socket socket = new(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);

        try
        {
            string path = System.IO.Path.Combine(GetSocketDirectory(), name);
            using CancellationTokenSource timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeout.CancelAfter(DiscordIpcConstants.ConnectTimeoutMilliseconds);
            await socket.ConnectAsync(new UnixDomainSocketEndPoint(path), timeout.Token);

            return new NetworkStream(socket, ownsSocket: true);
        }
        catch (Exception exception) when (IsConnectFailure(exception) && !cancellationToken.IsCancellationRequested)
        {
            socket.Dispose();

            return null;
        }
        catch (OperationCanceledException)
        {
            socket.Dispose();

            throw;
        }
    }

    private static string GetSocketDirectory()
    {
        string[] variables = ["XDG_RUNTIME_DIR", "TMPDIR", "TMP", "TEMP"];

        for (int i = 0; i < variables.Length; i++)
        {
            string? value = Environment.GetEnvironmentVariable(variables[i]);

            if (!string.IsNullOrEmpty(value))
                return value;
        }

        return DiscordIpcConstants.FallbackDirectory;
    }

    private static bool IsConnectFailure(Exception exception)
        => exception is IOException or TimeoutException or UnauthorizedAccessException or SocketException or ArgumentException
            or OperationCanceledException or InvalidOperationException or NotSupportedException;
}
