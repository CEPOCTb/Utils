using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace PK.Utils.Factories;

/// <summary>
/// Generic implementation fo <see cref="IAbstractFactory{T}"/>, that uses factory method
/// </summary>
/// <typeparam name="T">Value type</typeparam>
[PublicAPI]
public class GenericAsyncFactory<T> : IAsyncAbstractFactory<T>
{
	[NotNull] private readonly Func<ValueTask<T>> _factoryMethod;

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="factoryMethod">Factory method, that creates an object of type <typeparamref name="T"/></param>
	public GenericAsyncFactory([NotNull] Func<ValueTask<T>> factoryMethod) =>
		_factoryMethod = factoryMethod ?? throw new ArgumentNullException(nameof(factoryMethod));

	/// <inheritdoc />
	public ValueTask<T> CreateAsync() => _factoryMethod();
}

/// <summary>
/// Generic implementation fo <see cref="IAbstractFactory{T}"/>, that uses factory method
/// </summary>
/// <typeparam name="T">Value type</typeparam>
/// <typeparam name="TParam">Parameter type</typeparam>
[PublicAPI]
public class GenericAsyncFactory<T, TParam> : IAsyncAbstractFactory<T, TParam>
{
	[NotNull] private readonly Func<TParam, ValueTask<T>> _factoryMethod;

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="factoryMethod">Factory method, that creates an object of type <typeparamref name="T"/></param>
	public GenericAsyncFactory([NotNull] Func<TParam, ValueTask<T>> factoryMethod) => _factoryMethod =
		factoryMethod ?? throw new ArgumentNullException(nameof(factoryMethod));

	/// <inheritdoc />
	public ValueTask<T> CreateAsync(TParam param) => _factoryMethod(param);
}
