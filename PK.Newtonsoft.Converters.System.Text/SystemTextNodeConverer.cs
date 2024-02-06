#if NET6_0_OR_GREATER
using System;
using System.Text.Json.Nodes;
using Newtonsoft.Json;
using JsonSerializer=Newtonsoft.Json.JsonSerializer;

namespace PK.Newtonsoft.Converters.System.Text;

/// <summary>
/// Newtonsoft converter for converting to and from <see cref="JsonNode"/>
/// </summary>
public class SystemTextNodeConverter : JsonConverter
{
	/// <summary>
	/// Singleton instance
	/// </summary>
	public static readonly SystemTextNodeConverter Instance = new SystemTextNodeConverter();

	/// <inheritdoc />
	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		if (value == null)
		{
			writer.WriteNull();
			return;
		}

		if (value is JsonObject obj)
		{
			writer.WriteStartObject();

			foreach (var property in obj)
			{
				writer.WritePropertyName(property.Key);

				if (property.Value is JsonObject objVal)
				{
					WriteJson(writer, objVal, serializer);
					continue;
				}

				if (property.Value is JsonArray arrVal)
				{
					WriteJson(writer, arrVal, serializer);
					continue;
				}

				var val = property.Value.AsValue();
				if (val.TryGetValue(out DateTimeOffset dto))
				{
					writer.WriteValue(dto);
					continue;
				}

				if (val.TryGetValue(out DateTime dt))
				{
					writer.WriteValue(dt);
					continue;
				}

				if (val.TryGetValue(out TimeSpan ts))
				{
					writer.WriteValue(ts);
					continue;
				}

				if (val.TryGetValue(out int i))
				{
					writer.WriteValue(i);
					continue;
				}

				if (val.TryGetValue(out Int64 i6))
				{
					writer.WriteValue(i6);
					continue;
				}

				if (val.TryGetValue(out UInt64 ui6))
				{
					writer.WriteValue(ui6);
					continue;
				}

				if (val.TryGetValue(out double d))
				{
					writer.WriteValue(d);
					continue;
				}

				if (val.TryGetValue(out string s))
				{
					writer.WriteValue(s);
					continue;
				}

				if (val.TryGetValue(out byte[] b))
				{
					writer.WriteValue(b);
					continue;
				}
			}
			writer.WriteEndObject();
		}
		else if (value is JsonArray arr)
		{
			writer.WriteStartArray();

			foreach (var node in arr)
			{
				if (node is JsonObject objVal)
				{
					WriteJson(writer, objVal, serializer);

					continue;
				}

				if (node is JsonArray arrVal)
				{
					WriteJson(writer, arrVal, serializer);

					continue;
				}

				var val = node.AsValue();
				if (val.TryGetValue(out DateTimeOffset dto))
				{
					writer.WriteValue(dto);

					continue;
				}

				if (val.TryGetValue(out DateTime dt))
				{
					writer.WriteValue(dt);

					continue;
				}

				if (val.TryGetValue(out TimeSpan ts))
				{
					writer.WriteValue(ts);

					continue;
				}

				if (val.TryGetValue(out int i))
				{
					writer.WriteValue(i);

					continue;
				}

				if (val.TryGetValue(out Int64 i6))
				{
					writer.WriteValue(i6);

					continue;
				}

				if (val.TryGetValue(out UInt64 ui6))
				{
					writer.WriteValue(ui6);

					continue;
				}

				if (val.TryGetValue(out double d))
				{
					writer.WriteValue(d);

					continue;
				}

				if (val.TryGetValue(out string s))
				{
					writer.WriteValue(s);

					continue;
				}

				if (val.TryGetValue(out byte[] b))
				{
					writer.WriteValue(b);

					continue;
				}
			}

			writer.WriteEndArray();
		}
		else
		{
			var val = value as JsonValue;
			if (val.TryGetValue(out DateTimeOffset dto))
			{
				writer.WriteValue(dto);
				return;
			}

			if (val.TryGetValue(out DateTime dt))
			{
				writer.WriteValue(dt);
				return;
			}

			if (val.TryGetValue(out TimeSpan ts))
			{
				writer.WriteValue(ts);
				return;
			}

			if (val.TryGetValue(out int i))
			{
				writer.WriteValue(i);
				return;
			}

			if (val.TryGetValue(out Int64 i6))
			{
				writer.WriteValue(i6);
				return;
			}

			if (val.TryGetValue(out UInt64 ui6))
			{
				writer.WriteValue(ui6);
				return;
			}

			if (val.TryGetValue(out double d))
			{
				writer.WriteValue(d);
				return;
			}

			if (val.TryGetValue(out string s))
			{
				writer.WriteValue(s);
				return;
			}

			if (val.TryGetValue(out byte[] b))
			{
				writer.WriteValue(b);
				return;
			}
		}
	}

	/// <inheritdoc />
	public override object ReadJson(
		JsonReader reader,
		Type objectType,
		object existingValue,
		JsonSerializer serializer
		)
	{
		if (reader.TokenType == JsonToken.Null)
		{
			return null;
		}

		if (reader.TokenType == JsonToken.StartObject)
		{
			var obj = new JsonObject();

			while (reader.Read())
			{
				switch (reader.TokenType)
				{
					case JsonToken.EndObject:
						return obj;
					case JsonToken.PropertyName:
						var name = reader.Value!.ToString();
						reader.Read();
						var val = ReadValue(reader, serializer);
						obj.Add(name!, val);

						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			return obj;
		}

		if (reader.TokenType == JsonToken.StartArray)
		{
			var obj = new JsonArray();

			while (reader.Read())
			{
				switch (reader.TokenType)
				{
					case JsonToken.EndArray:
						return obj;
					default:
						var val = ReadValue(reader, serializer);
						obj.Add(val);
						break;
				}
			}

			return obj;
		}

		throw new ArgumentOutOfRangeException();
	}

	private JsonNode ReadValue(JsonReader reader, JsonSerializer serializer)
	{
		switch (reader.TokenType)
		{
			case JsonToken.StartObject:
				return (JsonObject) ReadJson(reader, typeof(JsonObject), null, serializer);
			case JsonToken.StartArray:
				return (JsonArray)ReadJson(reader, typeof(JsonArray), null, serializer);
			case JsonToken.String:
				return (string)reader.Value;
			case JsonToken.Integer:
				return (Int64?)reader.Value;
			case JsonToken.Boolean:
				return (bool?)reader.Value;
			case JsonToken.Date:
				return reader.Value is DateTimeOffset offset ? offset : (DateTime?)reader.Value;
			case JsonToken.Float:
				return (double?)reader.Value;
			case JsonToken.Bytes:
				return (byte[])reader.Value is {} s ? Convert.ToBase64String(s) : null;
			case JsonToken.Null:
				return null;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	/// <inheritdoc />
	public override bool CanConvert(Type objectType) =>
		typeof(JsonNode).IsAssignableFrom(objectType);
}
#endif
