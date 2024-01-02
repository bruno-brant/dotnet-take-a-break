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

			Settings.Default.AskToRegisterForStartup = false;

			Settings.Default.Save();
		}
	}
}
