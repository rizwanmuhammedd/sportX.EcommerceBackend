namespace Sportex.Application.Common;

public class ApiResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = "";
    public object Data { get; set; } = new { };

    // SUCCESS WITH DATA
    public static ApiResponse Success(string message, object data)
        => new ApiResponse
        {
            StatusCode = 200,
            Message = message,
            Data = data
        };

    // SUCCESS WITHOUT DATA
    public static ApiResponse Success(string message)
        => new ApiResponse
        {
            StatusCode = 200,
            Message = message,
            Data = new { success = true }
        };

    // FAIL
    public static ApiResponse Fail(int code, string message)
        => new ApiResponse
        {
            StatusCode = code,
            Message = message,
            Data = new { success = false }
        };
}
