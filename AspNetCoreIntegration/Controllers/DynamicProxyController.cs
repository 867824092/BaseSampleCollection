using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreIntegration.DynamicProxy;
using Castle.DynamicProxy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreIntegration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DynamicProxyController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> Get([FromServices]ICalculate calculate) {
            await calculate.Add(1,2);
            return Ok();
        }
    }
}
