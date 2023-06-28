// Copyright (c) Bruno Brant. All rights reserved.

namespace TakeABreak;

/// <summary>
///    Provides the last time the computer resumed from sleep.
/// </summary>
public interface ISuspendMonitor
{
	/// <summary>
	///     Gets the last time the computer resumed from sleep.
	/// </summary>
	DateTime LastResumeTime { get; }
}
