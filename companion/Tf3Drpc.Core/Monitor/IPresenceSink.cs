using System.Threading;
using System.Threading.Tasks;

using Tf3Drpc.Core.Structs;

namespace Tf3Drpc.Core.Monitor;

/// <summary>
/// Takes the desired presence and tells whether Discord is connected.
/// </summary>
public interface IPresenceSink
{
    /// <summary>
    /// Tells whether Discord is connected.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Brings the presence in line with the desired content; <see langword="null"/> clears it.
    /// </summary>
    Task UpdateAsync(PresenceContentStruct? desired, CancellationToken cancellationToken);
}
