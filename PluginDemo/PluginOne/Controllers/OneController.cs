using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PluginOne.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class OneController : ControllerBase {

        [HttpGet]
        public async Task<IActionResult> Get() {
            await Task.Delay(1000 * 60);
            return new JsonResult(new Student {
                 Age = 10,
                 Name = "Admin"
            });
        }
    }

    public class Student {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Sex {
            get {
                return "女";
            }
        }
    }
}
