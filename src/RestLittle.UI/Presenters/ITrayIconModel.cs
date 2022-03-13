// Copyright (c) Bruno Brant. All rights reserved.

using System;
using RestLittle.UI.Models;

namespace RestLittle.UI.Presenters
{
	/// <summary>
	/// Model used by the <see cref="TrayIconPresenter"/>.
	/// </summary>
	public interface ITrayIconModel : IDisposable
	{
		/// <summary>
		/// Called whenever the resting monitor is updated.
		/// </summary>
		event EventHandler RestingMonitorUpdated;

		/// <summary>
		///     Gets the accumulated WORKING time since the user was last considered rested.
		/// </summary>
		/// <remarks>
		///     User is considered rested once he's idle for at least
		///     <see cref="IRestingMonitorConfiguration.RestingTime"/>.
		/// </remarks>
		TimeSpan BusyTimeSinceRested { get; }

		/// <summary>
		///     Gets the accumulated RESTING time since the user was last considered rested.
		/// </summary>
		/// <remarks>
		///     User is considered rested once he's idle for at least
		///     <see cref="IRestingMonitorConfiguration.RestingTime"/>.
		/// </remarks>
		TimeSpan IdleTimeSinceRested { get; }

		/// <summary>
		/// Gets the last status of the user.
		/// </summary>
		UserStatus LastStatus { get; }

		/// <summary>
		/// Gets a value indicating whether the user must rest.
		/// </summary>
		bool MustRest { get; }
	}
}
