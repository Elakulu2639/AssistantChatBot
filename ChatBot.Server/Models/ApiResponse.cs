namespace ChatBot.Server.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public string[] Errors { get; set; } = Array.Empty<string>();

        public static ApiResponse<T> CreateSuccess(T data, string? message = null)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Errors = Array.Empty<string>()
            };
        }

        public static ApiResponse<T> CreateError(string message, string[]? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = default,
                Errors = errors ?? Array.Empty<string>()
            };
        }
    }
}