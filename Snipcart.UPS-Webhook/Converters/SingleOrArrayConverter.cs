using System.Text.Json;
using System.Text.Json.Serialization;

namespace Snipcart.UPS_Webhook.Converters
{
    public class SingleOrArrayConverter<T> : JsonConverter<List<T>> where T : class
    {
        public override List<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var list = new List<T>();

            if (reader.TokenType == JsonTokenType.StartArray)
            {
                list = JsonSerializer.Deserialize<List<T>>(ref reader, options);
            }
            else if (reader.TokenType == JsonTokenType.StartObject)
            {
                var singleItem = JsonSerializer.Deserialize<T>(ref reader, options);
                if (singleItem != null)
                {
                    list.Add(singleItem);
                }
            }

            return list;
        }

        public override void Write(Utf8JsonWriter writer, List<T> value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }
}