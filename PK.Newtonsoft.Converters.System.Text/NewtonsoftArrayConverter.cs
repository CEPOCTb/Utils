using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;

namespace PK.Newtonsoft.Converters.System.Text;

/// <summary>
/// System.Text.Json converter for converting to and from <see cref="JArray"/>
/// </summary>
public class NewtonsoftArrayConverter : JsonConverter<JArray>
{
	/// <summary>
	/// Singleton instance
	/// </summary>
	public static readonly NewtonsoftArrayConverter Instance = new NewtonsoftArrayConverter();

	/// <inheritdoc />
	public override JArray Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
		NewtonsoftTokenConverter.Instance.Read(ref reader, typeToConvert, options) as JArray;

	/// <inheritdoc />
	public override void Write(Utf8JsonWriter writer, JArray value, JsonSerializerOptions options) =>
		NewtonsoftTokenConverter.Instance.Write(writer, value, options);
}
