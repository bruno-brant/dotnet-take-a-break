// Copyright (c) Bruno Brant. All rights reserved.

namespace TakeABreak;

/// <summary>
/// Objects capable of obtaining input information.
/// </summary>
public interface IInputObserver
{
	/// <summary>
	///     Gets the time when the user last used the mouse or the keyboard.
	/// </summary>
	/// <returns>
	///     A <see cref="DateTime"/> instance of the last time the user used the input devices.
	/// </returns>
	DateTime GetLastInputTime();
}
