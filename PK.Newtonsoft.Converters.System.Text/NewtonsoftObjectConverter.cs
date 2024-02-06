using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;

namespace PK.Newtonsoft.Converters.System.Text;

/// <summary>
/// System.Text.Json converter for converting to and from <see cref="JObject"/>
/// </summary>
public class NewtonsoftObjectConverter : JsonConverter<JObject>
{
	/// <summary>
	/// Singleton instance
	/// </summary>
	public static readonly NewtonsoftObjectConverter Instance = new NewtonsoftObjectConverter();

	/// <inheritdoc />
	public override JObject Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
		NewtonsoftTokenConverter.Instance.Read(ref reader, typeToConvert, options) as JObject;

	/// <inheritdoc />
	public override void Write(Utf8JsonWriter writer, JObject value, JsonSerializerOptions options) =>
		NewtonsoftTokenConverter.Instance.Write(writer, value, options);
}
