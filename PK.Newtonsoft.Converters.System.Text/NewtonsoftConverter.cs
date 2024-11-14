using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;

namespace PK.Newtonsoft.Converters.System.Text;

/// <summary>
/// Factory converter for converting between Newtonsoft.Json LINQ to JSON objects (JObject, JArray, JValue)
/// and System.Text.Json elements.
/// </summary>
public class NewtonsoftConverter : JsonConverterFactory
{
	/// <inheritdoc />
	public override bool CanConvert(Type typeToConvert)
	{
		return typeof(JObject).IsAssignableFrom(typeToConvert) || typeof(JArray).IsAssignableFrom(typeToConvert) || typeof(JValue).IsAssignableFrom(typeToConvert);
	}

	/// <inheritdoc />
	public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
	{
		if (typeof(JObject).IsAssignableFrom(typeToConvert))
		{
			return NewtonsoftObjectConverter.Instance;
		}
		if (typeof(JArray).IsAssignableFrom(typeToConvert))
		{
			return NewtonsoftArrayConverter.Instance;
		}
		if (typeof(JValue).IsAssignableFrom(typeToConvert))
		{
			return NewtonsoftValueConverter.Instance;
		}
		throw new ArgumentException($"Type {typeToConvert} is not supported");
	}
}
