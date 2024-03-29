using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace PK.Utils;

/// <summary>
/// Periodic action wrapper
/// </summary>
[PublicAPI]
public class PeriodicAction : IDisposable
{
	[NotNull] private readonly Func<CancellationToken, Task> _action;
	private readonly TimeSpan _interval;
	[NotNull] private readonly ILogger<PeriodicAction> _logger;
	private volatile Task _task;
	[NotNull] private readonly CancellationTokenSource _cancellation = new CancellationTokenSource();

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="action">Async action</param>
	/// <param name="interval">Repeat interval</param>
	/// <param name="logger">Logger instance</param>
	public PeriodicAction(
		[NotNull] Func<CancellationToken, Task> action,
		TimeSpan interval,
		[NotNull] ILogger<PeriodicAction> logger
		)
	{
		_action = action ?? throw new ArgumentNullException(nameof(action));
		_interval = interval;
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		Start();
	}

	private void Start()
	{
		_task = _action(_cancellation.Token);
		_task.ContinueWith(
			async task =>
			{
				_logger.LogError(task.Exception, "Start failed");
				await Task.Delay(TimeSpan.FromMinutes(1), _cancellation.Token).ConfigureAwait(false);
				if (!_cancellation.Token.IsCancellationRequested)
				{
					Start();
				}
			},
			_cancellation.Token,
			TaskContinuationOptions.OnlyOnFaulted,
			TaskScheduler.Current
			);

		_task.ContinueWith(
			async task =>
			{
				await Task.Delay(_interval, _cancellation.Token).ConfigureAwait(false);
				if (!_cancellation.Token.IsCancellationRequested)
				{
					Renew();
				}
			},
			_cancellation.Token,
			TaskContinuationOptions.OnlyOnRanToCompletion,
			TaskScheduler.Current
			);
	}

	private void Renew()
	{
		var renewTask = _action(_cancellation.Token);
		renewTask.ContinueWith(
			task => { _task = task; },
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
							_logger.LogError(task.Exception, "Iteration failed");
						}
						else
						{
							_logger.LogInformation("Iteration task cancelled");
							return;
						}
					}

					if (!_cancellation.Token.IsCancellationRequested)
					{
						await Task.Delay(_interval, _cancellation.Token).ConfigureAwait(false);
					}
					else
					{
						_logger.LogInformation("Iteration task cancelled");
						return;
					}

					if (!_cancellation.Token.IsCancellationRequested)
					{
						Renew();
					}
					else
					{
						_logger.LogInformation("Iteration task cancelled");
					}
				},
				_cancellation.Token
				)
			.ContinueWith(
				task =>
				{
					_logger.LogInformation("Iteration task cancelled");
				},
				TaskContinuationOptions.OnlyOnCanceled
				);
	}

	/// <summary>
	/// IDisposable implementation
	/// </summary>
	public void Dispose()
	{
		_cancellation.Cancel();
		_task?.Dispose();
	}
}