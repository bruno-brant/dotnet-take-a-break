// Copyright (c) Bruno Brant. All rights reserved.

using Microsoft.Extensions.Logging;

namespace TakeABreak;

/// <summary>
/// Checks whether the user is properly rested.
/// </summary>
/// <param name="configuration">
///     Configuration of this service.
/// </param>
/// <param name="userIdleMonitor">
///     Used to check whether the user is currently idle or is busy.
/// </param>
/// <param name="suspendMonitor">
///     Used to check when the computer was suspended.
/// </param>
/// <param name="logger">
///     Used to log diagnostic messages.
/// </param>
public class RestingMonitor(
	IRestingMonitorConfiguration configuration, IUserInteractionStatusProvider userIdleMonitor, ISuspendMonitor suspendMonitor, ILogger<RestingMonitor> logger)
	: IRestingMonitor
{
	private readonly IRestingMonitorConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
	private readonly IUserInteractionStatusProvider _userIdleMonitor = userIdleMonitor ?? throw new ArgumentNullException(nameof(userIdleMonitor));
	private readonly ISuspendMonitor _suspendMonitor = suspendMonitor ?? throw new ArgumentNullException(nameof(suspendMonitor));
	private readonly ILogger<RestingMonitor> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

	/// <summary>
	///     Holds the statuses for elapsed time.
	/// </summary>
	/// <remarks>
	///     While a dictionary is an overkill here, I used it to simplify my code
	///     by being able to call _elapsed[Status] instead of having to switch
	///     all the time.
	/// </remarks>
	private readonly Dictionary<InteractionStatus, TimeSpan> _elapsedTimeOnStatusSinceLastRested = new()
	{
		[InteractionStatus.Busy] = TimeSpan.Zero,
		[InteractionStatus.Idle] = TimeSpan.Zero,
	};

	// Monitor the last time we checked the status.
	private DateTime _lastResumeTime = DateTime.MinValue;

	// The last status that the user was in since the last update.
	private InteractionStatus _lastStatus = configuration.InitialStatus;

	/// <summary>
	/// Gets the last status of the user.
	/// </summary>
	public UserStatus UserStatus
	{
		get
		{
			if (_elapsedTimeOnStatusSinceLastRested[InteractionStatus.Busy] > _configuration.MaxBusyTime)
			{
				return UserStatus.Tired;
			}

			if (_lastStatus == InteractionStatus.Busy)
			{
				return UserStatus.Busy;
			}

			return UserStatus.Rested;
		}
	}

	/// <summary>
	/// Gets the accumulated RESTING time since the user was last considered rested.
	/// </summary>
	/// <remarks>
	/// User is considered rested once he's idle for at least
	/// <see cref="IRestingMonitorConfiguration.RestingTime"/>.
	/// </remarks>
	public TimeSpan TotalIdleTimeSinceRested => _elapsedTimeOnStatusSinceLastRested[InteractionStatus.Idle];

	/// <summary>
	///     Gets the accumulated BUSY time since the user was last considered rested.
	/// </summary>
	/// <remarks>
	///     User is considered rested once he's idle for at least
	///     <see cref="IRestingMonitorConfiguration.RestingTime"/>.
	/// </remarks>
	public TimeSpan TotalBusyTimeSinceRested => _elapsedTimeOnStatusSinceLastRested[InteractionStatus.Busy];

	/// <inheritdoc/>
	public void Update(TimeSpan elapsed)
	{
		var currentStatus = _userIdleMonitor.GetStatus();

		// skip updating the elapsed time if this is the first time we get this status
		// in this way, we avoid updating when the user was partially on the previous status
		if (currentStatus == _lastStatus)
		{
			_elapsedTimeOnStatusSinceLastRested[currentStatus] += elapsed;
		}

		_lastStatus = currentStatus;

		// Reset monitors if the computer was suspended since last time we checked
		if (_lastResumeTime < _suspendMonitor.LastResumeTime)
		{
			_logger.LogInformation("Computer was suspended. Resetting elapsed time.");

			_lastResumeTime = _suspendMonitor.LastResumeTime;

			ResetElapsed();
		} // If the user busy time is zero, then so is the idle time.
		else if (TotalBusyTimeSinceRested == TimeSpan.Zero)
		{
			_elapsedTimeOnStatusSinceLastRested[InteractionStatus.Idle] = TimeSpan.Zero;
		} // If the user idle time is greater than the resting time, then he's rested.
		else if (TotalIdleTimeSinceRested >= _configuration.RestingTime)
		{
			ResetElapsed();
		}
	}

	private void ResetElapsed()
	{
		_elapsedTimeOnStatusSinceLastRested[InteractionStatus.Busy] = TimeSpan.Zero;
		_elapsedTimeOnStatusSinceLastRested[InteractionStatus.Idle] = TimeSpan.Zero;
	}
}
