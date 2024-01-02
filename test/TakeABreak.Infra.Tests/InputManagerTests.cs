// Copyright (c) Bruno Brant. All rights reserved.

using TakeABreak.Infra;

namespace TakeABreak.Tests;

public class InputManagerTests
{
	[Fact]
	public void GetLastInputTime_ReturnsValidDateTime()
	{
		var sut = new Win32InputObserver();
		var time = sut.GetLastInputTime();

		// BUG: this may fail when running in CI
		Assert.InRange(time, DateTime.Now.AddDays(-1), DateTime.Now);
	}
}
