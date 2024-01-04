// Copyright (c) Bruno Brant. All rights reserved.

using System;
using System.Drawing;
using System.Windows.Forms;
using TakeABreak.UI.Presenters;

namespace TakeABreak.UI.Views;

/// <summary>
/// Implements an icon that is available at the system tray to control the
/// application.
/// </summary>
public partial class TrayIconView : UserControl, ITrayIconView
{
	/// <summary>
	/// Initializes a new instance of the <see cref="TrayIconView"/> class.
	/// </summary>
	/// <param name="restingMonitor">
	/// Dependency. Service that monitors the working and resting of users.
	/// </param>
	public TrayIconView(IRestingMonitor restingMonitor)
	{
		InitializeComponent();

#pragma warning disable CA2000 // Dispose objects before losing scope

		// TODO: inject Settings
		var trayIconPresenter = new TrayIconPresenter(Settings.Default, this, restingMonitor);
		components.Add(trayIconPresenter);

#pragma warning restore CA2000 // Dispose objects before losing scope
	}

	/// <inheritdoc/>
	public event EventHandler ShowConfigurationClicked
	{
		add { _configurationMenuItem.Click += value; }
		remove { _configurationMenuItem.Click -= value; }
	}

	/// <inheritdoc/>
	public event EventHandler ShowAboutClicked;

	/// <inheritdoc/>
	public event EventHandler PauseUnpauseClicked;

	/// <inheritdoc/>
	public event EventHandler ExitClicked
	{
		add { _exitMenuItem.Click += value; }
		remove { _exitMenuItem.Click -= value; }
	}

	/// <inheritdoc/>
	public Icon Icon
	{
		get
		{
			return _notifyIcon1.Icon;
		}

		set
		{
			Action impl = () => _notifyIcon1.Icon = value;

			if (InvokeRequired)
			{
				BeginInvoke(impl);
			}
			else
			{
				impl();
			}
		}
	}

	/// <inheritdoc/>
	public string Status
	{
		get
		{
			return _notifyIcon1.Text;
		}

		set
		{
			var impl = () => _notifyIcon1.Text = value;

			if (InvokeRequired)
			{
				BeginInvoke(impl);
			}
			else
			{
				impl();
			}
		}
	}

	/// <inheritdoc/>
	public void ShowBalloonTip(int timeout, string tipTitle, string tipText, ToolTipIcon tipIcon)
	{
		Action impl = () => _notifyIcon1.ShowBalloonTip(timeout, tipTitle, tipText, tipIcon);

		if (InvokeRequired)
		{
			BeginInvoke(impl);
		}
		else
		{
			impl();
		}
	}
}
