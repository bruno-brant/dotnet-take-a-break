// Copyright (c) Bruno Brant. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TakeABreak.UI.Plumbing;
using Xunit;

namespace TakeABreak.UI.Tests.PlumbingTests;

public class TimeSpanValidatorAttributeTests
{
	[Fact]
	public void IsValid_WhenObjectIsNull_ReturnsTrue()
	{
		var sut = new TimeSpanValidatorAttribute();
		var actual = sut.IsValid(null);

		Assert.True(actual);
	}

	[Fact]
	public void IsValid_WhenObjectNotTimeSpan_ReturnsFalse()
	{
		var sut = new TimeSpanValidatorAttribute();
		var actual = sut.IsValid(new object());

		Assert.False(actual);
	}

	[Fact]
	public void IsValid_WhenOnlyMinValueSet_ThenAllowsAnyValueAboveIt()
	{
		var sut = new TimeSpanValidatorAttribute
		{
			MinValue = "00:00:01",
		};

		Assert.True(sut.IsValid(TimeSpan.FromSeconds(2)));
		Assert.True(sut.IsValid(TimeSpan.MaxValue));
		Assert.False(sut.IsValid(TimeSpan.FromSeconds(0)));
	}

	[Fact]
	public void IsValid_WhenOnlyMaxValueSet_ThenAllowsAnyValueBelowIt()
	{
		var sut = new TimeSpanValidatorAttribute
		{
			MinValue = "00:00:01",
		};

		Assert.True(sut.IsValid(TimeSpan.FromSeconds(2)));
		Assert.True(sut.IsValid(TimeSpan.MaxValue));
		Assert.False(sut.IsValid(TimeSpan.FromSeconds(0)));
	}

	[Fact]
	public void IsValid_WhenTimeSpanInRange_ReturnsTrue()
	{
		var sut = new TimeSpanValidatorAttribute
		{
			MinValue = TimeSpan.FromSeconds(1).ToString(),
			MaxValue = TimeSpan.FromSeconds(5).ToString(),
		};

		var actual = sut.IsValid(TimeSpan.FromSeconds(3));

		Assert.True(actual);
	}

	[Fact]
	public void IsValid_WhenTimeSpanOutsideRange_ReturnsFalse()
	{
		var sut = new TimeSpanValidatorAttribute
		{
			MinValue = TimeSpan.FromSeconds(1).ToString(),
			MaxValue = TimeSpan.FromSeconds(5).ToString(),
		};

		var actual = sut.IsValid(TimeSpan.FromSeconds(6));

		Assert.False(actual);
	}

	[Fact]
	public void IsValid_WhenTimeSpanOutsideRange_NameTheCorrectMember()
	{
		// ARRANGE
		var foo = new Foo { Bar = TimeSpan.Zero };

		// ACT
		var actual = Validator2.TryValidateObject(foo, out var validationResults);

		// ASSERT
		Assert.False(actual);
		var result = Assert.Single(validationResults);

		Assert.Equal(nameof(Foo.Bar), result.MemberNames.Single());
	}

	private class Foo
	{
		[TimeSpanValidator(MinValue = "00:00:01")]
		public TimeSpan Bar { get; set; }
	}
}
