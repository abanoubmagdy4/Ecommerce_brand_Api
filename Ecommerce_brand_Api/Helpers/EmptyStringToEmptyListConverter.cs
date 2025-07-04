using System.Text.Json.Serialization;

namespace Ecommerce_brand_Api.Helpers
{
    public class EmptyStringToEmptyListConverter<T> : JsonConverter<List<T>>
    {
        public override List<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String && reader.GetString() == "")
                return new List<T>();

            return JsonSerializer.Deserialize<List<T>>(ref reader, options);
        }

        public override void Write(Utf8JsonWriter writer, List<T> value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }

}
