using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using PK.Utils.Synchronization;

namespace PK.Utils;

/// <summary>
/// Class, that periodically updates some resource value
/// </summary>
/// <typeparam name="T">Resource type</typeparam>
[PublicAPI]
public class RenewableResource<T> : IDisposable
{
	[NotNull] private readonly Func<CancellationToken, Task<T>> _factory;
	[NotNull] private readonly ILogger<RenewableResource<T>> _logger;
	private volatile Task<T> _renewTask;
	[NotNull] private readonly CancellationTokenSource _cancellation = new CancellationTokenSource();
	private readonly AutoResetEvent _manualRenewEvent = new AutoResetEvent(false);

	/// <summary>
	/// Renew interval
	/// </summary>
	public TimeSpan Interval { get; set; }

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="factory">Async factory method for creation of resource</param>
	/// <param name="interval">Update interval</param>
	/// <param name="logger">Logger instance</param>
	public RenewableResource(
		[NotNull] Func<CancellationToken, Task<T>> factory,
		TimeSpan interval,
		[NotNull] ILogger<RenewableResource<T>> logger
		)
	{
		_factory = factory ?? throw new ArgumentNullException(nameof(factory));
		Interval = interval;
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		Start();
	}

	/// <summary>
	/// Force renew of resource
	/// </summary>
	public void Renew()
	{
		_manualRenewEvent.Set();
	}

	private void Start()
	{
		_renewTask = _factory(_cancellation.Token);
		_renewTask.ContinueWith(
			async task =>
			{
				_logger.LogError(task.Exception, "Start failed");
				await Task.WhenAny(
						_manualRenewEvent.WaitOneAsync(_cancellation.Token),
						Task.Delay(TimeSpan.FromMinutes(1), _cancellation.Token)
						)
					.ConfigureAwait(false);
				if (!_cancellation.Token.IsCancellationRequested)
				{
					Start();
				}
			},
			_cancellation.Token,
			TaskContinuationOptions.OnlyOnFaulted,
			TaskScheduler.Current
			);

		_renewTask.ContinueWith(
			async task =>
			{
				await Task.WhenAny(
					_manualRenewEvent.WaitOneAsync(_cancellation.Token),
					Task.Delay(Interval, _cancellation.Token)
					).ConfigureAwait(false);
				if (!_cancellation.Token.IsCancellationRequested)
				{
					RenewInternal();
				}
			},
			_cancellation.Token,
			TaskContinuationOptions.OnlyOnRanToCompletion,
			TaskScheduler.Current
			);
	}

	private void RenewInternal()
	{
		var renewTask = _factory(_cancellation.Token);
		renewTask.ContinueWith(
			task => { _renewTask = task; },
			_cancellation.Token,
			TaskContinuationOptions.OnlyOnRanToCompletion,
			TaskScheduler.Current
			);
		renewTask.ContinueWith(
				async task =>
				{
					if (!task.IsCompleted && task.Exception != null)
					{
						if (task.IsFaulted)
						{
							_logger.LogError(task.Exception, "Failed to renew resource");
						}
						else
						{
							_logger.LogInformation("Renew task cancelled");
							return;
						}
					}

					if (!_cancellation.Token.IsCancellationRequested)
					{
						await Task.WhenAny(
							_manualRenewEvent.WaitOneAsync(_cancellation.Token),
							Task.Delay(Interval, _cancellation.Token)
							).ConfigureAwait(false);
					}
					else
					{
						_logger.LogInformation("Renew task cancelled");
						return;
					}

					if (!_cancellation.Token.IsCancellationRequested)
					{
						RenewInternal();
					}
					else
					{
						_logger.LogInformation("Renew task cancelled");
					}
				},
				_cancellation.Token
				)
			.ContinueWith(
				task =>
				{
					_logger.LogInformation("Renew task cancelled");
				},
				TaskContinuationOptions.OnlyOnCanceled
				);
	}

	/// <summary>
	/// Current value (will not wait for initial task completion, use <see cref="GetCurrentValue"/>)
	/// </summary>
	public T Value
	{
		get
		{
			if (_renewTask.Status is TaskStatus.RanToCompletion or TaskStatus.WaitingForChildrenToComplete)
			{
				return _renewTask.Result;
			}

			return default;
		}
	}

	/// <summary>
	/// Get Current value (will block current thread to wait for a task to complete)
	/// </summary>
	/// <returns>Current value</returns>
	public T GetCurrentValue() => _renewTask.GetAwaiter().GetResult();


	/// <summary>
	/// Current task
	/// </summary>
	public Task<T> AsyncValue => _renewTask;

	/// <summary>
	/// IDisposable implementation
	/// </summary>
	public void Dispose()
	{
		_cancellation.Cancel();
		_renewTask?.Dispose();
	}
}
