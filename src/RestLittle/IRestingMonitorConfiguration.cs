// Copyright (c) Bruno Brant. All rights reserved.

using System;

namespace TakeABreak;

/// <summary>
/// Configuration for <see cref="RestingMonitor"/>.
/// </summary>
public interface IRestingMonitorConfiguration
{
	/// <summary>
	/// Gets maximum amount of time using the computer without rest.
	/// </summary>
	public TimeSpan MaxBusyTime { get; }

	/// <summary>
	/// Gets how long the user must rest in a interval of maxBusyTime.
	/// </summary>
	public TimeSpan RestingTime { get; }

	/// <summary>
	/// Gets the initial status of the user (whether he's idle or busy).
	/// </summary>
	public InteractionStatus InitialStatus { get; }
}
