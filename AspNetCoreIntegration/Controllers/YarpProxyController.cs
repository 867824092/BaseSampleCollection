using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreIntegration.Controllers
{
    [Route("api/yarp")]
    [ApiController]
    public class YarpProxyController : ControllerBase
    {

        [HttpGet]
        public ActionResult<string> Get(string path) {
            return Ok($"YarpProxyController.Get: {path}，origin: {Request.Path}");
        }

        [HttpPost]
        public ActionResult Post(RequestModel model) {
            //计算name传入值所占字节大小
            var bytes = System.Text.Encoding.UTF8.GetBytes(model.Name);
            Console.WriteLine($"字节大小：{bytes.Length}");
            return Ok();
        }
    }
    public class RequestModel {
        public string Name { get; set; }
    }
}
