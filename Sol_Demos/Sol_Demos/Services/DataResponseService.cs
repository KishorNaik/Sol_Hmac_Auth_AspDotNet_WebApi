using Sol_Demos.Extensions.Services;

namespace Sol_Demos.Services;

public class DataResponse<T>
{
    public bool Success { get; set; }

    public string? Message { get; set; }

    public int StatusCode { get; set; }

    public T? Data { get; set; }

    public string? TraceId { get; set; }
}

public class DataResponseFactory
{
    private readonly ITraceIdService _traceIdService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DataResponseFactory(ITraceIdService traceIdService, IHttpContextAccessor httpContextAccessor)
    {
        _traceIdService = traceIdService;
        _httpContextAccessor = httpContextAccessor;
    }

    public DataResponse<T> SetResponse<T>(bool success, string? message, int statusCode, T? data)
    {
        return new DataResponse<T>
        {
            Success = success,
            Message = message,
            StatusCode = statusCode,
            Data = data,
            TraceId = _traceIdService.GetOrGenerateTraceId(_httpContextAccessor.HttpContext!)
        };
    }
}