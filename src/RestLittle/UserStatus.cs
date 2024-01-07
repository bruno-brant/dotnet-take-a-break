// Copyright (c) Bruno Brant. All rights reserved.

namespace TakeABreak;

/// <summary>
/// Informs if the user is using the computer or not.
/// </summary>
public enum UserStatus
{
	/// <summary>
	/// The user is idle.
	/// </summary>
	Rested,

	/// <summary>
	/// The user is busy.
	/// </summary>
	Busy,

	/// <summary>
	/// The user is tired and must rest.
	/// </summary>
	Tired,
}
