using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace PK.Utils.Factories;

/// <summary>
/// Implementation of <see cref="IDisposable"/> value wrapper
/// </summary>
/// <typeparam name="T">Value type</typeparam>
[PublicAPI]
public class AsyncScopedValue<T> : IScopedValue<T>
{
	private readonly Func<T, ValueTask> _disposeAction;
	private readonly IDisposable _disposable;
	private volatile bool _disposed;

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="value">Value</param>
	/// <param name="disposeAction">Action to perform on dispose</param>
	public AsyncScopedValue(T value, Func<T, ValueTask> disposeAction)
	{
		Value = value;
		_disposeAction = disposeAction;
	}

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="value">Value</param>
	/// <param name="scope">Disposable scope</param>
	public AsyncScopedValue(T value, IDisposable scope)
	{
		Value = value;
		_disposable = scope;
	}

	#region Implementation of IScopedValue<out T>

	/// <inheritdoc />
	public T Value { get; }

	#endregion

	#region Implementation of IDisposable

	/// <inheritdoc />
	public void Dispose()
	{
		Dispose(true).GetAwaiter().GetResult();
	}

	private async ValueTask Dispose(bool disposing)
	{
		// will not dispose wrapped value - leave it for the client code
		// (the lifetime of value can be managed by DI container, for example)

		if (_disposed)
		{
			return;
		}

		_disposed = true;
		try
		{
			if (_disposeAction != null)
			{
				await _disposeAction.Invoke(Value).ConfigureAwait(false);
			}
		} catch (Exception ex) {}

		try
		{
			if (_disposable != null)
			{
				if (_disposable is IAsyncDisposable asyncDisposable)
				{
					await asyncDisposable.DisposeAsync().ConfigureAwait(false);
				}
				else
				{
					_disposable.Dispose();
				}
			}
		} catch (Exception ex) {}

		if (disposing)
		{
			GC.SuppressFinalize(this);
		}
	}

	/// <inheritdoc />
	~AsyncScopedValue() => Dispose(false).GetAwaiter().GetResult();

	/// <inheritdoc />
	public ValueTask DisposeAsync() => Dispose(true);

	#endregion
}
