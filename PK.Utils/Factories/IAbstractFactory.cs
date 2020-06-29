using JetBrains.Annotations;

namespace PK.Utils.Factories
{
	/// <summary>
	/// Abstract factory
	/// </summary>
	/// <typeparam name="T">Value type</typeparam>
	[PublicAPI]
	public interface IAbstractFactory<out T>
	{
		/// <summary>
		/// Creates a value of type <typeparamref name="T"/>
		/// </summary>
		/// <returns>Created value</returns>
		T Create();
	}

	/// <summary>
	/// Abstract factory
	/// </summary>
	/// <typeparam name="T">Value type</typeparam>
	/// <typeparam name="TParam">Parameter type</typeparam>
	[PublicAPI]
	public interface IAbstractFactory<out T, in TParam>
	{
		/// <summary>
		/// Creates a value of type <typeparamref name="T"/>
		/// </summary>
		/// <param name="obj">Factory parameter</param>
		/// <returns>Created value</returns>
		T Create(TParam obj);
	}
}
