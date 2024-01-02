// Copyright (c) Bruno Brant. All rights reserved.

namespace TakeABreak.Infra;

/// <summary>
/// Implementation of <see cref="IInputObserver"/> using Win32 API.
/// </summary>
public class Win32InputObserver : IInputObserver
{
	/// <inheritdoc/>
	public DateTime GetLastInputTime()
	{
		var lastInputInfo = new NativeMethods.LASTINPUTINFO
		{
			CbSize = NativeMethods.LASTINPUTINFO.Size,
		};

		if (!NativeMethods.GetLastInputInfo(ref lastInputInfo))
		{
			throw new Exception("Couldn't get last time.");
		}

		var tickCountElapseSinceInputTime = Environment.TickCount - lastInputInfo.DwTime;

		return DateTime.Now.AddMilliseconds(-tickCountElapseSinceInputTime);
	}
}
