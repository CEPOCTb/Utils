namespace PK.Utils.Helpers;

/// <summary>
/// Provides extension methods for common string operations.
/// </summary>
public static class StringHelpers
{
	/// <summary>
	/// Determines whether a specified string is null, empty, or consists only of white-space characters.
	/// </summary>
	/// <param name="value">The string to test.</param>
	/// <returns>True if the value parameter is null or empty, or if value consists exclusively of white-space characters.</returns>
	public static bool IsNullOrWhiteSpace(this string value) => string.IsNullOrWhiteSpace(value);

	/// <summary>
	/// Determines whether the specified string is null or an empty string.
	/// </summary>
	/// <param name="value">The string to check.</param>
	/// <returns>True if the string is null or empty; otherwise, false.</returns>
	public static bool IsNullOrEmpty(this string value) => string.IsNullOrEmpty(value);

	/// <summary>
	/// Checks if a string is null.
	/// </summary>
	/// <param name="value">The string to check.</param>
	/// <return>True if the string is null; otherwise, false.</return>
	public static bool IsNull(this string value) => value is null;
}
