using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;
using JetBrains.Annotations;

namespace PK.Utils
{
	[PublicAPI]
	public static class HttpUtils
	{
		/// <summary>
		/// Parse query string into dictionary of parameters
		/// </summary>
		/// <param name="query">Query string</param>
		/// <param name="encoding">Encoding (default UTF-8)</param>
		/// <returns>Parameters dictionary</returns>
		[NotNull]
		[PublicAPI]
		public static IDictionary<string, IList<string>> ParseQueryString([CanBeNull] string query, Encoding encoding = null)
		{
			var parseResult = new Dictionary<string, IList<string>>();

			var queryStripped = query?.TrimStart('?');

			if (string.IsNullOrWhiteSpace(queryStripped))
			{
				return parseResult;
			}

			var pairs = queryStripped.Split('&');

			var enc = encoding ?? Encoding.UTF8;

			foreach (var pair in pairs)
			{
				var index = pair.IndexOf('=');

				string key;
				string value = null;

				if (index >= 0)
				{
					key = pair.Substring(0, index);
					value = pair.Substring(index + 1);
				}
				else
				{
					key = pair;
				}

				if (!parseResult.TryGetValue(key, out var list))
				{
					list = new List<string>();
					parseResult.Add(key, list);
				}

				list.Add(value != null ? HttpUtility.UrlDecode(value, enc) : null);
			}

			return parseResult;
		}

		/// <summary>
		/// Formats query string from dictionary of parameters
		/// </summary>
		/// <param name="paramsList">Parameters dictionary</param>
		/// <param name="encoding">Encoding (default UTF-8)</param>
		/// <returns>Query string</returns>
		[NotNull]
		[PublicAPI]
		public static string FormatQueryString(IDictionary<string, IList<string>> paramsList, Encoding encoding = null)
		{
			var enc = encoding ?? Encoding.UTF8;

			if (paramsList == null)
			{
				return string.Empty;
			}

			var builder = new StringBuilder();
			bool first = true;

			foreach (var pair in paramsList)
			{
				if (pair.Value == null)
				{
					if (first)
					{
						first = false;
					}
					else
					{
						builder.Append('&');
					}

					builder.Append(HttpUtility.UrlEncode(pair.Key, enc));
				}
				else
				{
					foreach (var value in pair.Value)
					{
						if (first)
						{
							first = false;
						}
						else
						{
							builder.Append('&');
						}

						builder.Append(HttpUtility.UrlEncode(pair.Key, enc));
						if (pair.Value != null)
						{
							builder.Append('=');
							builder.Append(HttpUtility.UrlEncode(value, enc));
						}
					}
				}
			}

			return builder.ToString();
		}

		/// <summary>
		/// Checks if <see cref="HttpStatusCode"/> indicate success
		/// </summary>
		/// <param name="statusCode">Status code</param>
		/// <returns>True if success status code</returns>
		[Pure]
		[PublicAPI]
		public static bool IsSuccessStatusCode(this HttpStatusCode statusCode)
		{
			return ((int) statusCode >= 200) && ((int) statusCode <= 299);
		}

		/// <summary>
		/// Parse UriBuilder's query into dictionary of parameters
		/// </summary>
		/// <param name="builder">Uri builder instance</param>
		/// <param name="encoding">Encoding (default UTF-8)</param>
		/// <returns>Parameters dictionary</returns>
		[NotNull]
		[PublicAPI]
		public static IDictionary<string, IList<string>> ParseQueryString(this UriBuilder builder, Encoding encoding = null)
		{
			return ParseQueryString(builder.Query, encoding);
		}

		/// <summary>
		/// Formats UriBuilder's query from dictionary of parameters
		/// </summary>
		/// <param name="builder">Uri builder instance</param>
		/// <param name="paramList">Parameters dictionary</param>
		/// <param name="encoding">Encoding (default UTF-8)</param>
		/// <returns></returns>
		[NotNull]
		[PublicAPI]
		public static string SetQueryParams([NotNull] this UriBuilder builder,
			[NotNull] IDictionary<string, IList<string>> paramList,
			Encoding encoding = null)
		{
			builder.Query = FormatQueryString(paramList, encoding);
			return builder.Query;
		}

		/// <summary>
		/// Searches query params for placeholders and replaces them with provided values 
		/// </summary>
		/// <param name="paramList">Parameters dictionary</param>
		/// <param name="placeholders">Search-replace values</param>
		/// <returns>New parameters dictionary</returns>
		[PublicAPI]
		[NotNull]
		public static IDictionary<string, IList<string>> ReplaceParamsPlaceholders(
			[NotNull] this IDictionary<string, IList<string>> paramList,
			[NotNull] params (string Search, string Replace)[] placeholders
			)
		{
			var newList = new Dictionary<string, IList<string>>();
			foreach (var pair in paramList)
			{
				if (pair.Value != null)
				{
					for (int i = 0; i < pair.Value.Count; i++)
					{
						foreach (var placeholder in placeholders)
						{
							pair.Value[i] = pair.Value[i].Replace(placeholder.Search, placeholder.Replace);
						}
					}
				}

				var key = pair.Key;
				foreach (var placeholder in placeholders)
				{
					key = key.Replace(placeholder.Search, placeholder.Replace);
				}

				if (newList.TryGetValue(key, out var existing))
				{
					if (pair.Value != null)
					{
						foreach (var val in pair.Value)
						{
							existing.Add(val);
						}
					}
				}
				else
				{
					newList.Add(key, pair.Value);
				}
			}

			return newList;
		}

		/// <summary>
		/// Searches UriBuilder's query params for placeholders and replaces them with provided values 
		/// </summary>
		/// <param name="builder">Uri builder instance</param>
		/// <param name="encoding">Encoding (default UTF-8)</param>
		/// <param name="placeholders">Search-replace values</param>
		[PublicAPI]
		public static void ReplaceParamsPlaceholders(
			[NotNull] this UriBuilder builder,
			Encoding encoding = null,
			[NotNull] params (string Search, string Replace)[] placeholders
			)
		{
			var paramsCollection = builder
				.ParseQueryString(encoding)
				.ReplaceParamsPlaceholders(placeholders);

			builder.SetQueryParams(paramsCollection, encoding);
		}
	}
}
