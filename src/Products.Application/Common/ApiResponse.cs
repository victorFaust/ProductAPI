namespace Products.Application.Common
{

    public class ApiResponse<T>
    {
        public bool IsSuccess { get; init; }
        public int ResponseCode { get; init; }
        public string ResponseMsg { get; init; } = string.Empty;
        public T? Data { get; init; }

        public static ApiResponse<T> Success(T data, string message = "Success", int code = 200) =>
            new() { IsSuccess = true, ResponseCode = code, ResponseMsg = message, Data = data };

        public static ApiResponse<T> Fail(string message, int code, T? data = default) =>
            new() { IsSuccess = false, ResponseCode = code, ResponseMsg = message, Data = data };
    }
}
