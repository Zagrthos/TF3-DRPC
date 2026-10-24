using System;

using Tf3Drpc.Core.Monitor;

namespace Tf3Drpc.App.Services.Interfaces;

/// <summary>
/// The connection to Discord; disposing it clears the presence.
/// </summary>
internal interface IDiscordService : IPresenceSink, IAsyncDisposable;
