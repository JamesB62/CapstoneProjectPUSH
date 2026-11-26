namespace CapstoneProjectPUSH.Services
{
    // Outcome from an API call
    public class ApiResult<T>
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;

        public static ApiResult<T> Ok(int statusCode, T? data)
        {
            return new ApiResult<T>
            {
                Success = true,
                StatusCode = statusCode,
                Data = data
            };
        }

        public static ApiResult<T> Fail(int statusCode, string message)
        {
            return new ApiResult<T>
            {
                Success = false,
                StatusCode = statusCode,
                Message = message
            };
        }
    }
}