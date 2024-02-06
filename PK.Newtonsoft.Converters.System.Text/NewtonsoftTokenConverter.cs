using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;

namespace PK.Newtonsoft.Converters.System.Text;

internal class NewtonsoftTokenConverter : JsonConverter<JToken>
{
	public static readonly NewtonsoftTokenConverter Instance = new NewtonsoftTokenConverter();

	/// <inheritdoc />
	public override JToken Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType == JsonTokenType.Null)
		{
			return null;
		}

		if (reader.TokenType == JsonTokenType.StartObject)
		{
			return ReadObject(ref reader, typeToConvert, options);
		}

		if (reader.TokenType == JsonTokenType.StartArray)
		{
			return ReadArray(ref reader, typeToConvert, options);
		}

		return ReadValue(ref reader, options);
	}

	private JObject ReadObject(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType == JsonTokenType.Null)
		{
			return null;
		}

		var obj = new JObject();

		while (reader.Read())
		{
			switch (reader.TokenType)
			{
				case JsonTokenType.EndObject:
					return obj;
				case JsonTokenType.PropertyName:
					var name = reader.GetString();
					reader.Read();
					var val = ReadValue(ref reader, options);
					obj.Add(name, val);

					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		return obj;
	}

	private JArray ReadArray(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType == JsonTokenType.Null)
		{
			return null;
		}

		var obj = new JArray();

		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndArray)
			{
				return obj;
			}

			var val = ReadValue(ref reader, options);
			obj.Add(val);
		}

		return obj;
	}

	internal JToken ReadValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
	{
		switch (reader.TokenType)
		{
			case JsonTokenType.StartObject:
				return ReadObject(ref reader, typeof(JObject), options);
			case JsonTokenType.StartArray:
				return ReadArray(ref reader, typeof(JArray), options);
			case JsonTokenType.String:
				return new JValue(reader.GetString());
			case JsonTokenType.Number:
				return new JValue(reader.TryGetInt64(out var i) ? i : reader.GetDouble());
			case JsonTokenType.True:
				return new JValue(true);
			case JsonTokenType.False:
				return new JValue(false);
			case JsonTokenType.Null:
				return null;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	/// <inheritdoc />
	public override void Write(Utf8JsonWriter writer, JToken value, JsonSerializerOptions options)
	{
		if (value == null)
		{
			writer.WriteNullValue();
			return;
		}

		if (value is JObject obj)
		{
			writer.WriteStartObject();

			foreach (var property in obj.Properties())
			{
				writer.WritePropertyName(property.Name);

				switch (property.Value.Type)
				{
					case JTokenType.Guid:
						writer.WriteStringValue(property.Value.Value<Guid>());

						break;
					case JTokenType.Boolean:
						writer.WriteBooleanValue(property.Value.Value<bool>());

						break;
					case JTokenType.Integer:
						writer.WriteNumberValue(property.Value.Value<long>());

						break;
					case JTokenType.Float:
						writer.WriteNumberValue(property.Value.Value<double>());

						break;
					case JTokenType.String:
						writer.WriteStringValue(property.Value.Value<string>());

						break;
					case JTokenType.Null:
						writer.WriteNullValue();

						break;
					case JTokenType.Date:
						writer.WriteStringValue(property.Value.Value<DateTimeOffset>());

						break;
					case JTokenType.Bytes:
						writer.WriteBase64StringValue(property.Value.Value<byte[]>());

						break;
					case JTokenType.Uri:
						writer.WriteStringValue(property.Value.Value<Uri>().ToString());

						break;
					case JTokenType.TimeSpan:
						writer.WriteStringValue(property.Value.Value<TimeSpan>().ToString());

						break;
					case JTokenType.Object:
						Write(writer, property.Value.Value<JObject>(), options);

						break;
					case JTokenType.Array:
						Write(writer, property.Value.Value<JArray>(), options);

						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			writer.WriteEndObject();
		}
		else if (value is JArray array)
		{
			writer.WriteStartArray();

			foreach (var token in array)
			{
				switch (token.Type)
				{
					case JTokenType.Guid:
						writer.WriteStringValue(token.Value<Guid>());

						break;
					case JTokenType.Boolean:
						writer.WriteBooleanValue(token.Value<bool>());

						break;
					case JTokenType.Integer:
						writer.WriteNumberValue(token.Value<long>());

						break;
					case JTokenType.Float:
						writer.WriteNumberValue(token.Value<double>());

						break;
					case JTokenType.String:
						writer.WriteStringValue(token.Value<string>());

						break;
					case JTokenType.Null:
						writer.WriteNullValue();

						break;
					case JTokenType.Date:
						writer.WriteStringValue(token.Value<DateTimeOffset>());

						break;
					case JTokenType.Bytes:
						writer.WriteBase64StringValue(token.Value<byte[]>());

						break;
					case JTokenType.Uri:
						writer.WriteStringValue(token.Value<Uri>().ToString());

						break;
					case JTokenType.TimeSpan:
						writer.WriteStringValue(token.Value<TimeSpan>().ToString());

						break;
					case JTokenType.Object:
						Write(writer, token.Value<JObject>(), options);

						break;
					case JTokenType.Array:
						Write(writer, token.Value<JArray>(), options);

						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			writer.WriteEndArray();
		}
		else
		{
			switch (value.Type)
			{
				case JTokenType.Guid:
					writer.WriteStringValue(value.Value<Guid>());
					break;
				case JTokenType.Boolean:
					writer.WriteBooleanValue(value.Value<bool>());
					break;
				case JTokenType.Integer:
					writer.WriteNumberValue(value.Value<long>());
					break;
				case JTokenType.Float:
					writer.WriteNumberValue(value.Value<double>());
					break;
				case JTokenType.String:
					writer.WriteStringValue(value.Value<string>());
					break;
				case JTokenType.Null:
					writer.WriteNullValue();
					break;
				case JTokenType.Date:
					writer.WriteStringValue(value.Value<DateTimeOffset>());
					break;
				case JTokenType.Bytes:
					writer.WriteBase64StringValue(value.Value<byte[]>());
					break;
				case JTokenType.Uri:
					writer.WriteStringValue(value.Value<Uri>().ToString());
					break;
				case JTokenType.TimeSpan:
					writer.WriteStringValue(value.Value<TimeSpan>().ToString());
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

	}
}