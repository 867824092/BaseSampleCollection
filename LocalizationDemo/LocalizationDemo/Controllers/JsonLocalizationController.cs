using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace LocalizationDemo.Controllers {
    [Route("api/json")]
    [ApiController]
    public class JsonLocalizationController : ControllerBase {
        private readonly IStringLocalizer<JsonLocalizationController> _stringLocalizer;

        public JsonLocalizationController(IStringLocalizer<JsonLocalizationController> stringLocalizer) {
            _stringLocalizer = stringLocalizer;
        }
        [HttpGet]
        public async Task<string> Get() {
            await Task.Delay(100);
            return _stringLocalizer["Welcome"];
        }

    }
}
