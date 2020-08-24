using System.Linq;
using System.Security.Cryptography;
using System.Text;
using JetBrains.Annotations;

namespace PK.Utils
{
	[PublicAPI]
	public static class CryptoHelpers
	{
		/// <summary>
		/// Returns MD5 hash of string
		/// </summary>
		/// <param name="input">Input string</param>
		/// <param name="encoding">Encoding (default ASCII)</param>
		/// <param name="format">Byte to string conversion format (<see cref="byte.ToString(string)"/>)></param>
		/// <returns>MD5 hash of string</returns>
		public static string Md5Hash(this string input, Encoding encoding = null, string format = "X2")
		{
			if (input == null)
			{
				return null;
			}

			encoding ??= Encoding.ASCII;
			using var md5 = MD5.Create();
			return string.Concat(
				md5.ComputeHash(encoding.GetBytes(input))
					.Select(b => b.ToString(format))
				);
		}

		/// <summary>
		/// Returns SHA-256 hash of string
		/// </summary>
		/// <param name="input">Input string</param>
		/// <param name="encoding">Encoding (default ASCII)</param>
		/// <param name="format">Byte to string conversion format (<see cref="byte.ToString(string)"/>)></param>
		/// <returns>SHA-256 hash of string</returns>
		public static string Sha256Hash(this string input, Encoding encoding = null, string format = "X2")
		{
			if (input == null)
			{
				return null;
			}

			encoding ??= Encoding.ASCII;
			using var md5 = SHA256.Create();
			return string.Concat(
				md5.ComputeHash(encoding.GetBytes(input))
					.Select(b => b.ToString(format))
				);
		}

		/// <summary>
		/// String comparison method with constant execution time
		/// </summary>
		/// <param name="input">Input string</param>
		/// <param name="target">Target string</param>
		/// <returns>Weather strings equals or not</returns>
		public static bool CryptoEquals(this string input, string target)
		{
			int len = input?.Length ?? 20;
			int targetLen = target?.Length ?? 0;
			int diff = input?.Length ?? 0 ^ targetLen;

			var fakeInput = new string('\0', len);
			var fake = new string('\0', len);

			var inputString = input ?? fakeInput;

			for (int i = 0; i < len; i++)
			{
				diff |= inputString[i] ^ (i < targetLen ? target[i] : fake[i]);
			}

			return diff == 0;
		}
	}
}
