using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Polly;
using Polly.CircuitBreaker;

namespace AspNetCoreIntegration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TroubleshootingController : ControllerBase {
        private static readonly AsyncCircuitBreakerPolicy breaker =  Policy
            .Handle<HttpRequestException>()
            .Or<Exception>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: 2, 
                durationOfBreak: TimeSpan.FromMinutes(1),
                onBreak: (exception, span) => {
                    Console.WriteLine($"发生异常{exception.Message},{span}");
                },
                onReset: () => {
                    Console.WriteLine("OnRest事件");
                },
                onHalfOpen: () => {
                    Console.WriteLine("onHalfOpen事件");
                }
            );
        
        [HttpGet]
        public async Task Get() {
            await breaker.ExecuteAsync(() => {
                throw new Exception("测试异常");
            });
            await HttpContext.Response.WriteAsync("测试");
        }
    }
}
