using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PK.Utils.Linq;

public static class LinqExtensions
{
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

	public static async Task ForEachAsync<T>(this IEnumerable<T> enumerable, Func<T,Task> action, CancellationToken cancellationToken = default)
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
			await action(val).ConfigureAwait(false);
		}
	}

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
}
