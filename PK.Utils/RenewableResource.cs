using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace PK.Utils
{
	[PublicAPI]
	public class RenewableResource<T> : IDisposable
	{
		[NotNull] private readonly Func<CancellationToken, T> _factory;
		private readonly TimeSpan _interval;
		[NotNull] private readonly ILogger<RenewableResource<T>> _logger;
		private volatile Task<T> _renewTask;
		[NotNull] private readonly CancellationTokenSource _cancellation = new CancellationTokenSource();

		public RenewableResource(
			[NotNull] Func<CancellationToken, T> factory,
			TimeSpan interval,
			[NotNull] ILogger<RenewableResource<T>> logger
			)
		{
			_factory = factory ?? throw new ArgumentNullException(nameof(factory));
			_interval = interval;
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			Start();
		}

		private void Start()
		{
			_renewTask = Task.Run(() => _factory(_cancellation.Token), _cancellation.Token);
			_renewTask.ContinueWith(
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

			_renewTask.ContinueWith(
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
			var renewTask = Task.Run(() => _factory(_cancellation.Token), _cancellation.Token);
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
							await Task.Delay(_interval, _cancellation.Token).ConfigureAwait(false);
						}
						else
						{
							_logger.LogInformation("Renew task cancelled");
							return;
						}

						if (!_cancellation.Token.IsCancellationRequested)
						{
							Renew();
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
					)
				;
		}

		public T Value
		{
			get
			{
				if (_renewTask.Status == TaskStatus.RanToCompletion
					|| _renewTask.Status == TaskStatus.WaitingForChildrenToComplete)
				{
					return _renewTask.Result;
				}

				return default;
			}
		}

		public void Dispose()
		{
			_cancellation.Cancel();
			_renewTask?.Dispose();
		}
	}
}
