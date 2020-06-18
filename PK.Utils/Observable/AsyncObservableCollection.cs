using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace PK.Utils.Observable
{
	/// <summary>
	/// Helper class to easily manage observer registrations and notifications
	/// </summary>
	/// <typeparam name="T">Observable event type</typeparam>
    public class AsyncObservableCollection<T> : IAsyncObservable<T>
    {
        private ImmutableList<IAsyncObserver<T>> _observers = ImmutableList.Create<IAsyncObserver<T>>();

        /// <summary>
        /// Notify observers of event
        /// </summary>
        /// <param name="obj">Event</param>
        /// <param name="token">Cancellation token</param>
        /// <returns></returns>
        public async Task Notify(T obj, CancellationToken token = default)
        {
            var observers = _observers;

            foreach (var observer in _observers)
            {
                await observer.Update(obj, token).ConfigureAwait(false);
            }
        }
        
        #region Implementation of IAsyncObservable<out T>

        /// <inheritdoc />
        public void Register(IAsyncObserver<T> observer)
        {
            if (observer == null)
            {
                throw new ArgumentNullException(nameof(observer));
            }

            Interlocked.Exchange(ref _observers, _observers.Add(observer));
        }

        /// <inheritdoc />
        public void Unregister(IAsyncObserver<T> observer)
        {
            if (observer == null)
            {
                throw new ArgumentNullException(nameof(observer));
            }

            Interlocked.Exchange(ref _observers, _observers.Remove(observer));
        }

        #endregion
    }
}
