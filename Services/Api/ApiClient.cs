using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace CapstoneProjectPUSH.Services
{
    // One shared connection to the api -- also holds the login token and sends it with every request
    public class ApiClient
    {
        // The hosted api's address
        public const string BaseUrl = "https://push.runasp.net/";

        // Matches the api's JSON options
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

        private readonly HttpClient _httpClient;

        public ApiClient()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(BaseUrl);
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        // The current login token, or null when nobody is logged in
        public string? Token { get; private set; }

        public void SetToken(string? token)
        {
            Token = token;

            if (string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        // Turns a stored image path into a full web address
        public static string GetImageUrl(string? imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return string.Empty;
            }

            if (imagePath.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
            {
                return BaseUrl.TrimEnd('/') + imagePath;
            }

            return imagePath;
        }

        // Http request types
        public Task<ApiResult<T>> GetAsync<T>(string path)
        {
            return SendAsync<T>(HttpMethod.Get, path, null);
        }

        public Task<ApiResult<T>> PostAsync<T>(string path, object? body)
        {
            return SendAsync<T>(HttpMethod.Post, path, body);
        }

        public Task<ApiResult<T>> PutAsync<T>(string path, object? body)
        {
            return SendAsync<T>(HttpMethod.Put, path, body);
        }

        public Task<ApiResult<T>> DeleteAsync<T>(string path)
        {
            return SendAsync<T>(HttpMethod.Delete, path, null);
        }

        // Uploads a photo method
        public async Task<ApiResult<T>> PostFileAsync<T>(string path, Stream fileStream, string fileName)
        {
            try
            {
                using (MultipartFormDataContent form = new MultipartFormDataContent())
                {
                    StreamContent fileContent = new StreamContent(fileStream);
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(GetImageContentType(fileName));
                    form.Add(fileContent, "file", fileName);

                    using (HttpResponseMessage response = await _httpClient.PostAsync(path, form))
                    {
                        return await ReadResultAsync<T>(response);
                    }
                }
            }
            catch (HttpRequestException)
            {
                return ApiResult<T>.Fail(0, "Couldn't reach the server. Check your internet connection.");
            }
            catch (TaskCanceledException)
            {
                return ApiResult<T>.Fail(0, "The server took too long to respond. Please try again.");
            }
        }

        // Helpers
        // Send http request
        private async Task<ApiResult<T>> SendAsync<T>(HttpMethod method, string path, object? body)
        {
            try
            {
                using (HttpRequestMessage request = new HttpRequestMessage(method, path))
                {
                    if (body != null)
                    {
                        request.Content = JsonContent.Create(body, body.GetType(), null, JsonOptions);
                    }

                    using (HttpResponseMessage response = await _httpClient.SendAsync(request))
                    {
                        return await ReadResultAsync<T>(response);
                    }
                }
            }
            catch (HttpRequestException)
            {
                return ApiResult<T>.Fail(0, "Couldn't reach the server. Check your internet connection.");
            }
            catch (TaskCanceledException)
            {
                return ApiResult<T>.Fail(0, "The server took too long to respond. Please try again.");
            }
        }

        // Converts http response into result -- either a success with the data, or a failure with an error message
        private static async Task<ApiResult<T>> ReadResultAsync<T>(HttpResponseMessage response)
        {
            int statusCode = (int)response.StatusCode;
            string json = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    T? data = default;

                    if (json.Length > 0)
                    {
                        data = JsonSerializer.Deserialize<T>(json, JsonOptions);
                    }

                    return ApiResult<T>.Ok(statusCode, data);
                }
                catch (JsonException)
                {
                    return ApiResult<T>.Fail(statusCode, "Unexpected response from the server.");
                }
            }

            string message = ReadMessage(json);

            // Failed -- use the api's own message if it sent one
            if (message.Length == 0)
            {
                message = GetDefaultMessage(statusCode);
            }

            return ApiResult<T>.Fail(statusCode, message);
        }

        // Reads the message field from an error reply, if there is one
        private static string ReadMessage(string json)
        {
            if (json.Length == 0)
            {
                return string.Empty;
            }

            try
            {
                using (JsonDocument document = JsonDocument.Parse(json))
                {
                    JsonElement messageElement;

                    if (document.RootElement.ValueKind == JsonValueKind.Object
                        && document.RootElement.TryGetProperty("message", out messageElement)
                        && messageElement.ValueKind == JsonValueKind.String)
                    {
                        string? text = messageElement.GetString();

                        if (text != null)
                        {
                            return text;
                        }
                    }
                }
            }
            catch (JsonException)
            {
            }

            return string.Empty;
        }

        // Response messages based on common status code errors
        private static string GetDefaultMessage(int statusCode)
        {
            if (statusCode == 401)
            {
                return "Your session has ended. Please log in again.";
            }
            else if (statusCode == 403)
            {
                return "You don't have access to do that.";
            }
            else if (statusCode == 404)
            {
                return "That could not be found.";
            }
            else
            {
                return "Something went wrong. Please try again.";
            }
        }

        // Reads the image file type
        private static string GetImageContentType(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLowerInvariant();

            if (extension == ".png")
            {
                return "image/png";
            }
            else if (extension == ".webp")
            {
                return "image/webp";
            }
            else
            {
                return "image/jpeg";
            }
        }
    }
}