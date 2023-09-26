using System;
using System.Threading;
using JetBrains.Annotations;

namespace PK.Utils;

/// <summary>
/// Static random helper class
/// </summary>
[PublicAPI]
public static class RandomHelper
{
	// Threadsafe random - initializes separate static instance for each thread
	private static readonly ThreadLocal<Random> _random =
		new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));

	/// <summary>
	/// Thread-safe <see cref="System.Random"/> instance
	/// </summary>
	public static Random Random => _random.Value;
}