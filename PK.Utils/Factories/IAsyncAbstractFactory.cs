using System.Threading.Tasks;
using JetBrains.Annotations;

namespace PK.Utils.Factories;

/// <summary>
/// Abstract factory
/// </summary>
/// <typeparam name="T">Value type</typeparam>
[PublicAPI]
public interface IAsyncAbstractFactory<T>
{
	/// <summary>
	/// Creates a value of type <typeparamref name="T"/>
	/// </summary>
	/// <returns>Created value</returns>
	ValueTask<T> CreateAsync();
}

/// <summary>
/// Abstract factory
/// </summary>
/// <typeparam name="T">Value type</typeparam>
/// <typeparam name="TParam">Parameter type</typeparam>
[PublicAPI]
public interface IAsyncAbstractFactory<T, in TParam>
{
	/// <summary>
	/// Creates a value of type <typeparamref name="T"/>
	/// </summary>
	/// <param name="obj">Factory parameter</param>
	/// <returns>Created value</returns>
	ValueTask<T> CreateAsync(TParam obj);
}
