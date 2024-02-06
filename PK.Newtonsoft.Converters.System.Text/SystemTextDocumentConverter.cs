using System;
using System.IO;
using System.Text.Json;
using Newtonsoft.Json;
using JsonSerializer=Newtonsoft.Json.JsonSerializer;

namespace PK.Newtonsoft.Converters.System.Text;

/// <summary>
/// Newtonsoft converter for converting to and from <see cref="JsonDocument"/> and <see cref="JsonElement"/>>
/// </summary>
public class SystemTextDocumentConverter : JsonConverter
{
	/// <summary>
	/// Singleton instance
	/// </summary>
	public static readonly SystemTextDocumentConverter Instance = new SystemTextDocumentConverter();

	/// <inheritdoc />
	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		if (value == null)
		{
			writer.WriteNull();
			return;
		}

		void WriteElem(JsonElement elem)
		{
			switch (elem.ValueKind)
			{
				case JsonValueKind.Null:
				{
					writer.WriteNull();
					return;
				}
				case JsonValueKind.True:
				case JsonValueKind.False:
				{
					writer.WriteValue(elem.GetBoolean());
					return;
				}
				case JsonValueKind.String:
				{
					writer.WriteValue(elem.GetString());
					return;
				}
				case JsonValueKind.Number:
				{
					writer.WriteValue(elem.TryGetInt64(out var i)
						? i
						: elem.TryGetUInt64(out var ui)
							? ui
							: elem.GetDouble());
					return;
				}
				case JsonValueKind.Object:
				{
					writer.WriteStartObject();

					foreach (var el in elem.EnumerateObject())
					{
						writer.WritePropertyName(el.Name);
						WriteElem(el.Value);
					}

					writer.WriteEndObject();
					return;
				}
				case JsonValueKind.Array:
				{
					writer.WriteStartArray();

					foreach (var el in elem.EnumerateArray())
					{
						WriteElem(el);
					}

					writer.WriteEndArray();
					return;
				}
				default:
				{
					writer.WriteUndefined();
					return;
				}
			}
		}

		WriteElem(value is JsonDocument doc ? doc.RootElement : (JsonElement)value);
	}

	/// <inheritdoc />
	public override object ReadJson(
		JsonReader reader,
		Type objectType,
		object existingValue,
		JsonSerializer serializer
		)
	{
		using var stream = new MemoryStream();
		using var writer = new Utf8JsonWriter(stream);

		void WriteTokens()
		{
			var depth = reader.Depth;

			do
			{
				switch (reader.TokenType)
				{
					case JsonToken.Null:
					{
						writer.WriteNullValue();
						break;
					}
					case JsonToken.Boolean:
					{
						writer.WriteBooleanValue((bool)reader.Value);
						break;
					}
					case JsonToken.Bytes:
					{
						writer.WriteBase64StringValue((byte[])reader.Value);
						break;
					}
					case JsonToken.Comment:
					{
						writer.WriteCommentValue((string)reader.Value);
						break;
					}
					case JsonToken.Date:
					{
						switch (reader.ValueType)
						{
							case {} t when t == typeof(DateTimeOffset):
								writer.WriteStringValue((DateTimeOffset)reader.Value);
								break;
							case {} t when t == typeof(DateTime):
								writer.WriteStringValue((DateTime)reader.Value);
								break;
						}
						break;
					}
					case JsonToken.Float:
					{
						writer.WriteNumberValue((double)reader.Value);
						break;
					}
					case JsonToken.Integer:
					{
						writer.WriteNumberValue((int)reader.Value);
						break;
					}
					case JsonToken.String:
					{
						writer.WriteStringValue((string)reader.Value);
						break;
					}
					case JsonToken.PropertyName:
					{
						writer.WritePropertyName((string)reader.Value);
						break;
					}
					case JsonToken.StartObject:
					{
						writer.WriteStartObject();
						break;
					}
					case JsonToken.EndObject:
					{
						writer.WriteEndObject();
						if (depth == reader.Depth)
						{
							writer.Flush();
							return;
						}
						break;
					}
					case JsonToken.StartArray:
					{
						writer.WriteStartArray();
						break;
					}
					case JsonToken.EndArray:
					{
						writer.WriteEndArray();
						break;
					}
					default:
						throw new Exception($"Unknown token type {reader.TokenType}, path: {reader.Path}");
				}

			} while (reader.Read());

			writer.Flush();
		}

		WriteTokens();

		var uReader = new Utf8JsonReader(stream.GetBuffer().AsSpan(0, (int)stream.Length));
		var doc = JsonDocument.ParseValue(ref uReader);

		return objectType == typeof(JsonDocument) ? doc : doc.RootElement;
	}

	/// <inheritdoc />
	public override bool CanConvert(Type objectType) =>
		objectType == typeof(JsonElement) || objectType == typeof(JsonDocument);
}
