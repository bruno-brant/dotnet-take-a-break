// Copyright (c) Bruno Brant. All rights reserved.

using System;
using System.ComponentModel;
using System.Timers;
using System.Windows.Forms;
using TakeABreak.UI.Plumbing;
using TakeABreak.UI.Views;

namespace TakeABreak.UI.Presenters;

/// <summary>
/// A Presenter from MVP for the TrayIconView.
/// </summary>
public class TrayIconPresenter : IComponent
{
	/// <summary>
	/// Configuration for this presenter.
	/// </summary>
	private readonly ITrayIconConfiguration _configuration;

	/// <summary>
	/// The model of the presenter is the resting monitor.
	/// </summary>
	private readonly IRestingMonitor _restingMonitor;

	/// <summary>
	/// The view controlled by this presenter.
	/// </summary>
	private readonly ITrayIconView _trayIconView;

	/// <summary>
	/// The last time the warning was displayed.
	/// </summary>
	private DateTime _lastWarning = DateTime.Now;

	// The timer used to update the resting monitor
	private readonly ElapsedTimer _restingMonitorTimer;

	/// <summary>
	/// Informs that the component has been disposed.
	/// </summary>
	private bool _disposedValue;

	/// <summary>
	///     Initializes a new instance of the <see cref="TrayIconPresenter"/> class.
	/// </summary>
	/// <param name="configuration">
	///     The configuration for this presenter.
	/// </param>
	/// <param name="trayIconView">
	///    View that controls the tray icon.
	/// </param>
	/// <param name="trayIconModel">The model controlled by this presenter.</param>
	public TrayIconPresenter(ITrayIconConfiguration configuration, ITrayIconView trayIconView, IRestingMonitor trayIconModel)
	{
		_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
		_trayIconView = trayIconView ?? throw new ArgumentNullException(nameof(trayIconView));
		_restingMonitor = trayIconModel ?? throw new ArgumentNullException(nameof(trayIconModel));

		_trayIconView.ShowConfigurationClicked += TrayIconView_ShowConfigurationClicked;

		// Update the icon every 10 seconds
		_restingMonitorTimer = new(RestingMonitorUpdate, _configuration.RestingMonitorUpdateInterval);
	}

	/// <inheritdoc/>
	public event EventHandler Disposed;

	/// <inheritdoc/>
	public ISite Site { get; set; }

	/// <inheritdoc/>
	public void Dispose()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	/// <summary>
	///     Dispose pattern implementation.
	/// </summary>
	/// <param name="disposing">
	///     Whether this was called by disposing or by the destructor.
	/// </param>
	protected virtual void Dispose(bool disposing)
	{
		if (!_disposedValue)
		{
			if (disposing)
			{
				// TODO: dispose managed state (managed objects)
			}

			Disposed?.Invoke(this, EventArgs.Empty);

			_disposedValue = true;
		}
	}

	/// <summary>
	/// Get the corresponding icon.
	/// </summary>
	/// <param name="status">Current status.</param>
	/// <returns>The correponding icon.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// When the status has no corresponding icon.
	/// </exception>
	private static UserStatusIcon GetIcon(UserStatus status)
	{
		// TODO: this should be moved to UserStatusIcon
		return status switch
		{
			UserStatus.Rested => UserStatusIcon.Resting,
			UserStatus.Busy => UserStatusIcon.Working,
			UserStatus.Tired => UserStatusIcon.Tired,
			_ => throw new ArgumentOutOfRangeException(nameof(status), status, $"No corresponding icon."),
		};
	}

	private void TrayIconView_ShowConfigurationClicked(object sender, EventArgs e)
	{
		using var cfv = new ConfigurationFormView();

		cfv.ShowDialog();
	}

	private void RestingMonitorUpdate(TimeSpan elapsed)
	{
		_restingMonitor.Update(elapsed);

		var icon = GetIcon(_restingMonitor.UserStatus).ToIcon();

		// avoid repainting the icon
		if (_trayIconView.Icon != icon)
		{
			_trayIconView.Icon = icon;
		}

		var busyElapsed = _restingMonitor.TotalBusyTimeSinceRested.ToFriendlyString();
		var idleElapsed = _restingMonitor.TotalIdleTimeSinceRested.ToFriendlyString();
		_trayIconView.Status = $"Busy for {busyElapsed} and idle for {idleElapsed}".Truncate(63, true);

		if (_lastWarning.UntilNow() > _configuration.WarningInterval
			&& _restingMonitor.UserStatus == UserStatus.Tired)
		{
			var tipText = $"Please go rest! You haven't rested for {busyElapsed}.";
			_trayIconView.ShowBalloonTip(5000, "Must rest!", tipText, ToolTipIcon.Warning);

			_lastWarning = DateTime.Now;
		}
	}
}
