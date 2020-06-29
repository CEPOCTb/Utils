using System;
using JetBrains.Annotations;

namespace PK.Utils.Factories
{
	/// <summary>
	/// Value wrapper, which implements <see cref="IDisposable"/> interface
	/// </summary>
	/// <typeparam name="T">Value type</typeparam>
	[PublicAPI]
	public interface IScopedValue<out T> : IDisposable
	{
		/// <summary>
		/// Wrapped value
		/// </summary>
		T Value { get; }
	}
}
