using System.Threading;
using System.Threading.Tasks;

namespace PK.Utils.Observable;

/// <summary>
/// Async observer
/// </summary>
/// <typeparam name="T">Observable event type</typeparam>
public interface IAsyncObserver<in T>
{
	/// <summary>
	/// Called by IAsyncObservable to notify of event
	/// </summary>
	/// <param name="obj">Event</param>
	/// <param name="token">Cancellation token</param>
	/// <returns></returns>
	Task Update(T obj, CancellationToken token = default);
}