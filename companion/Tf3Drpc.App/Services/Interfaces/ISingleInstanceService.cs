using System;

namespace Tf3Drpc.App.Services.Interfaces;

/// <summary>
/// Makes sure only one instance runs; a second start hands over to the first.
/// </summary>
internal interface ISingleInstanceService
{
    /// <summary>
    /// Raised in the running instance when another start was redirected to it.
    /// </summary>
    event EventHandler? ActivationRequested;
}
