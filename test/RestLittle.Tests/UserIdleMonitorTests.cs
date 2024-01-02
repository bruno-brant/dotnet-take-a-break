// Copyright (c) Bruno Brant. All rights reserved.

using System;
using TakeABreak.Infra;

namespace TakeABreak.Tests;

public class UserIdleMonitorTests
{
	private readonly IInputObserver _inputManager = Substitute.For<IInputObserver>();

	[Theory, AutoSubstituteData]
	public void GetStatus_WhenLastTimeIsLessThanTimeToIdle_StatusIsBusy(IUserInteractionProviderConfiguration configuration)
	{
		var returnThis = DateTime.Now - (configuration.TimeToIdle - TimeSpan.FromMinutes(1));

		_inputManager
			.GetLastInputTime()
			.Returns(returnThis);

		var sut = new UserInteractionStatusProvider(configuration, _inputManager);

		var actual = sut.GetStatus();

		Assert.Equal(InteractionStatus.Busy, actual);
	}

	[Theory, AutoSubstituteData]
	public void GetStatus_WhenLastTimeIsLargerThanMaxBusyTime_StatusIsIdle(IUserInteractionProviderConfiguration configuration)
	{
		var returnThis = DateTime.Now - (configuration.TimeToIdle + TimeSpan.FromMinutes(1));

		_inputManager
			.GetLastInputTime()
			.Returns(returnThis);

		var sut = new UserInteractionStatusProvider(configuration, _inputManager);

		var actual = sut.GetStatus();

		Assert.Equal(InteractionStatus.Idle, actual);
	}
}
