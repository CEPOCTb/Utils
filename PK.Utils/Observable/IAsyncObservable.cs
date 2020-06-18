namespace PK.Utils.Observable
{
	/// <summary>
	/// Async observable
	/// </summary>
	/// <typeparam name="T">Observable event type</typeparam>
    public interface IAsyncObservable<out T>
    {
	    /// <summary>
	    /// Register event observer
	    /// </summary>
	    /// <param name="observer">Observer</param>
        void Register(IAsyncObserver<T> observer);
	    
	    /// <summary>
	    /// Unregister event observer
	    /// </summary>
	    /// <param name="observer">Observer</param>
        void Unregister(IAsyncObserver<T> observer);
    }
}
