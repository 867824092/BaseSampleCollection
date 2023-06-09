using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace LocalizationDemo.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase {
        private readonly IStringLocalizer<SharedResource> _stringLocalizer;
        private readonly IStringLocalizer<ValuesController> _valuesStringLocalizer;
        public ValuesController(IStringLocalizer<SharedResource> stringLocalizer,
            IStringLocalizer<ValuesController> valuesStringLocalizer) {
            _stringLocalizer = stringLocalizer;
            _valuesStringLocalizer = valuesStringLocalizer;
        }
        [HttpGet]
        public async Task<string> Get() {
            await Task.Delay(100);
            return _stringLocalizer["Hello"] + "    " + _valuesStringLocalizer["Hello"];
        }
    }
}
