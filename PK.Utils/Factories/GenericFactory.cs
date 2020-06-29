using System;
using JetBrains.Annotations;

namespace PK.Utils.Factories
{
	/// <summary>
	/// Generic implementation fo <see cref="IAbstractFactory{T}"/>, that uses factory method
	/// </summary>
	/// <typeparam name="T">Value type</typeparam>
	[PublicAPI]
	public class GenericFactory<T> : IAbstractFactory<T>
	{
		[NotNull] private readonly Func<T> _factoryMethod;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="factoryMethod">Factory method, that creates an object of type <typeparamref name="T"/></param>
		public GenericFactory([NotNull] Func<T> factoryMethod) =>
			_factoryMethod = factoryMethod ?? throw new ArgumentNullException(nameof(factoryMethod));

		#region Implementation of IAbstractFactory<out T>

		/// <inheritdoc />
		public T Create() => _factoryMethod();

		#endregion
	}

	/// <summary>
	/// Generic implementation fo <see cref="IAbstractFactory{T}"/>, that uses factory method
	/// </summary>
	/// <typeparam name="T">Value type</typeparam>
	/// <typeparam name="TParam">Parameter type</typeparam>
	[PublicAPI]
	public class GenericFactory<T, TParam> : IAbstractFactory<T, TParam>
	{
		[NotNull] private readonly Func<TParam, T> _factoryMethod;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="factoryMethod">Factory method, that creates an object of type <typeparamref name="T"/></param>
		public GenericFactory([NotNull] Func<TParam, T> factoryMethod) => _factoryMethod =
			factoryMethod ?? throw new ArgumentNullException(nameof(factoryMethod));

		#region Implementation of IAbstractFactory<out T>

		/// <inheritdoc />
		public T Create(TParam param) => _factoryMethod(param);

		#endregion
	}
}
