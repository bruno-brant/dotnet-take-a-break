// Copyright (c) Bruno Brant. All rights reserved.

using System;
using System.Windows.Forms;

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
