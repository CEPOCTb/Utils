using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PK.Utils.Linq;

public static class LinqExtensions
{
	/// <summary>
	/// Enumerates through <see cref="IEnumerable{T}"/> and executes <see cref="Action{T}"/> on each element
	/// </summary>
	/// <param name="enumerable">Enumerable</param>
	/// <param name="action">Action to execute</param>
	/// <typeparam name="T">Element type</typeparam>
	/// <exception cref="ArgumentNullException">When enumerable or action is null</exception>
	public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
	{
		if (enumerable == null)
		{
			throw new ArgumentNullException(nameof(enumerable));
		}

		if (action == null)
		{
			throw new ArgumentNullException(nameof(action));
		}

		foreach (var val in enumerable)
		{
			action(val);
		}
	}

	/// <summary>
	/// Asynchronously enumerates through <see cref="IEnumerable{T}"/> and executes <see cref="Action{T}"/> on each element
	/// </summary>
	/// <param name="enumerable">Enumerable</param>
	/// <param name="action">Action to execute</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <typeparam name="T">Element type</typeparam>
	/// <exception cref="ArgumentNullException">When enumerable or action is null</exception>
	/// <exception cref="T:System.OperationCanceledException">The token has had cancellation requested.</exception>
	/// <exception cref="T:System.ObjectDisposedException">The associated <see cref="T:System.Threading.CancellationTokenSource"></see> has been disposed.</exception>
	public static async Task ForEachAsync<T>(this IEnumerable<T> enumerable, Func<T, CancellationToken,Task> action, CancellationToken cancellationToken = default)
	{
		if (enumerable == null)
		{
			throw new ArgumentNullException(nameof(enumerable));
		}

		if (action == null)
		{
			throw new ArgumentNullException(nameof(action));
		}

		foreach (var val in enumerable)
		{
			cancellationToken.ThrowIfCancellationRequested();
			await action(val, cancellationToken).ConfigureAwait(false);
		}
	}

	/// <summary>
	/// Splits <see cref="IEnumerable{T}"/> into partitions of the given size
	/// </summary>
	/// <param name="enumerable">Source enumerable</param>
	/// <param name="size">Partition size</param>
	/// <typeparam name="T">Element type</typeparam>
	/// <returns>IEnumerable of partitions</returns>
	/// <exception cref="ArgumentNullException">Source enumerable is null</exception>
	/// <exception cref="ArgumentOutOfRangeException">Size is less or equal to zero</exception>
	public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> enumerable, int size)
	{
		if (enumerable == null)
		{
			throw new ArgumentNullException(nameof(enumerable));
		}

		if (size <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(size));
		}

		using var enumerator = enumerable.GetEnumerator();

		var expr = enumerator.MoveNext();
		while (expr)
		{
			var list = new List<T>(size);
			for (var i = 0; i < size; i++)
			{
				list.Add(enumerator.Current);
				expr = enumerator.MoveNext();
				if (!expr)
				{
					break;
				}
			}

			yield return list;
		}

	}

	/// <summary>
	/// Checks if <see cref="IEnumerable{T}"/> is null or empty
	/// </summary>
	/// <param name="enumerable">Enumerable</param>
	/// <typeparam name="T">Element type</typeparam>
	/// <returns>True if enumerable is null or empty</returns>
	public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
	{
		return enumerable == null || !enumerable.Any();
	}


}
