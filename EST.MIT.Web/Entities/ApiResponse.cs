using System.Net;

namespace EST.MIT.Web.Entities;

public class ApiResponse
{
    public Dictionary<string, string> Errors { get; set; } = new();
    public dynamic Data { get; set; } = default!;
    public bool IsSuccess { get; set; }
    public HttpStatusCode StatusCode { get; set; }

    public ApiResponse(bool Success, HttpStatusCode StatusCode)
    {
        this.IsSuccess = Success;
        this.StatusCode = StatusCode;
    }

    public ApiResponse(bool Success, HttpStatusCode StatusCode, Dictionary<string, string> Errors)
    {
        this.IsSuccess = Success;
        this.StatusCode = StatusCode;
        this.Errors = Errors;
        this.Data = Data;
    }
}

public class ApiResponse<T> where T : class
{
    public Dictionary<string, List<string>> Errors { get; set; } = new();
    public T Data { get; set; } = default!;
    public bool IsSuccess { get; set; }
    public HttpStatusCode StatusCode { get; set; }

    public ApiResponse(bool success)
    {
        this.IsSuccess = success;
    }

    public ApiResponse(HttpStatusCode StatusCode)
    {
        this.IsSuccess = StatusCode == HttpStatusCode.OK || StatusCode == HttpStatusCode.Created;
        this.StatusCode = StatusCode;
    }

    public ApiResponse(HttpStatusCode StatusCode, Dictionary<string, List<string>> Errors)
    {
        this.IsSuccess = StatusCode == HttpStatusCode.OK;
        this.StatusCode = StatusCode;
        this.Errors = Errors;
        this.Data = Data;
    }
}
