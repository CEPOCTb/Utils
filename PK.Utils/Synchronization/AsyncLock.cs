using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace PK.Utils.Synchronization;


/// <summary>
/// Asynchronous lock
/// </summary>
[PublicAPI]
public static class AsyncLock
{
	private static readonly ConcurrentDictionary<object, AsyncLockEntry> _lockDictionary = new();

	private class AsyncLockEntry
	{
		private readonly object _sync = new();
		private bool _acquired;
		private readonly Queue<TaskCompletionSource<IDisposable>> _queue = new();

		private class InternalLock : IDisposable
		{
			private readonly AsyncLockEntry _parent;

			public InternalLock(AsyncLockEntry parent)
			{
				_parent = parent ?? throw new ArgumentNullException(nameof(parent));
			}

			public void Dispose()
			{
				_parent.ReleaseLock();
			}
		}

		internal Task<IDisposable> AcquireLock()
		{
			lock (_sync)
			{
				if (!_acquired)
				{
					_acquired = true;

					return Task.FromResult<IDisposable>(new InternalLock(this));
				}

				var waiting = new TaskCompletionSource<IDisposable>();
				_queue.Enqueue(waiting);

				return waiting.Task;
			}
		}

		private void ReleaseLock()
		{
			lock (_sync)
			{
				if (_queue.Count > 0)
				{
					var next = _queue.Dequeue();
					next.SetResult(new InternalLock(this));
				}
				else
				{
					_acquired = false;
				}
			}
		}

	}

	private static AsyncLockEntry GetSyncObject(object obj) => _lockDictionary.GetOrAdd(obj, _ => new AsyncLockEntry());


	/// <summary>
	/// Execute action within asynchronous lock
	/// </summary>
	/// <param name="obj">Object to lock</param>
	/// <param name="action">Action to execute</param>
	public static async Task ExecuteWithLock(object obj, Func<Task> action)
	{
		using (await GetSyncObject(obj).AcquireLock().ConfigureAwait(false))
		{
			await action().ConfigureAwait(false);
		}
	}

	/// <summary>
	/// Get asynchronous lock
	/// </summary>
	/// <param name="obj">Object to lock</param>
	/// <returns>Lock <see cref="IDisposable"/> object, lock released on dispose</returns>
	public static async Task<IDisposable> Lock(object obj)
	{
		return await GetSyncObject(obj).AcquireLock().ConfigureAwait(false);
	}
}
