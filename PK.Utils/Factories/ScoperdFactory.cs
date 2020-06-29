using System;
using JetBrains.Annotations;

namespace PK.Utils.Factories
{
	[PublicAPI]
	public static class ScopedFactory
	{
		/// <summary>
		/// Helper for creation of scoped factory 
		/// </summary>
		/// <param name="scopeFactory">Scope factory method</param>
		/// <param name="factory">Value factory method</param>
		/// <typeparam name="T">Value type</typeparam>
		/// <typeparam name="TScope">Scope type</typeparam>
		/// <returns>Factory</returns>
		public static ScopedFactory<T, TScope> CreateFactory<T, TScope>(
			[NotNull] Func<TScope> scopeFactory,
			[NotNull] Func<TScope, T> factory)
			where TScope : class, IDisposable
			=> new ScopedFactory<T, TScope>(scopeFactory, factory);

		/// <summary>
		/// Helper for creation of scoped factory 
		/// </summary>
		/// <param name="scopeFactory">Scope factory method</param>
		/// <param name="factory">Value factory method</param>
		/// <typeparam name="T">Value type</typeparam>
		/// <typeparam name="TParam">Parameter type</typeparam>
		/// <typeparam name="TScope">Scope type</typeparam>
		/// <returns>Factory</returns>
		public static ScopedFactory<T, TParam, TScope> CreateFactory<T, TParam, TScope>(
			[NotNull] Func<TScope> scopeFactory,
			[NotNull] Func<TScope, TParam, T> factory)
			where TScope : class, IDisposable
			=> new ScopedFactory<T, TParam, TScope>(scopeFactory, factory);
		
	}
	
	/// <summary>
	/// Factory for creation of scoped value
	/// </summary>
	/// <typeparam name="T">Value type</typeparam>
	/// <typeparam name="TScope">Scope type</typeparam>
	[PublicAPI]
	public class ScopedFactory<T, TScope> : IScopedAbstractFactory<T> where TScope : class, IDisposable
	{
		[NotNull] private readonly Func<TScope, T> _factory;
		[NotNull] private readonly Func<TScope> _scopeFactory;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="scopeFactory">Scope factory method</param>
		/// <param name="factory">Value factory method</param>
		public ScopedFactory([NotNull] Func<TScope> scopeFactory, [NotNull] Func<TScope, T> factory)
		{
			_scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
			_factory = factory ?? throw new ArgumentNullException(nameof(factory));
		}

		#region Implementation of IAbstractFactory<ScopedValue<T>>

		/// <inheritdoc />
		public IScopedValue<T> Create()
		{
			var scope = _scopeFactory() ?? throw new InvalidOperationException("scopeFactory() returned null scope");
			var value = _factory(scope);
			return new ScopedValue<T>(value, scope);
		}

		#endregion
	}

	/// <summary>
	/// Factory for creation of scoped value
	/// </summary>
	/// <typeparam name="T">Value type</typeparam>
	/// <typeparam name="TParam">Parameter type</typeparam>
	/// <typeparam name="TScope">Scope type</typeparam>
	[PublicAPI]
	public class ScopedFactory<T, TParam, TScope> : IScopedAbstractFactory<T, TParam> where TScope : class, IDisposable
	{
		[NotNull] private readonly Func<TScope, TParam, T> _factory;
		[NotNull] private readonly Func<TScope> _scopeFactory;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="scopeFactory">Scope factory method</param>
		/// <param name="factory">Value factory method</param>
		public ScopedFactory([NotNull] Func<TScope> scopeFactory, [NotNull] Func<TScope, TParam, T> factory)
		{
			_scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
			_factory = factory ?? throw new ArgumentNullException(nameof(factory));
		}

		#region Implementation of IAbstractFactory<ScopedValue<T>>

		/// <inheritdoc />
		public IScopedValue<T> Create(TParam param)
		{
			var scope = _scopeFactory() ?? throw new InvalidOperationException("scopeFactory() returned null scope");
			var value = _factory(scope, param);
			return new ScopedValue<T>(value, scope);
		}

		#endregion
	}

}
