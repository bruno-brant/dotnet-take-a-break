// Copyright (c) Bruno Brant. All rights reserved.

using System;
using System.Threading;
using System.Windows.Forms;
using NSubstitute;
using RestLittle.UI.Presenters;
using Xunit;

namespace RestLittle.UI.Tests.PresenterTests
{
	public class TrayIconPresenterTests
	{
		[Fact]
		public void MonitorUpdated_CallShowBalloonOnlyIfUserIsntAway()
		{
			var configuration = Substitute.For<ITrayIconViewConfiguration>();
			configuration.WarningInterval.Returns(TimeSpan.Zero);

			var iconView = Substitute.For<ITrayIconView>();

			var model = Substitute.For<ITrayIconModel>();
			model.MustRest.Returns(true);
			model.LastStatus.Returns(Models.UserStatus.Resting);

			using var sut = new TrayIconPresenter(configuration, iconView, model);

			// ACT
			Thread.Sleep(1); // make sure some time has been elapsed
			model.RestingMonitorUpdated += Raise.Event();

			// ASSERT
			iconView.DidNotReceiveWithAnyArgs().ShowBalloonTip(0, null, null, Arg.Any<ToolTipIcon>());
		}
	}
}
