// Copyright (c) Bruno Brant. All rights reserved.

using System.Runtime.Versioning;
using Microsoft.Win32;

namespace TakeABreak;

/// <summary>
/// Implementation of <see cref="ISuspendMonitor"/> that uses the <see cref="SystemEvents.PowerModeChanged"/> event.
/// </summary>
[SupportedOSPlatform("windows")]
public class SuspendMonitor : ISuspendMonitor, IDisposable
{
	private bool _disposedValue;

	/// <summary>
	/// Initializes a new instance of the <see cref="SuspendMonitor"/> class.
	/// </summary>
	public SuspendMonitor()
	{
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
		if (e.Mode == PowerModes.Resume)
		{
			LastResumeTime = DateTime.Now;
		}
	}
}
