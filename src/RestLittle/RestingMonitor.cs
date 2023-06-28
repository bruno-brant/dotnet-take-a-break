// Copyright (c) Bruno Brant. All rights reserved.

namespace TakeABreak;

/// <summary>
/// Checks whether the user is properly rested.
/// </summary>
public class RestingMonitor : IRestingMonitor
{
	private readonly IRestingMonitorConfiguration _configuration;
	private readonly IUserIdleMonitor _userIdleMonitor;
	private readonly ISuspendMonitor _suspendMonitor;

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

	/// <summary>
	///     Initializes a new instance of the <see cref="RestingMonitor"/> class.
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
	public RestingMonitor(IRestingMonitorConfiguration configuration, IUserIdleMonitor userIdleMonitor, ISuspendMonitor suspendMonitor)
	{
		_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
		_userIdleMonitor = userIdleMonitor ?? throw new ArgumentNullException(nameof(userIdleMonitor));
		_suspendMonitor = suspendMonitor ?? throw new ArgumentNullException(nameof(suspendMonitor));

		LastStatus = configuration.InitialStatus;
	}

	/// <summary>
	/// Gets the last status of the user.
	/// </summary>
	public InteractionStatus LastStatus { get; private set; }

	/// <summary>
	/// Gets time passed since the last status is the current.
	/// </summary>
	public TimeSpan TimeSinceLastStatus => _elapsedTimeOnStatusSinceLastRested[LastStatus];

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
	public bool MustRest => TotalBusyTimeSinceRested > _configuration.MaxBusyTime;

	/// <inheritdoc/>
	public void Update(TimeSpan elapsed)
	{
		// Reset monitors if the computer was suspended since last time we checked
		if (_lastResumeTime < _suspendMonitor.LastResumeTime)
		{
			_lastResumeTime = _suspendMonitor.LastResumeTime;

			ResetElapsed();
		}

		var currentStatus = _userIdleMonitor.GetStatus();

		// skip updating the elapsed time if this is the first time we get this status
		// in this way, we avoid updating when the user was partially on the previous status
		if (currentStatus == LastStatus)
		{
			_elapsedTimeOnStatusSinceLastRested[currentStatus] += elapsed;
		}

		// Reset monitors if the user isn't working
		if (_elapsedTimeOnStatusSinceLastRested[InteractionStatus.Busy] == TimeSpan.Zero
			|| TotalIdleTimeSinceRested > _configuration.RestingTime)
		{
			ResetElapsed();
		}

		LastStatus = currentStatus;
	}

	private void ResetElapsed()
	{
		_elapsedTimeOnStatusSinceLastRested[InteractionStatus.Busy] = TimeSpan.Zero;
		_elapsedTimeOnStatusSinceLastRested[InteractionStatus.Idle] = TimeSpan.Zero;
	}
}
