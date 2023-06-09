using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PluginManagent.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase {
        [HttpGet]
        public async Task<IActionResult> Get() {
            await Task.Delay(10);
            return new JsonResult(new People ());
        }
    }
}
