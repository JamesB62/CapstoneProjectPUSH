namespace CapstoneProjectPUSH.Services
{
    // Image url to json converter
    public class ImageUrlJsonConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? value = reader.GetString();

            if (value is null)
            {
                return string.Empty;
            }

            return ApiClient.GetImageUrl(value);
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
}