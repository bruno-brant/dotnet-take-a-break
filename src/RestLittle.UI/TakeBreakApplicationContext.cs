// Copyright (c) Bruno Brant. All rights reserved.

using Microsoft.Extensions.Logging;
using Serilog.Core;
using Serilog;
using System;
using System.Windows.Forms;
using TakeABreak.UI.Views;
using TakeABreak.Infra;

namespace TakeABreak.UI;

/// <summary>
/// This class acts like a controller for the whole application.
/// </summary>
public class TakeBreakApplicationContext : ApplicationContext
{
	// The format string for the Serilog logger.
	private const string OutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}";

	// Displays the tray icon.
	private readonly TrayIconView _trayIconView;

	/// <summary>
	/// Initializes a new instance of the <see cref="TakeBreakApplicationContext"/> class.
	/// </summary>
	public TakeBreakApplicationContext()
	{
		var configuration = new LoggerConfiguration()
			.MinimumLevel.Debug()
			.WriteTo.File("takeabreak.log", outputTemplate: OutputTemplate, rollingInterval: RollingInterval.Day)
			.CreateLogger();

		var loggerFactory = new LoggerFactory();
		loggerFactory.AddSerilog(configuration);

		_trayIconView = new TrayIconView(
			new RestingMonitor(
				Settings.Default,
				new UserInteractionStatusProvider(
					Settings.Default,
					new Win32InputObserver()),
				new SuspendMonitor(
					loggerFactory.CreateLogger<SuspendMonitor>()),
				loggerFactory.CreateLogger<RestingMonitor>()));

		_trayIconView.ExitClicked += TrayIconView_ExitClicked;
	}

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			_trayIconView.Dispose();
		}

		base.Dispose(disposing);
	}

	private void TrayIconView_ExitClicked(object sender, EventArgs e)
	{
		ExitThread();
	}
}
