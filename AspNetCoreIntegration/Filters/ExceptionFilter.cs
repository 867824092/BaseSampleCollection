using Microsoft.AspNetCore.Mvc.Filters;

namespace AspNetCoreIntegration.Filters; 

public class ExceptionFilterAttribute : Microsoft.AspNetCore.Mvc.Filters.ExceptionFilterAttribute {
    
    
    public override Task OnExceptionAsync(ExceptionContext context) {
        return base.OnExceptionAsync(context);
    }
}