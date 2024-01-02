// Copyright (c) Bruno Brant. All rights reserved.

using System;
using System.Linq;
using AutoFixture;
using AutoFixture.Xunit2;
using Microsoft.Extensions.Logging;

namespace TakeABreak.Tests;

public class RestingMonitorTests
{
	private readonly IUserInteractionStatusProvider _userIdleMonitor = Substitute.For<IUserInteractionStatusProvider>();
	private readonly ISuspendMonitor _suspendMonitor = Substitute.For<ISuspendMonitor>();
	private readonly ILogger<RestingMonitor> _logger = Substitute.For<ILogger<RestingMonitor>>();
	private readonly Fixture _fixture = new();

	[Theory, AutoSubstituteData]
	public void Update_WhenStatusAlwaysBusy_TotalTimeEqualsElapsed(IRestingMonitorConfiguration configuration, TimeSpan elapsed)
	{
		configuration.InitialStatus.Returns(InteractionStatus.Busy);
		configuration.MaxBusyTime.Returns(elapsed);
		configuration.RestingTime.Returns(TimeSpan.MaxValue);

		_userIdleMonitor.GetStatus().Returns(InteractionStatus.Busy);

		var sut = new RestingMonitor(configuration, _userIdleMonitor, _suspendMonitor, _logger);

		sut.Update(elapsed);

		Assert.Equal(elapsed, sut.TotalBusyTimeSinceRested);
	}

	[Theory, AutoSubstituteData]
	public void Update_StatusChanged_TotalTimeEqualsZero(IRestingMonitorConfiguration configuration, TimeSpan elapsed)
	{
		// ARRANGE
		var invertedStatus = configuration.InitialStatus switch
		{
			InteractionStatus.Busy => InteractionStatus.Idle,
			InteractionStatus.Idle => InteractionStatus.Busy,
			_ => throw new Exception($"Unexpected {nameof(UserStatus)} value '{configuration.InitialStatus}'"),
		};

		_userIdleMonitor.GetStatus().Returns(invertedStatus);

		var sut = new RestingMonitor(configuration, _userIdleMonitor, _suspendMonitor, _logger);

		// ACT

		sut.Update(elapsed);

		// ASSERT

		Assert.Equal(TimeSpan.Zero, sut.TotalIdleTimeSinceRested);
		Assert.Equal(TimeSpan.Zero, sut.TotalBusyTimeSinceRested);
	}

	[Theory, AutoSubstituteData]
	public void Update_WhenInitialStatusIsBusyAndThenBusyAgain_TotalBusyTimeIsElapsed(IRestingMonitorConfiguration configuration, TimeSpan elapsed)
	{
		// ARRANGE
		configuration.InitialStatus.Returns(InteractionStatus.Busy);
		configuration.RestingTime.Returns(TimeSpan.MaxValue);

		_userIdleMonitor.GetStatus().Returns(InteractionStatus.Busy);

		var sut = new RestingMonitor(configuration, _userIdleMonitor, _suspendMonitor, _logger);

		// ACT
		sut.Update(elapsed);

		// ASSERT
		Assert.Equal(elapsed, sut.TotalBusyTimeSinceRested);
	}

	[Theory, AutoSubstituteData]
	public void Update_WhenInitialStatusIsIdleAndThenIdleAgain_TotalIdleTimeIsZero(IRestingMonitorConfiguration configuration, TimeSpan elapsed)
	{
		configuration.InitialStatus.Returns(InteractionStatus.Idle);
		_userIdleMonitor.GetStatus().Returns(InteractionStatus.Idle);

		var sut = new RestingMonitor(configuration, _userIdleMonitor, _suspendMonitor, _logger);

		sut.Update(elapsed);

		Assert.Equal(TimeSpan.Zero, sut.TotalIdleTimeSinceRested);
	}

	[Fact]
	public void Update_WhenTotalIdleTimeLargerThanRestTimePerBusyTime_TotalBusyTimeIsZero()
	{
		var configuration = Substitute.For<IRestingMonitorConfiguration>();

		configuration.InitialStatus.Returns(InteractionStatus.Busy);
		configuration.MaxBusyTime.Returns(TimeSpan.MaxValue);
		configuration.RestingTime.Returns(TimeSpan.FromSeconds(3));

		var busyTime = TimeSpan.FromSeconds(30);

		var sut = new RestingMonitor(configuration, _userIdleMonitor, _suspendMonitor, _logger);

		// Adds lots of busy time
		_userIdleMonitor.GetStatus().Returns(InteractionStatus.Busy);
		sut.Update(busyTime);

		// now rest for a while
		Enumerable
			.Range(0, 10)
			.ForEach(_ =>
			{
				_userIdleMonitor.GetStatus().Returns(InteractionStatus.Idle);
				sut.Update(TimeSpan.FromSeconds(1));
			});

		Assert.Equal(UserStatus.Rested, sut.UserStatus);
		Assert.Equal(TimeSpan.Zero, sut.TotalBusyTimeSinceRested);
		Assert.Equal(TimeSpan.Zero, sut.TotalIdleTimeSinceRested);
	}

