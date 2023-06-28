// Copyright (c) Bruno Brant. All rights reserved.

using System;
using System.ComponentModel;
using System.Windows.Forms;
using TakeABreak.UI.Models;
using TakeABreak.UI.Plumbing;
using TakeABreak.UI.Views;

namespace TakeABreak.UI.Presenters
{
	/// <summary>
	/// A Presenter from MVP for the TrayIconView.
	/// </summary>
	public class TrayIconPresenter : IComponent
	{
		/// <summary>
		/// The model that this presenter uses.
		/// </summary>
		private readonly ITrayIconModel _trayIconModel;

		/// <summary>
		/// Configuration for this presenter.
		/// </summary>
		private readonly ITrayIconViewConfiguration _configuration;

		/// <summary>
		/// The view controlled by this presenter.
		/// </summary>
		private readonly ITrayIconView _trayIconView;

		/// <summary>
		/// The last time the warning was displayed.
		/// </summary>
		private DateTime _lastWarning = DateTime.Now;

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
		public TrayIconPresenter(ITrayIconViewConfiguration configuration, ITrayIconView trayIconView, ITrayIconModel trayIconModel)
		{
			_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
			_trayIconView = trayIconView ?? throw new ArgumentNullException(nameof(trayIconView));
			_trayIconModel = trayIconModel ?? throw new ArgumentNullException(nameof(trayIconModel));

			_trayIconModel.RestingMonitorUpdated += RestingMonitorModel_RestingMonitorUpdated;

			_trayIconView.ShowConfigurationClicked += TrayIconView_ShowConfigurationClicked;
			_trayIconView.PauseUnpauseClicked += TrayIconView_PauseUnpauseClicked;
			_trayIconView.ShowAboutClicked += TrayIconView_ShowAboutClicked;
		}

		/// <summary>
		/// Raised when the user has expressed the desire to exit the application.
		/// </summary>
		public event EventHandler ExitClicked
		{
			add { _trayIconView.ExitClicked += value; }
			remove { _trayIconView.ExitClicked -= value; }
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
				UserStatus.Resting => UserStatusIcon.Resting,
				UserStatus.Working => UserStatusIcon.Working,
				UserStatus.Tired => UserStatusIcon.Tired,
				_ => throw new ArgumentOutOfRangeException(nameof(status), status, $"No corresponding icon."),
			};
		}

		private void TrayIconView_ShowConfigurationClicked(object sender, EventArgs e)
		{
			using var cfv = new ConfigurationFormView();

			cfv.ShowDialog();
		}

		private void TrayIconView_ShowAboutClicked(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}

		private void TrayIconView_PauseUnpauseClicked(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}

		private void RestingMonitorModel_RestingMonitorUpdated(object sender, EventArgs e)
		{
			var busyElapsed = _trayIconModel.BusyTimeSinceRested.ToFriendlyString();
			var idleElapsed = _trayIconModel.IdleTimeSinceRested.ToFriendlyString();
			var icon = GetIcon(_trayIconModel.LastStatus);

			// avoid repainting the icon
			if (_trayIconView.Icon != icon.ToIcon())
			{
				_trayIconView.Icon = icon;
			}

			_trayIconView.Status = $"You've been busy for {busyElapsed} and idle for {idleElapsed}".Truncate(63, true);

			if (_trayIconModel.MustRest
				&& _trayIconModel.LastStatus != UserStatus.Resting
				&& _lastWarning.UntilNow() > _configuration.WarningInterval)
			{
				var tipText = $"Please go rest! You haven't rested for {busyElapsed}.";
				_trayIconView.ShowBalloonTip(5000, "Must rest!", tipText, ToolTipIcon.Warning);

				_lastWarning = DateTime.Now;
			}
		}
	}
}
