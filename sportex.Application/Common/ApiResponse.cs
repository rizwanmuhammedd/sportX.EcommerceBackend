namespace Sportex.Application.Common;

public class ApiResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = "";
    public object? Data { get; set; }

    public static ApiResponse Success(string message, object? data = null)
        => new ApiResponse { StatusCode = 200, Message = message, Data = data };

    public static ApiResponse Fail(int code, string message)
        => new ApiResponse { StatusCode = code, Message = message };
}
