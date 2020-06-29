using JetBrains.Annotations;

namespace PK.Utils.Factories
{
	/// <summary>
	/// Abstract scoped factory - creates value of type <typeparamref name="T"/> wrapped into <see cref="IScopedValue{T}"/>,
	/// which implements <see cref="System.IDisposable"/> interface.
	/// </summary>
	/// <typeparam name="T">Value type</typeparam>
	[PublicAPI]
	public interface IScopedAbstractFactory<out T> : IAbstractFactory<IScopedValue<T>>
	{
	}

	/// <summary>
	/// Abstract scoped factory - creates value of type <typeparamref name="T"/> wrapped into <see cref="IScopedValue{T}"/>,
	/// which implements <see cref="System.IDisposable"/> interface.
	/// </summary>
	/// <typeparam name="T">Value type</typeparam>
	/// <typeparam name="TParam">Parameter type</typeparam>
	[PublicAPI]
	public interface IScopedAbstractFactory<out T, in TParam> : IAbstractFactory<IScopedValue<T>, TParam>
	{
	}
}
