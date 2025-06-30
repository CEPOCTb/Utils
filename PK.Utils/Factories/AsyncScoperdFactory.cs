using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace PK.Utils.Factories;

/// <summary>
/// Helpers class for creating scoped factories
/// </summary>
[PublicAPI]
public static class AsyncScopedFactory
{
	/// <summary>
	/// Helper for creation of scoped factory
	/// </summary>
	/// <param name="scopeFactory">Scope factory method</param>
	/// <param name="factory">Value factory method</param>
	/// <typeparam name="T">Value type</typeparam>
	/// <typeparam name="TScope">Scope type</typeparam>
	/// <returns>Factory</returns>
	public static IAsyncScopedAbstractFactory<T> CreateFactory<T, TScope>(
		[NotNull] Func<ValueTask<TScope>> scopeFactory,
		[NotNull] Func<TScope, ValueTask<T>> factory
		)
		where TScope : class, IDisposable
		=> new AsyncScopedFactory<T, TScope>(scopeFactory, factory);


	/// <summary>
	/// Helper for creation of scoped factory
	/// </summary>
	/// <param name="scopeFactory">Scope factory method</param>
	/// <param name="factory">Value factory method</param>
	/// <typeparam name="T">Value type</typeparam>
	/// <typeparam name="TScope">Scope type</typeparam>
	/// <returns>Factory</returns>
	public static IAsyncScopedAbstractFactory<T> CreateFactory<T, TScope>(
		[NotNull] Func<ValueTask<TScope>> scopeFactory,
		[NotNull] Func<TScope, T> factory
		)
		where TScope : class, IDisposable
		=> new AsyncScopedFactory<T, TScope>(scopeFactory, factory);

	/// <summary>
	/// Helper for creation of scoped factory
	/// </summary>
	/// <param name="scopeFactory">Scope factory method</param>
	/// <param name="factory">Value factory method</param>
	/// <typeparam name="T">Value type</typeparam>
	/// <typeparam name="TParam">Parameter type</typeparam>
	/// <typeparam name="TScope">Scope type</typeparam>
	/// <returns>Factory</returns>
	public static IAsyncScopedAbstractFactory<T, TParam> CreateFactory<T, TParam, TScope>(
		[NotNull] Func<ValueTask<TScope>> scopeFactory,
		[NotNull] Func<TScope, TParam, ValueTask<T>> factory
		)
		where TScope : class, IDisposable
		=> new AsyncScopedFactory<T, TParam, TScope>(scopeFactory, factory);

	/// <summary>
	/// Helper for creation of scoped factory
	/// </summary>
	/// <param name="scopeFactory">Scope factory method</param>
	/// <param name="factory">Value factory method</param>
	/// <typeparam name="T">Value type</typeparam>
	/// <typeparam name="TParam">Parameter type</typeparam>
	/// <typeparam name="TScope">Scope type</typeparam>
	/// <returns>Factory</returns>
	public static IAsyncScopedAbstractFactory<T, TParam> CreateFactory<T, TParam, TScope>(
		[NotNull] Func<ValueTask<TScope>> scopeFactory,
		[NotNull] Func<TScope, TParam, T> factory
		)
		where TScope : class, IDisposable
		=> new AsyncScopedFactory<T, TParam, TScope>(scopeFactory, factory);
}

/// <summary>
/// Factory for creation of scoped value
/// </summary>
/// <typeparam name="T">Value type</typeparam>
/// <typeparam name="TScope">Scope type</typeparam>
[PublicAPI]
public class AsyncScopedFactory<T, TScope> : IAsyncScopedAbstractFactory<T> where TScope : class, IDisposable
{
	private readonly Func<TScope, ValueTask<T>> _asyncFactory;
	private readonly Func<TScope, T> _factory;
	[NotNull] private readonly Func<ValueTask<TScope>> _scopeFactory;

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="scopeFactory">Scope factory method</param>
	/// <param name="factory">Value factory method</param>
	public AsyncScopedFactory([NotNull] Func<ValueTask<TScope>> scopeFactory, [NotNull] Func<TScope, T> factory)
	{
		_scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
		_factory = factory ?? throw new ArgumentNullException(nameof(factory));
	}

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="scopeFactory">Scope factory method</param>
	/// <param name="factory">Value factory method</param>
	public AsyncScopedFactory([NotNull] Func<ValueTask<TScope>> scopeFactory, [NotNull] Func<TScope, ValueTask<T>> factory)
	{
		_scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
		_asyncFactory = factory ?? throw new ArgumentNullException(nameof(factory));
	}

	/// <inheritdoc />
	public async ValueTask<IScopedValue<T>> CreateAsync()
	{
		var scope = await _scopeFactory().ConfigureAwait(false) ?? throw new InvalidOperationException("scopeFactory() returned null scope");
		var value = _asyncFactory != null
			? await _asyncFactory(scope).ConfigureAwait(false)
			: _factory(scope);

		return new AsyncScopedValue<T>(value, scope);
	}
}

/// <summary>
/// Factory for creation of scoped value
/// </summary>
/// <typeparam name="T">Value type</typeparam>
/// <typeparam name="TParam">Parameter type</typeparam>
/// <typeparam name="TScope">Scope type</typeparam>
[PublicAPI]
public class AsyncScopedFactory<T, TParam, TScope> : IAsyncScopedAbstractFactory<T, TParam> where TScope : class, IDisposable
{
	private readonly Func<TScope, TParam, ValueTask<T>> _asyncFactory;
	private readonly Func<TScope, TParam, T> _factory;
	[NotNull] private readonly Func<ValueTask<TScope>> _scopeFactory;

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="scopeFactory">Scope factory method</param>
	/// <param name="factory">Value factory method</param>
	public AsyncScopedFactory([NotNull] Func<ValueTask<TScope>> scopeFactory, [NotNull] Func<TScope, TParam, T> factory)
	{
		_scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
		_factory = factory ?? throw new ArgumentNullException(nameof(factory));
	}

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="scopeFactory">Scope factory method</param>
	/// <param name="factory">Value factory method</param>
	public AsyncScopedFactory([NotNull] Func<ValueTask<TScope>> scopeFactory, [NotNull] Func<TScope, TParam, ValueTask<T>> factory)
	{
		_scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
		_asyncFactory = factory ?? throw new ArgumentNullException(nameof(factory));
	}

	/// <inheritdoc />
	public async ValueTask<IScopedValue<T>> CreateAsync(TParam param)
	{
		var scope = await _scopeFactory().ConfigureAwait(false) ?? throw new InvalidOperationException("scopeFactory() returned null scope");
		var value = _asyncFactory != null
			? await _asyncFactory(scope, param).ConfigureAwait(false)
			: _factory(scope, param);
		return new ScopedValue<T>(value, scope);
	}
}
