// Copyright (c) Bruno Brant. All rights reserved.

using System;

namespace TakeABreak.UI.Presenters
{
	/// <summary>
	/// Configuration for <see cref="ITrayIconView"/>.
	/// </summary>
	public interface ITrayIconViewConfiguration
	{
		/// <summary>
		/// Gets the delay between consecutive warnings.
		/// </summary>
		TimeSpan WarningInterval { get; }
	}
}
