// Copyright (c) Bruno Brant. All rights reserved.

using System;
using System.ComponentModel;
using System.Timers;
using System.Windows.Forms;
using TakeABreak.UI.Plumbing;
using TakeABreak.UI.Views;

namespace TakeABreak.UI.Presenters;

internal class ElapsedTimer : IDisposable
{
	private bool disposedValue;
	private DateTime _lastUpdate;
	private Update _update;
	private System.Threading.Timer _timer;

	/// <summary>
	/// Called when the timer ticks.
	/// </summary>
	/// <param name="elapsed">
	/// The elapsed time since the last update.
	/// </param>
	public delegate void Update(TimeSpan elapsed);

	/// <summary>
	/// Initializes a new instance of the <see cref="ElapsedTimer"/> class.
	/// </summary>
	/// <param name="update">What to do during an update. Gets the elapsed time </param>
	/// <param name="interval"></param>
	public ElapsedTimer(Update update, TimeSpan interval)
	{
		_lastUpdate = DateTime.Now;
		_update = update;
		_timer = new((state) => Tick(), null, TimeSpan.Zero, interval);
	}

	private void Tick()
	{
		var elapsed = DateTime.Now - _lastUpdate;
		_update(elapsed);
		_lastUpdate = DateTime.Now;
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				_timer.Dispose();
			}

			disposedValue = true;
		}
	}

	public void Dispose()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}