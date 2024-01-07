// Copyright (c) Bruno Brant. All rights reserved.

namespace TakeABreak;

/// <summary>
/// Object that monitors user interaction and knows for how long he's been
/// working and how long he's rested.
/// </summary>
public interface IRestingMonitor
{
	/// <summary>
	/// Gets the last status of the user.
	/// </summary>
	UserStatus UserStatus { get; }

	/// <summary>
	///     Gets the accumulated WORKING time since the user was last considered rested.
	/// </summary>
	TimeSpan TotalBusyTimeSinceRested { get; }

	/// <summary>
	/// Gets the accumulated RESTING time since the user was last considered rested.
	/// </summary>
	TimeSpan TotalIdleTimeSinceRested { get; }

	/// <summary>
	///     Updates the current status.
	/// </summary>
	/// <param name="elapsed">
	///     How much time elapsed since the last call to <see cref="Update(TimeSpan)"/>.
	/// </param>
	void Update(TimeSpan elapsed);
}
