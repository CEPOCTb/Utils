using System;
using JetBrains.Annotations;

namespace PK.Utils
{
	/// <summary>
	/// Code helpers
	/// </summary>
	[PublicAPI]
	public static class ExecutionHelpers
	{
		/// <summary>
		/// Try execute action, invoke optional callback on exception and optionally rethrow or mute exception (by default)
		/// </summary>
		/// <param name="action">Action to execute</param>
		/// <param name="onException">Func to execute on exception, returning bool, indicating to rethrow exception or not</param>
		public static void Try(this Action action, Func<Exception, bool> onException = null)
		{
			try
			{
				action();
			}
			catch (Exception e)
			{
				if (onException?.Invoke(e) == true)
				{
					throw;
				}
			}
		}
	}
}
