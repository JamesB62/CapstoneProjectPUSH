using System.Text.Json;
using System.Text.Json.Serialization;

namespace CapstoneProjectPUSH.Services
{
    // Converts utc datetime values to phone's time zone
    public class LocalDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Read the datetime stored in the json response
            DateTime value = reader.GetDateTime();

            // Checks if the datetime is marked as utc
            if (value.Kind == DateTimeKind.Utc)
            {
                // If true, converts the datetime to phone's time zone
                return value.ToLocalTime();
            }

            // If false, return value unchanged -- cannot reliably convert value
            return value;
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToUniversalTime());
        }
    }
}