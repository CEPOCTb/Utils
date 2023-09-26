using System;
using System.Threading;
using JetBrains.Annotations;

namespace PK.Utils.Synchronization;

/// <summary>
/// Extensions for <see cref="ReaderWriterLock"/> and <see cref="ReaderWriterLockSlim"/>
/// </summary>
[PublicAPI]
public static class ReadWriteLockExtensions
{
	/// <summary>
	/// Extension method that acquires reader lock and returns <see cref="IDisposable"/>,
	/// which releases lock on dispose
	/// </summary>
	/// <param name="objLock"><see cref="ReaderWriterLock"/> instance</param>
	/// <returns><see cref="IDisposable"/>, which releases lock on dispose</returns>
	public static IDisposable ReaderLock([NotNull] this ReaderWriterLock objLock)
	{
		if (objLock == null)
		{
			throw new ArgumentNullException(nameof(objLock));
		}

		objLock.AcquireReaderLock(Timeout.Infinite);
		return new DisposableAction(objLock.ReleaseReaderLock);
	}

	/// <summary>
	/// Extension method that acquires writer lock and returns <see cref="IDisposable"/>,
	/// which releases lock on dispose
	/// </summary>
	/// <param name="objLock"><see cref="ReaderWriterLock"/> instance</param>
	/// <returns><see cref="IDisposable"/>, which releases lock on dispose</returns>
	public static IDisposable WriterLock(this ReaderWriterLock objLock)
	{
		if (objLock == null)
		{
			throw new ArgumentNullException(nameof(objLock));
		}

		objLock.AcquireWriterLock(Timeout.Infinite);
		return new DisposableAction(objLock.ReleaseWriterLock);
	}

	/// <summary>
	/// Extension method that enters read lock and returns <see cref="IDisposable"/>,
	/// which exits lock on dispose
	/// </summary>
	/// <param name="objLock"><see cref="ReaderWriterLockSlim"/> instance</param>
	/// <returns><see cref="IDisposable"/>, which exits lock on dispose</returns>
	public static IDisposable ReaderLock(this ReaderWriterLockSlim objLock)
	{
		if (objLock == null)
		{
			throw new ArgumentNullException(nameof(objLock));
		}

		objLock.EnterReadLock();
		return new DisposableAction(objLock.ExitReadLock);
	}

	/// <summary>
	/// Extension method that enters write lock and returns <see cref="IDisposable"/>,
	/// which exits lock on dispose
	/// </summary>
	/// <param name="objLock"><see cref="ReaderWriterLockSlim"/> instance</param>
	/// <returns><see cref="IDisposable"/>, which exits lock on dispose</returns>
	public static IDisposable WriterLock(this ReaderWriterLockSlim objLock)
	{
		if (objLock == null)
		{
			throw new ArgumentNullException(nameof(objLock));
		}

		objLock.EnterWriteLock();
		return new DisposableAction(objLock.ExitWriteLock);
	}
}