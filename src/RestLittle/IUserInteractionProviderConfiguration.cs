// Copyright (c) Bruno Brant. All rights reserved.

namespace TakeABreak;

/// <summary>
/// Configuration for service <see cref="IUserInteractionStatusProvider"/>.
/// </summary>
public interface IUserInteractionProviderConfiguration
{
	/// <summary>
	/// Gets the minimum amount of time without using the computer that is considered to be idle.
	/// </summary>
	TimeSpan TimeToIdle { get; }
}
