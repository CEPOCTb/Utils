using System;
using JetBrains.Annotations;

namespace PK.Utils.Factories
{
	/// <summary>
	/// Implementation of <see cref="IDisposable"/> value wrapper
	/// </summary>
	/// <typeparam name="T">Value type</typeparam>
	[PublicAPI]
	public class ScopedValue<T> : IScopedValue<T>
	{
		private readonly Action<T> _disposeAction;
		private readonly IDisposable _disposable;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="value">Value</param>
		/// <param name="disposeAction">Action to perform on dispose</param>
		public ScopedValue(T value, Action<T> disposeAction)
		{
			Value = value;
			_disposeAction = disposeAction;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="value">Value</param>
		/// <param name="scope">Disposable scope</param>
		public ScopedValue(T value, IDisposable scope)
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
		public void Dispose() => _disposeAction?.Invoke(Value);

		#endregion
	}
}