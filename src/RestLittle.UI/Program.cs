// Copyright (c) Bruno Brant. All rights reserved.

using System;
using System.Windows.Forms;

namespace TakeABreak.UI;

/// <summary>
/// Main program entry point.
/// </summary>
public static class Program
{
	/// <summary>
	///  The main entry point for the application.
	/// </summary>
	[STAThread]
	private static void Main()
	{
		RegisterApplicationForStartup();

		// Prevent multiple instances of the application
		using var mutex = new System.Threading.Mutex(true, "TakeABreak+FA403F85-44B1-4B15-9422-631661453E14", out var createdNew);

		if (!createdNew)
		{
			MessageBox.Show("Another instance of this application is already running.", "Take A Break", MessageBoxButtons.OK, MessageBoxIcon.Information);

			return;
		}

		Application.SetHighDpiMode(HighDpiMode.SystemAware);
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);

		using var context = new TakeBreakApplicationContext();

		Application.Run(context);
	}

	private static void RegisterApplicationForStartup()
	{
		if (Settings.Default.AskToRegisterForStartup && !Startup.IsRegistered())
		{
			// Show dialog and ask user for confirmation
			var result = MessageBox.Show(
				"Would you like to run this application automatically on startup?",
				"Run on startup", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

				if (result == DialogResult.Yes)
				{
					Startup.RegisterApplication();
				} 
				else
				{
					Settings.Default.AskToRegisterForStartup = false;
					Settings.Default.Save();
				}
			}
		}
	}
}

public class Startup
{
	// The registry key where the startup applications are stored
	private const string StartupApps = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

	// The name of the application
	private const string AppKey = "TakeABreak";

	/// <summary>
	/// Register the application to run on startup.
	/// </summary>
	public static void RegisterApplication()
	{
		using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(StartupApps, true);

		key.SetValue(AppKey, Application.ExecutablePath);
	}

	/// <summary>
	/// Check if the application is registered to run on startup.
	/// </summary>
	/// <returns>
	/// <see cref="true"/> if the application is registered to run on startup; <see cref="false"/> otherwise."
	/// </returns>
	public static bool IsRegistered()
	{
		using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(StartupApps, true);

		var value = key.GetValue(AppKey);

		return value == Application.ExecutablePath;
	}
}
