using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Tf3Drpc.Core.Discord;

/// <summary>
/// Opens a duplex stream to the running Discord client.
/// </summary>
internal interface IDiscordConnector
{
    /// <summary>
    /// Returns a connected stream, or <see langword="null"/> if Discord is not running.
    /// </summary>
    ValueTask<Stream?> ConnectAsync(CancellationToken cancellationToken);
}