	[Fact]
	public void Update_WhenTotalBusyIsZeroAndTotalIdleNotZeroAndUserIsNowBusy_TotalIdleIsZero()
	{
		var configuration = Substitute.For<IRestingMonitorConfiguration>();

		configuration.InitialStatus.Returns(InteractionStatus.Busy);
		configuration.MaxBusyTime.Returns(TimeSpan.MaxValue);
		configuration.RestingTime.Returns(TimeSpan.FromSeconds(3));

		var busyTime = TimeSpan.FromSeconds(30);

		var sut = new RestingMonitor(configuration, _userIdleMonitor, _suspendMonitor, _logger);

		// Adds lots of busy time
		_userIdleMonitor.GetStatus().Returns(InteractionStatus.Busy);
		sut.Update(busyTime);

		// now rest for a while
		_userIdleMonitor.GetStatus().Returns(InteractionStatus.Idle);
		sut.Update(TimeSpan.FromSeconds(10));

		// and now get busy
		_userIdleMonitor.GetStatus().Returns(InteractionStatus.Busy);
		sut.Update(TimeSpan.FromSeconds(1));

		Assert.Equal(UserStatus.Busy, sut.UserStatus);
		Assert.Equal(TimeSpan.Zero, sut.TotalIdleTimeSinceRested);
	}

	[Theory, AutoSubstituteData]
	public void Update_WhenTotalBusyLargerThanMaxBusyTime_ThenMustRestIsTrue(IRestingMonitorConfiguration configuration)
	{
		configuration.InitialStatus.Returns(InteractionStatus.Busy);
		configuration.RestingTime.Returns(TimeSpan.MaxValue);

		_userIdleMonitor.GetStatus().Returns(InteractionStatus.Busy);

		var sut = new RestingMonitor(configuration, _userIdleMonitor, _suspendMonitor, _logger);

		// Adds lots of busy time
		sut.Update(configuration.MaxBusyTime + TimeSpan.FromSeconds(1));

		Assert.Equal(UserStatus.Tired, sut.UserStatus);
	}

	[Theory, AutoSubstituteData]
	public void Update_WhenTotalBusyLesserThanMaxBusyTime_ThenMustRestIsFalse(IRestingMonitorConfiguration configuration)
	{
		_userIdleMonitor.GetStatus().Returns(InteractionStatus.Busy);
		configuration.InitialStatus.Returns(InteractionStatus.Busy);

		var sut = new RestingMonitor(configuration, _userIdleMonitor, _suspendMonitor, _logger);

		// Adds lots of busy time
		sut.Update(configuration.MaxBusyTime + new TimeSpan(-1));

		Assert.NotEqual(UserStatus.Tired, sut.UserStatus);
	}

	[Theory, AutoSubstituteData]
	public void Update_WhenComputerSuspendedSinceLastUpdate_ResetTimers(IRestingMonitorConfiguration configuration)
	{
		// ARRANGE
		var sut = new RestingMonitor(configuration, _userIdleMonitor, _suspendMonitor, _logger);

		// ACT

		// User is busy...
		_userIdleMonitor
			.GetStatus()
			.Returns(InteractionStatus.Busy);

		// ...for the past 60 seconds
		sut.Update(TimeSpan.FromSeconds(60));

		// Computer resumed 30 seconds ago
		_suspendMonitor
			.LastResumeTime
			.Returns(DateTime.Now - TimeSpan.FromSeconds(30));

		// The user is busy now
		_userIdleMonitor
			.GetStatus()
			.Returns(InteractionStatus.Busy);

		// It's been 30 seconds since the last update
		sut.Update(TimeSpan.FromSeconds(30));

		// ASSERT
		Assert.Equal(TimeSpan.Zero, sut.TotalBusyTimeSinceRested);
	}

	[Fact]
	public void Update_WhenComputerSuspendedAndNoTimeElapsed_ResetTimers()
	{
		// ARRANGE
		var configuration = Substitute.For<IRestingMonitorConfiguration>();
		configuration.InitialStatus.Returns(InteractionStatus.Busy);
		configuration.MaxBusyTime.Returns(TimeSpan.FromMinutes(25));
		configuration.RestingTime.Returns(TimeSpan.FromMinutes(5));

		var sut = new RestingMonitor(configuration, _userIdleMonitor, _suspendMonitor, _logger);

		// ACT

		// User is busy...
		_userIdleMonitor
			.GetStatus()
			.Returns(InteractionStatus.Busy);

		// And needs resting
		sut.Update(configuration.MaxBusyTime + TimeSpan.FromMinutes(2));

		// Computer was suspended just now
		_suspendMonitor
			.LastResumeTime
			.Returns(DateTime.Now);

		// Update right now
		sut.Update(TimeSpan.FromSeconds(0));

		// ASSERT
		Assert.NotEqual(UserStatus.Tired, sut.UserStatus);
	}
}
