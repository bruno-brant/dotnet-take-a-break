// Copyright (c) Bruno Brant. All rights reserved.

using System;
using System.Collections.Generic;

namespace TakeABreak.Tests;

public static class Helper
{
	public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
	{
		ArgumentNullException.ThrowIfNull(collection);

		foreach (var item in collection)
		{
			action(item);
		}
	}
}
