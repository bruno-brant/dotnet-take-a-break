// Copyright (c) Bruno Brant. All rights reserved.

using System;
using System.Threading;
using System.Windows.Forms;
using NSubstitute;
using TakeABreak.UI.Presenters;
using Xunit;

namespace TakeABreak.UI.Tests.PresenterTests;

public class TrayIconPresenterTests
{
	[Fact]
	public void MonitorUpdated_CallShowBalloonOnlyIfUserIsntAway()
	{
		var configuration = Substitute.For<ITrayIconConfiguration>();
		configuration.WarningInterval.Returns(TimeSpan.Zero);
		configuration.RestingMonitorUpdateInterval.Returns(TimeSpan.FromMilliseconds(50));

		var iconView = Substitute.For<ITrayIconView>();

		var model = Substitute.For<IRestingMonitor>();
		model.UserStatus.Returns(UserStatus.Rested);

		using var sut = new TrayIconPresenter(configuration, iconView, model);

		// ACT
		Thread.Sleep(TimeSpan.FromMilliseconds(100)); // make sure some time has been elapsed

		// ASSERT
		iconView
			.DidNotReceiveWithAnyArgs()
			.ShowBalloonTip(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ToolTipIcon>());
	}
}
