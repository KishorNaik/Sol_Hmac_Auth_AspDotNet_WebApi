using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Sol_Demos.Extensions.Services;
using System.Text;

namespace Sol_Demos.Extensions.Services;

public class TraceIdEnricher : ILogEventEnricher
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITraceIdService _traceIdService;

    public TraceIdEnricher(ITraceIdService traceIdService, IHttpContextAccessor httpContextAccessor)
    {
        _traceIdService = traceIdService;
        _httpContextAccessor = httpContextAccessor;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var context = _httpContextAccessor.HttpContext;
        if (context != null)
        {
            var traceId = _traceIdService.GetOrGenerateTraceId(context);
            var traceIdProperty = propertyFactory.CreateProperty("RequestTraceId", traceId);
            logEvent.AddPropertyIfAbsent(traceIdProperty);
        }
    }
}

public static class SeriLogService
{
    public static void AddSerilog(this WebApplicationBuilder builder)
    {
        // Access the IServiceProvider
        var serviceProvider = builder.Services.BuildServiceProvider();
        var traceIdService = serviceProvider?.GetService<ITraceIdService>();
        var httpContextAccessor = serviceProvider?.GetService<IHttpContextAccessor>();

        var logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithEnvironmentName()
            .Enrich.With(new TraceIdEnricher(traceIdService!, httpContextAccessor!))
            .WriteTo.Async(x =>
            {
                x.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} TraceId:{RequestTraceId} Env:{EnvironmentName} {NewLine} {Exception}");
            })
            .CreateLogger();

        builder.Logging.ClearProviders();

        Log.Logger = logger;
        builder.Logging.AddSerilog(logger);

        builder.Host.UseSerilog();
    }
}