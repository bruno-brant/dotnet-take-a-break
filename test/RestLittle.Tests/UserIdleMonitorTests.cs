// Copyright (c) Bruno Brant. All rights reserved.

using System;
using NSubstitute;
using Xunit;

namespace TakeABreak.Tests;

public class UserIdleMonitorTests
{
	private readonly IInputObserver _inputManager = Substitute.For<IInputObserver>();

	[Theory, AutoSubstituteData]
	public void GetStatus_WhenLastTimeIsLessThanTimeToIdle_StatusIsBusy(IUserIdleMonitorConfiguration configuration)
	{
		var returnThis = DateTime.Now - (configuration.TimeToIdle - TimeSpan.FromMinutes(1));

		_inputManager
			.GetLastInputTime()
			.Returns(returnThis);

		var sut = new UserIdleMonitor(configuration, _inputManager);

		var actual = sut.GetStatus();

		Assert.Equal(InteractionStatus.Busy, actual);
	}

	[Theory, AutoSubstituteData]
	public void GetStatus_WhenLastTimeIsLargerThanMaxBusyTime_StatusIsIdle(IUserIdleMonitorConfiguration configuration)
	{
		var returnThis = DateTime.Now - (configuration.TimeToIdle + TimeSpan.FromMinutes(1));

		_inputManager
			.GetLastInputTime()
			.Returns(returnThis);

		var sut = new UserIdleMonitor(configuration, _inputManager);

		var actual = sut.GetStatus();

		Assert.Equal(InteractionStatus.Idle, actual);
	}
}
