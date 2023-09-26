using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace PK.Utils.Synchronization;

/// <summary>
/// Extensions for <see cref="WaitHandle"/>
/// </summary>
[PublicAPI]
public static class WaitHandleExtensions
{
	private class RegisteredWaitHandleWrapper : IDisposable
	{
		[NotNull] private readonly RegisteredWaitHandle _registeredWaitHandle;
		[NotNull] private readonly WaitHandle _waitHandle;
		private readonly CancellationTokenRegistration _tokenRegistration;

		public RegisteredWaitHandleWrapper(
			[NotNull] RegisteredWaitHandle registeredWaitHandle,
			[NotNull] WaitHandle waitHandle,
			CancellationToken cancellationToken
			)
		{
			_registeredWaitHandle =
				registeredWaitHandle ?? throw new ArgumentNullException(nameof(registeredWaitHandle));
			_waitHandle = waitHandle ?? throw new ArgumentNullException(nameof(waitHandle));
			_tokenRegistration = cancellationToken.Register(
				_ =>
				{
					_registeredWaitHandle.Unregister(_waitHandle);
				},
				null);
		}

		#region IDisposable

		/// <inheritdoc />
		public void Dispose()
		{
			_registeredWaitHandle.Unregister(_waitHandle);
			_tokenRegistration.Dispose();
		}

		#endregion
	}

	/// <summary>
	/// Asynchronously wait for <see cref="WaitHandle"/>
	/// </summary>
	/// <param name="handle">Handle to wait for</param>
	/// <param name="millisecondsTimeout">Wait timeout in milliseconds</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>False on timeout, True otherwise</returns>
	public static async Task<bool> WaitOneAsync(
		this WaitHandle handle,
		int millisecondsTimeout,
		CancellationToken cancellationToken
		)
	{
		RegisteredWaitHandle registeredHandle = null;
		var tokenRegistration = default(CancellationTokenRegistration);
		try
		{
			var tcs = new TaskCompletionSource<bool>();
			registeredHandle = ThreadPool.RegisterWaitForSingleObject(
				handle,
				(state, timedOut) => ((TaskCompletionSource<bool>)state).TrySetResult(!timedOut),
				tcs,
				millisecondsTimeout,
				true);

			tokenRegistration = cancellationToken.Register(
				state => ((TaskCompletionSource<bool>)state).TrySetCanceled(),
				tcs);
			return await tcs.Task;
		}
		finally
		{
			registeredHandle?.Unregister(null);
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
			await tokenRegistration.DisposeAsync();
#else
				tokenRegistration.Dispose();
#endif
		}
	}

	/// <summary>
	/// Asynchronously wait for <see cref="WaitHandle"/>
	/// </summary>
	/// <param name="handle">Handle to wait for</param>
	/// <param name="timeout">Wait timeout</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>False on timeout, True otherwise</returns>
	public static Task<bool> WaitOneAsync(
		this WaitHandle handle,
		TimeSpan timeout,
		CancellationToken cancellationToken
		) => handle.WaitOneAsync((int)timeout.TotalMilliseconds, cancellationToken);

	/// <summary>
	/// Asynchronously wait for <see cref="WaitHandle"/>
	/// </summary>
	/// <param name="handle">Handle to wait for</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>Should be always True</returns>
	public static Task<bool> WaitOneAsync(this WaitHandle handle, CancellationToken cancellationToken) =>
		handle.WaitOneAsync(Timeout.Infinite, cancellationToken);

	/// <summary>
	/// Registers repeating callback for <see cref="WaitHandle"/>
	/// </summary>
	/// <param name="handle"><see cref="WaitHandle"/></param>
	/// <param name="callback">Callback to be executed on wait end</param>
	/// <param name="state">State parameter for callback</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns><see cref="IDisposable"/>, which unregisters callback on dispose</returns>
	public static IDisposable RegisterCallback(
		this WaitHandle handle,
		WaitOrTimerCallback callback,
		object state,
		CancellationToken cancellationToken
		)
	{
		var registeredHandle = ThreadPool.RegisterWaitForSingleObject(
			handle,
			callback,
			state,
			Timeout.Infinite,
			false);

		return new RegisteredWaitHandleWrapper(registeredHandle, handle, cancellationToken);
	}
}
