// Copyright (c) Bruno Brant. All rights reserved.

namespace TakeABreak;

/// <inheritdoc/>
public class InputObserver : IInputObserver
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
