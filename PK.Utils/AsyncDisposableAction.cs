using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace PK.Utils;

/// <summary>
/// Disposable action - performs action, passed in constructor, on dispose
/// </summary>
[PublicAPI]
public class AsyncDisposableAction : IAsyncDisposable
{
	[NotNull] private readonly Func<ValueTask> _action;

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="action">Action to execute on dispose</param>
	public AsyncDisposableAction([NotNull] Func<ValueTask> action) =>
		_action = action ?? throw new ArgumentNullException(nameof(action));

	/// <inheritdoc />
	public ValueTask DisposeAsync() => _action.Invoke();
}
