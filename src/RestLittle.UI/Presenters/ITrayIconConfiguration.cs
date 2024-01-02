// Copyright (c) Bruno Brant. All rights reserved.

using System;

namespace TakeABreak.UI.Presenters
{
	/// <summary>
	/// Configuration for <see cref="ITrayIconView"/>.
	/// </summary>
	public interface ITrayIconConfiguration
	{
		/// <summary>
		/// Gets the delay between consecutive warnings.
		/// </summary>
		TimeSpan WarningInterval { get; }

		/// <summary>
		/// Gets the interval between updates to the <see cref="IRestingMonitor"/>.
		/// </summary>
		TimeSpan RestingMonitorUpdateInterval { get; }
	}
}
