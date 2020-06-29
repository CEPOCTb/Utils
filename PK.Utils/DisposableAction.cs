using System;
using JetBrains.Annotations;

namespace PK.Utils
{
	/// <summary>
	/// Disposable action - performs action, passed in constructor, on dispose
	/// </summary>
	[PublicAPI]
	public class DisposableAction : IDisposable
	{
		[NotNull] private readonly Action _action;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="action">Action to execute on dispose</param>
		public DisposableAction([NotNull] Action action) =>
			_action = action ?? throw new ArgumentNullException(nameof(action));

		/// <inheritdoc />
		public void Dispose() => _action.Invoke();
	}
}
