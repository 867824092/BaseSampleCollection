using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreIntegration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        [HttpPost]
        public async Task Post([FromForm] FilesModel filesModel) {
            var lists = new List<int>();
            lists.Where(u => u > 0);
            await Task.Delay(10);
           await HttpContext.Response.WriteAsync($"接收到文件数量：{filesModel.Files?.Count}");
        }
    }

    public class FilesModel {
        public int Age { get; set; }
        public string Name { get; set; }
        public IFormFileCollection Files { get; set; }
    }
}
