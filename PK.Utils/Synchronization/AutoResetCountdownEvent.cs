using System;
using System.Threading;
using JetBrains.Annotations;

namespace PK.Utils.Synchronization
{
	/// <summary>
	/// CountDownEvent implementation with auto-reset support
	/// </summary>
	[PublicAPI]
	public class AutoResetCountdownEvent : IDisposable
	{
		private volatile bool _disposed;
		private volatile int _count;
		private readonly ManualResetEventSlim _event;
		private readonly object _lock = new object();

		/// <summary>
		/// Constructor
		/// </summary>
		public AutoResetCountdownEvent() : this(0)
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="count">Initial count</param>
		/// <exception cref="ArgumentOutOfRangeException">Throws if initial count is less than 0</exception>
		public AutoResetCountdownEvent(int count)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(count));
			}

			_count = count;
			_disposed = false;
			_event = new ManualResetEventSlim(count == 0);
		}


		/// <summary>
		/// <see cref="WaitHandle"/> object for this event
		/// </summary>
		public WaitHandle WaitHandle
		{
			get
			{
				CheckDisposed();
				return _event.WaitHandle;
			}
		}

		/// <summary>
		/// Current count
		/// </summary>
		public int CurrentCount => Math.Max(0, _count);

		/// <summary>
		/// Returns whether event is set 
		/// </summary>
		public bool IsSet => _event.IsSet;

		/// <summary>
		/// Signals an event to decrement counter and set, if reaches 0
		/// </summary>
		/// <returns>True if counter reached 0</returns>
		/// <exception cref="InvalidOperationException">throws if counter goes below 0 for some reason. Should never happen.</exception>
		public bool Signal()
		{
			CheckDisposed();

			var count = Interlocked.Decrement(ref _count);

			if (count == 0)
			{
				lock (_lock)
				{
					// double check to prevent double set
					// and to avoid situations with quick set-reset "vibrations"
					// when count quickly changes near 0-1
					// This means event be not be set even if counter reaches 0, when
					// it was incremented again in a very small amount of time
					if (_count == 0 && !_event.IsSet)
					{
						_event.Set();
					}
				}

				return true;
			}

			if (count < 0)
			{
				throw new InvalidOperationException("AutoResetCountdownEvent count can't be less than zero");
			}

			return false;
		}


		/// <summary>
		/// Increments counter
		/// </summary>
		/// <returns>True if count > 0</returns>
		/// <exception cref="InvalidOperationException">Throws when count overflows <see cref="int.MaxValue"/></exception>
		public bool TryAddCount()
		{
			CheckDisposed();

			if (_count == int.MaxValue)
			{
				throw new InvalidOperationException("AutoResetCountdownEvent count can't be more than " + int.MaxValue);
			}

			var count = Interlocked.Increment(ref _count);

			if (count <= 0)
			{
				return false;
			}

			if (_event.IsSet)
			{
				lock (_lock)
				{
					// double check to prevent double reset
					// and to avoid situation with quick set-reset "vibrations",
					// when count quickly changes near 0-1
					// This means event be not be reset even if counter increases > 0, when
					// it was reached 0 again in a very small amount of time
					if (_count > 0 && _event.IsSet)
					{
						_event.Reset();
					}
				}
			}

			return true;
		}

		/// <summary>
		/// Increments counter
		/// </summary>
		/// <exception cref="InvalidOperationException">Throws when count overflows <see cref="int.MaxValue"/> or was less than zero</exception>
		public void AddCount()
		{
			if (!TryAddCount())
			{
				throw new InvalidOperationException("AutoResetCountdownEvent count less than zero");
			}
		}

		/// <summary>
		/// Resets event and set counter to 0
		/// </summary>
		public void Reset()
		{
			CheckDisposed();

			lock (_lock)
			{
				_count = 0;

				if (_event.IsSet)
				{
					_event.Reset();
				}
			}
		}

		/// <summary>
		/// Resets counter to a given count, and sets or resets event, depending on previews and new state 
		/// </summary>
		/// <param name="count">New count</param>
		/// <exception cref="ArgumentOutOfRangeException">Throws if new count less than zero</exception>
		public void Reset(int count)
		{
			CheckDisposed();

			if (count < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(count));
			}

			lock (_lock)
			{
				_count = count;

				if (count > 0 && _event.IsSet)
				{
					_event.Reset();
				}
				else if (count == 0 && !_event.IsSet)
				{
					_event.Set();
				}
			}
		}

		/// <summary>
		/// <see cref="M:System.Threading.ManualResetEventSlim.Wait"/>
		/// </summary>
		public void Wait() => Wait(Timeout.Infinite, CancellationToken.None);

		/// <summary>
		/// <see cref="M:System.Threading.ManualResetEventSlim.Wait(System.Threading.CancellationToken)"/>
		/// </summary>
		public void Wait(CancellationToken token) => Wait(Timeout.Infinite, token);

		/// <summary>
		/// <see cref="M:System.Threading.ManualResetEventSlim.Wait(System.TimeSpan)"/>
		/// </summary>
		public bool Wait(TimeSpan timeout)
		{
			var ms = (long)timeout.TotalMilliseconds;
			if (ms < -1 || ms > int.MaxValue)
			{
				throw new ArgumentOutOfRangeException(nameof(timeout));
			}

			return Wait((int)ms, CancellationToken.None);
		}

		/// <summary>
		/// <see cref="M:System.Threading.ManualResetEventSlim.Wait(System.TimeSpan,System.Threading.CancellationToken)"/>
		/// </summary>
		public bool Wait(TimeSpan timeout, CancellationToken token)
		{
			var ms = (long)timeout.TotalMilliseconds;
			if (ms < -1 || ms > int.MaxValue)
			{
				throw new ArgumentOutOfRangeException(nameof(timeout));
			}

			return Wait((int)ms, token);
		}

		/// <summary>
		/// <see cref="M:System.Threading.ManualResetEventSlim.Wait(System.Int32)"/>
		/// </summary>
		public bool Wait(int timeout) => Wait(timeout, CancellationToken.None);

		/// <summary>
		/// <see cref="M:System.Threading.ManualResetEventSlim.Wait(System.Int32,System.Threading.CancellationToken)"/>
		/// </summary>
		public bool Wait(int timeout, CancellationToken token)
		{
			CheckDisposed();

			if (timeout < -1)
			{
				throw new ArgumentOutOfRangeException(nameof(timeout));
			}

			token.ThrowIfCancellationRequested();

			return _event.Wait(timeout, token);
		}

		/// <inheritdoc/>>
		public void Dispose()
		{
			var disposed = _disposed;
			if (!disposed)
			{
				_disposed = true;
				_event.Dispose();
			}
		}

		private void CheckDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(nameof(AutoResetEvent));
			}
		}
	}
}
