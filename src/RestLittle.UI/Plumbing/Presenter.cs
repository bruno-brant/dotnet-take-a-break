// Copyright (c) Bruno Brant. All rights reserved.

using System;

namespace TakeABreak.UI.Plumbing;

/// <summary>
///     Base classe for presenters.
/// </summary>
/// <typeparam name="TView">
///     The type of the view used by this presenter.
/// </typeparam>
/// <typeparam name="TModel">
///     The type of the model used by this presenter.
/// </typeparam>
/// <remarks>
///     Presenter is a class that manages a view and a model. It is responsible for
///     updating the view with the data from the model, and updating the model with
///     actions from the view.
///     
///     See <see href="https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93presenter">
///     Model-View-Presenter (Wikipedia)</see>
/// </remarks>
public abstract class Presenter<TView, TModel> : IPresenter
	where TView : IDisposable
{
	private bool _disposedValue;

	/// <summary>
	/// Initializes a new instance of the <see cref="Presenter{TView, TModel}"/> class.
	/// </summary>
	/// <param name="view">The view that is managed by this presenter.</param>
	/// <param name="model">The model that holds the data of the view.</param>
	protected Presenter(TView view, TModel model)
	{
		View = view;
		Model = model;
	}

	/// <inheritdoc/>
	public abstract event EventHandler Unload;

	/// <summary>
	/// Gets the current view of the presenter.
	/// </summary>
	protected TView View { get; private set; }

	/// <summary>
	/// Gets the current model for this presenter.
	/// </summary>
	protected TModel Model { get; private set; }

	/// <inheritdoc/>
	public void Dispose()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	/// <summary>
	/// Disposable pattern.
	/// </summary>
	/// <param name="disposing">If called through disposing.</param>
	protected virtual void Dispose(bool disposing)
	{
		if (!_disposedValue)
		{
			if (disposing)
			{
				// TODO: dispose managed state (managed objects)
				View.Dispose();
			}

			// TODO: free unmanaged resources (unmanaged objects) and override finalizer
			// TODO: set large fields to null
			View = default;
			_disposedValue = true;
		}
	}
}
