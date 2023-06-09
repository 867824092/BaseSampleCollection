using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.Extensions.DiagnosticAdapter;

namespace AspNetCoreIntegration.DiagnosticCollectors; 

public sealed class MvcActionDiagnosticCollector {
    private readonly ILogger<MvcActionDiagnosticCollector> _logger;
    public MvcActionDiagnosticCollector(ILogger<MvcActionDiagnosticCollector> logger) {
        _logger = logger;
    }
    
    [DiagnosticName(BeforeActionEventData.EventName)]
    public void OnReceiveRequest(ActionDescriptor actionDescriptor,
        HttpContext httpContext,
        RouteData routeData) {
        _logger.LogInformation($"Controller '{actionDescriptor?.DisplayName}' is will to be executed.");
    }
}