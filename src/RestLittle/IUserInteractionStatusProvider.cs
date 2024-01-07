// Copyright (c) Bruno Brant. All rights reserved.

namespace TakeABreak;

/// <summary>
/// Monitors the current status of the user - whether he's using the computer or not.
/// </summary>
public interface IUserInteractionStatusProvider
{
	/// <summary>
	///     Gets the current usage status of the computer.
	/// </summary>
	/// <returns>
	///    <see cref="InteractionStatus.Busy"/> if the user is using the computer,
	///    <see cref="InteractionStatus.Idle"/> otherwise.
	/// </returns>
	InteractionStatus GetStatus();
}
