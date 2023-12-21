// Copyright (c) Bruno Brant. All rights reserved.

using System;
using System.Windows.Forms;

namespace TakeABreak.UI
{
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
}

public class Startup
{
	/// <summary>
	/// Register the application to run on startup.
	/// </summary>
	public static void RegisterApplication()
	{
		using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

		key.SetValue("TakeABreak", Application.ExecutablePath);
	}

	public static bool IsRegistered()
	{
		using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

		var value = key.GetValue("TakeABreak");

		return key.GetValue("TakeABreak") == Application.ExecutablePath;
	}
}
