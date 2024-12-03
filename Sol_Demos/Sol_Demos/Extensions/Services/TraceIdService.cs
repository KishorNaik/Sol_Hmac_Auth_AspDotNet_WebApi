namespace Sol_Demos.Extensions.Services;

public interface ITraceIdService
{
    string GetOrGenerateTraceId(HttpContext context);
}

public class TraceIdService : ITraceIdService
{
    private const string TraceIdKey = "TraceId";

    public string GetOrGenerateTraceId(HttpContext context)
    {
        if (!context.Items.ContainsKey(TraceIdKey))
        {
            context.Items[TraceIdKey] = Guid.NewGuid().ToString();
        }

        return context.Items[TraceIdKey] as string;
    }
}