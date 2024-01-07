// Copyright (c) Bruno Brant. All rights reserved.

using System.Runtime.Versioning;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;

namespace TakeABreak;

/// <summary>
///     Implementation of <see cref="ISuspendMonitor"/> that uses the <see cref="SystemEvents.PowerModeChanged"/> event.
/// </summary>
[SupportedOSPlatform("windows")]
public class SuspendMonitor : ISuspendMonitor, IDisposable
{
	private readonly ILogger<SuspendMonitor> _logger;
	private bool _disposedValue;

	/// <summary>
	///     Initializes a new instance of the <see cref="SuspendMonitor"/> class.
	/// </summary>
	/// <param name="logger">
	///     Used to log diagnostic messages.
	/// </param>
	public SuspendMonitor(ILogger<SuspendMonitor> logger)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));

		SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
	}

	/// <inheritdoc/>
	public DateTime LastResumeTime { get; private set; } = DateTime.MinValue;

	/// <inheritdoc/>
	public void Dispose()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	/// <summary>
	/// Dispose managed and unmanaged resources.
	/// </summary>
	/// <param name="disposing">
	/// True if called from <see cref="Dispose()"/>, false if called from finalizer.
	/// </param>
	protected virtual void Dispose(bool disposing)
	{
		if (!_disposedValue)
		{
			if (disposing)
			{
				SystemEvents.PowerModeChanged -= SystemEvents_PowerModeChanged;
			}

			_disposedValue = true;
		}
	}

	private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
	{
		_logger.LogDebug("Power mode changed to {PowerMode}.", e.Mode);

		if (e.Mode == PowerModes.Resume)
		{
			LastResumeTime = DateTime.Now;
		}
	}
}
